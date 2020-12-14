using BO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Threading;

namespace BL
{
    public interface IWorkflowBL
    {

        public int SaveWorkflowComment(int intA01ID, string strComment,string strUploadGUID, List<int> a45ids_restrict_to);
        public int RunWorkflowStep(BO.a01Event rec, BO.b06WorkflowStep recB06, List<BO.a41PersonToEvent> lisNominee, string strComment, string strUploadGUID, bool bolManualStep);
        public List<BO.a11EventForm> GetForms4Validation(BO.a01Event rec, BO.b06WorkflowStep recB06);
        public void CheckDefaultWorkflowStatus(int a01id,bool bolSimulation=false);
        public bool RunAutoWorkflowStepByRawSql(BO.a01Event recA01, BO.b06WorkflowStep recB06);
    }
    class WorkflowBL : BaseBL, IWorkflowBL
    {
        public WorkflowBL(BL.Factory mother) : base(mother)
        {

        }

        public bool RunAutoWorkflowStepByRawSql(BO.a01Event recA01, BO.b06WorkflowStep recB06)
        {
            //testuje podmínky automaticky spustitelného wrk kroku + případně ho spouští
            if (GetAutoWorkflowSQLResult(recA01, recB06) == 1)  //pokud sql vrací hodnotu 1, pak je podmínka splněna
            {
                if ( RunWorkflowStep(recA01, recB06, null, null, null, false) > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return false;
        }
        public int GetAutoWorkflowSQLResult(BO.a01Event recA01,BO.b06WorkflowStep recB06)
        {
            string strSQL = DL.BAS.ParseMergeSQL(recB06.b06ValidateAutoMoveSQL, recA01.pid.ToString());
            return _db.GetIntegerFromSql(strSQL);
        }
        public int RunWorkflowStep(BO.a01Event rec, BO.b06WorkflowStep recB06, List<BO.a41PersonToEvent> lisNominee, string strComment, string strUploadGUID, bool bolManualStep)
        {
            if (ValidateWorkflowStepBeforeRun(rec, recB06, lisNominee, strComment, strUploadGUID, bolManualStep) == false)
            {
                return 0;
            }
            if (string.IsNullOrEmpty(strUploadGUID) == false)
            {
                _mother.o27AttachmentBL.SaveChangesAndUpload(strUploadGUID, 101, rec.pid);
            }

            if (recB06.b02ID_Target > 0)
            {
                //změnit cílový status akce
                var recB02 = _mother.b02WorkflowStatusBL.Load(recB06.b02ID_Target);
                SaveStepWithChangeStatus(rec, recB06, recB02, strComment, bolManualStep);
                
            }
            else
            {
                SaveStepWithoutChangeStatus(rec, recB06, strComment, bolManualStep);
            }
            
            if (recB06.b06IsNominee)
            {
                SaveNominee(rec, recB06, lisNominee);
            }
            
            if (recB06.b06IsFormAutoLock)
            {   //spuštění kroku uzamyká otevřené formuláře v akci
                var lisA11 = _mother.a11EventFormBL.GetList(new BO.myQuery("a11") { a01id = rec.pid });
                foreach(var recA11 in lisA11)
                {
                    _mother.a11EventFormBL.LockUnLockForm(recA11.pid, true);
                }
            }
            if (recB06.b06IsToDoGeneration)
            {   //krok generuje úkol
                AutoGenerateToDoFromWorkflow(rec, recB06);
            }
            if (string.IsNullOrEmpty(recB06.b06RunSQL)==false)
            {   //krok má spouštět SQL dotaz
                _db.RunSql(_db.ParseMergeSQL(recB06.b06RunSQL, rec.pid.ToString()));
            }

            RunB09Commands_By_Step(rec, recB06);    //spustit SQL příkazy spojené s krokem
            if (recB06.b02ID_Target > 0)
            {
                RunB09Commands_By_Status(rec, recB06.b02ID_Target); //spustit SQL příkazy spojené s novým stavem
            }


            TrySendNotificationsToStep(rec, recB06, strComment);

            if (recB06.b02ID_Target > 0)
            {   //změna stavu může být také notifikována
                TrySendNotificationsToStatus(rec, recB06.b02ID_Target, strComment);
            }

            return rec.pid;
        }

        private int SaveStepWithChangeStatus(BO.a01Event rec, BO.b06WorkflowStep recB06, BO.b02WorkflowStatus recB02, string strComment, bool bolManualStep)
        {
            _db.RunSql("UPDATE a01Event SET b02ID=@b02id WHERE a01ID=@pid", new { b02id = recB02.pid, pid = rec.pid });
            switch (recB02.b02AutoUpdateScopeFlag)
            {
                case b02AutoUpdateScope.Slaves:
                    //aktualizovat statusy u dětí matky
                    var slave = _db.Load<BO.GetInteger>("SELECT TOP 1 a01ID as Value FROM a01Event WHERE a01ParentID=@pid", new { pid = rec.pid });
                    if (slave != null && slave.Value > 0)
                    {
                        //aktualizovat statusy u dětí matky
                        _db.RunSql("UPDATE a01Event SET b02ID=@b02id WHERE a01ParentID=@pid", new { b02id = recB06.b02ID_Target, pid = rec.pid });
                    }
                    break;
                case b02AutoUpdateScope.Parent:
                    if (rec.a01ParentID > 0)
                    {
                        _db.RunSql("UPDATE a01Event SET b02ID=@b02id WHERE a01ID=@parentid", new { b02id = recB06.b02ID_Target, parentid = rec.a01ParentID });
                    }
                    break;
            }
            
            //zápis do historie workflow
            var c = new BO.b05Workflow_History() { a01ID = rec.pid, b05IsCommentOnly = false, b05IsManualStep = bolManualStep, b05Comment = strComment, b02ID_From = rec.b02ID, b02ID_To = recB02.pid };
            if (recB06 != null)
            {
                c.b06ID = recB06.pid;
            }
            int intB05ID = _mother.b05Workflow_HistoryBL.Save(c);

            return intB05ID;
        }
        private int SaveStepWithoutChangeStatus(BO.a01Event rec, BO.b06WorkflowStep recB06, string strComment, bool bolManualStep)
        {
            //zápis do historie workflow
            var c = new BO.b05Workflow_History() { a01ID = rec.pid, b05IsCommentOnly = false, b05IsManualStep = bolManualStep, b05Comment = strComment };
            if (recB06 != null)
            {
                c.b06ID = recB06.pid;
            }
            int intB05ID = _mother.b05Workflow_HistoryBL.Save(c);
            return intB05ID;
        }
        private void SaveNominee(BO.a01Event rec, BO.b06WorkflowStep recB06, List<BO.a41PersonToEvent> lisNominee)
        {
            if (lisNominee == null) return;

            foreach (var c in lisNominee)                
            {
               
                if (c.a45ID == BO.EventRoleENUM.NominovanySchvalovatel)
                {
                    //98: nominovaný schvalovatel, odstranit dosavadní/staré nominace ke stejnému kroku
                    var test = _db.Load<BO.GetInteger>("select top 1 a41id as Value FROM a41PersonToEvent WHERE a45ID=98 AND a01ID=@pid AND b06ID_NomineeSource=@b06id", new { pid = rec.pid, b06id = c.b06ID_NomineeSource });
                    if (test != null)
                    {
                        _db.RunSql("DELETE FROM a41PersonToEvent WHERE a45ID=98 AND a01ID=@pid AND b06ID_NomineeSource=@b06id", new { pid = rec.pid, b06id = c.b06ID_NomineeSource });
                    }                                        
                }
                else
                {
                    //řešitel,vedoucí,čtenář
                    //odstranit původní záznamy pro danou roli akce, role se nahrazuje, nikoliv přidává jako další
                    var test = _db.Load<BO.GetInteger>("select top 1 a41id as Value FROM a41PersonToEvent WHERE a01ID=@pid AND a45ID=@a45id", new { pid = rec.pid, a45id = (int)c.a45ID });
                    if (test != null)
                    {
                        _db.RunSql("DELETE FROM a41PersonToEvent WHERE a01ID=@pid AND a45ID=@a45id", new { pid = rec.pid, a45id = (int)c.a45ID });
                    }                        
                }
            }
            foreach (var c in lisNominee)
            {
                _mother.a41PersonToEventBL.Save(c, true, recB06);
            }
        }

        public void CheckDefaultWorkflowStatus(int a01id, bool bolSimulation = false)
        {
            BO.a01Event rec = _mother.a01EventBL.Load(a01id);
            if (rec.b02ID > 0)
            {
                return; //akce má status, nemusí se nahazovat a testovat zbytek
            }
            if (rec.a01ParentID > 0)    //nová podřízená akce automaticky přebírá aktuální workflow stav nadřízené akce - jinak se 
            {
                var recParent = _mother.a01EventBL.Load(rec.a01ParentID);
                _db.RunSql("UPDATE a01Event SET b02ID=@b02id WHERE a01ID=@pid", new { b02id = recParent.b02ID, pid = rec.pid });
                return;
            }
            var mq = new BO.myQuery("b02") { IsRecordValid = true, b01id = rec.b01ID };
            var recDefB02 = _mother.b02WorkflowStatusBL.GetList(mq).FirstOrDefault(p => p.b02IsDefaultStatus == true);
            if (recDefB02 == null)
            {
                return;   //workflow šablona nemá výchozí workflow status
            }
            if (SaveStepWithChangeStatus(rec, null, recDefB02, null, false) > 0)
            {       
                if (bolSimulation)
                {
                    return; //v rámci simulace fomuláře raději dále v akci nic automatického nedělat
                }
                mq = new BO.myQuery("a41") { a01id = rec.pid };
                var lisA41 = _mother.a41PersonToEventBL.GetList(mq);

                var recA10 = _mother.a10EventTypeBL.Load(rec.a10ID);
                if (recA10.a45ID_Creator > 0)   //automaticky založit vlastníka nebo zadavatele (role 3 nebo 5) - záleží na typu akce
                {
                    if (lisA41.Where(p => (int)p.a45ID == recA10.a45ID_Creator).Count() == 0)
                    {
                        var recA41 = new BO.a41PersonToEvent() { a01ID = rec.pid, a45ID = (BO.EventRoleENUM)recA10.a45ID_Creator, j02ID = rec.j02ID_Issuer, a41IsAllocateAllPeriod = true };
                        _mother.a41PersonToEventBL.Save(recA41, true);
                    }
                }
                foreach (var cB03 in _mother.b02WorkflowStatusBL.GetListB03(recDefB02.pid))
                {
                    //existují automatičtí výchozí řešitelé statusu
                    var recA41 = new BO.a41PersonToEvent() { a01ID = rec.pid, a45ID = (BO.EventRoleENUM)cB03.a45ID, j11ID = cB03.j11ID};
                    _mother.a41PersonToEventBL.Save(recA41, true);
                }
                
                RunB09Commands_By_Status(rec, recDefB02.pid);  //spouštění pevných SQL příkazů, kterou jsou spojeny s výchozím stavem
            }

            
        }

        public List<BO.a11EventForm> GetForms4Validation(BO.a01Event rec, BO.b06WorkflowStep recB06)
        {   //vrátí seznam formulář, u kterých je třeba zkontrolovat vyplnění otázek
            var lisA11_TestForms = new List<BO.a11EventForm>();
            if (recB06.b06IsFormValidationRequired)
            {
                var mq = new BO.myQuery("a11");
                mq.a01id = rec.pid;
                var lisA11 = _mother.a11EventFormBL.GetList(mq).Where(p => p.a11IsLocked == false).ToList();
                if (rec.a01ChildsCount > 0) //akce má podřízené potomky
                {
                    mq = new BO.myQuery("a11");
                    mq.a01parentid = rec.pid;
                    var lisChilds = _mother.a11EventFormBL.GetList(mq).ToList();
                    lisA11.InsertRange(0, lisChilds);
                }
                lisA11_TestForms = lisA11;
            }
            var lisB13 = _mother.b06WorkflowStepBL.GetListB13(recB06.pid);
            if (lisB13.Count() > 0) //spuštění kroku vyžaduje kontrolu vybraných formulářů v akci
            {
                foreach (var cB13 in lisB13)
                {
                    var mq = new BO.myQuery("a11");
                    mq.a01id = rec.pid;
                    mq.f06id = cB13.f06ID;
                    var lisA11 = _mother.a11EventFormBL.GetList(mq).ToList();
                    if (rec.a01ChildsCount > 0) //akce má podřízené potomky
                    {
                        mq = new BO.myQuery("a11");
                        mq.a01parentid = rec.pid;
                        mq.f06id = cB13.f06ID;
                        var lisChilds = _mother.a11EventFormBL.GetList(mq).ToList();
                        lisA11.InsertRange(0, lisChilds);
                    }
                    foreach (var c in lisA11)
                    {
                        if (lisA11_TestForms.Where(p => p.pid == c.a11ID).Count() == 0)
                        {
                            lisA11_TestForms.Add(c);
                        }
                    }
                }
            }

            return lisA11_TestForms;
        }
        private bool ValidateWorkflowStepBeforeRun(BO.a01Event rec, BO.b06WorkflowStep recB06, List<BO.a41PersonToEvent> lisNominee, string strComment, string strUploadGUID, bool bolManualStep)
        {
            //Kromě validace je třeba otestovat obsah formulářů GetForms4Validation() v UI!!
            if (recB06.pid == 0)
            {
                this.AddMessage("Musíte zvolit z nabídky konkrétní krok!"); return false;
            }
            if (recB06.b06IsCommentRequired && string.IsNullOrEmpty(strComment) == true)
            {
                this.AddMessage(string.Format("Krok {0} vyžaduje zapsat komentář!", recB06.b06Name)); return false;
            }
            if (recB06.b06IsNomineeRequired && (lisNominee == null || lisNominee.Count() == 0))
            {
                this.AddMessage(string.Format("Krok {0} vyžaduje nominaci!", recB06.b06Name)); return false;
            }
            if (lisNominee != null)
            {
                if (lisNominee.Where(p => p.b06ID_NomineeSource == 0).Count() > 0)
                {
                    this.AddMessage("V řádku předané nominace chybí identifikace kroku."); return false;
                }
                if (lisNominee.Where(p => p.a45ID == 0).Count() > 0)
                {
                    this.AddMessage("V řádku předané nominace chybí identifikace role v akci."); return false;
                }
                if (lisNominee.Where(p => p.j02ID == 0 && p.j11ID == 0).Count() > 0)
                {
                    this.AddMessage("V řádku předané nominace není osoba ani tým."); return false;
                }
            }
            if (recB06.b06IsRunOneInstanceOnly) //krok je povolen spustit v akci pouze jednou
            {
                var mq = new BO.myQuery("b05");
                mq.a01id = rec.pid;
                var lisB05 = _mother.b05Workflow_HistoryBL.GetList(mq);
                if (lisB05.Where(p => p.b06ID == recB06.pid).Count() > 0)
                {
                    this.AddMessage(string.Format("Krok {0} je povoleno spouštět pouze jednou!", recB06.b06Name)); return false;
                }
            }
            if (recB06.b06ValidateBeforeRunSQL != null)
            {
                string strSQL = _db.ParseMergeSQL(recB06.b06ValidateBeforeRunSQL, rec.pid.ToString());
                if (_db.GetIntegerFromSql(strSQL) != 1)
                {
                    this.AddMessage(recB06.b06ValidateBeforeErrorMessage); return false;
                }
            }
            if (recB06.b06IsAttachmentTestRequired) //spuštění kroku vyžaduje přiložení povinných příloh podle nastavení tématu akce
            {
                var lisA14 = _mother.a08ThemeBL.GetListA14(rec.a08ID).Where(p => p.a14IsRequired == true);
                foreach(var c in lisA14)
                {
                    var lisO27 = _mother.o27AttachmentBL.GetList(new BO.myQuery("o27") { a01id = rec.pid, o13id = c.o13ID },null);
                    if (lisO27.Count() == 0)
                    {
                        this.AddMessage(string.Format("Povinný typ dokumentu [{0}] není přiložen v akci.", _mother.o13AttachmentTypeBL.Load(c.o13ID).o13Name));
                        return false;
                    }
                }
                
            }
            if (_mother.b06WorkflowStepBL.GetNeededO13IDs(recB06.pid).Count() > 0) //spuštění kroku kontroluje existenci vybraných typů dokumentů v akci
            {
                foreach (int intO13ID in _mother.b06WorkflowStepBL.GetNeededO13IDs(recB06.pid))
                {
                    var lisO27 = _mother.o27AttachmentBL.GetList(new BO.myQuery("o27") { a01id = rec.pid, o13id = intO13ID }, null);
                    if (lisO27.Count() == 0)
                    {
                        this.AddMessage(string.Format("Tento krok vyžaduje v akci přiložený dokument typu [{0}].", _mother.o13AttachmentTypeBL.Load(intO13ID).o13Name));
                        return false;
                    }
                }
            }



            return true;
        }

        public int SaveWorkflowComment(int intA01ID, string strComment,string strUploadGUID, List<int> a45ids_restrict_to)
        {
            if (string.IsNullOrEmpty(strComment) || strComment.Trim().Length<5)
            {
                this.AddMessage("Délka komentáře je příliš malá.");return 0;
            }
            var c = new BO.b05Workflow_History() { a01ID = intA01ID, b05IsCommentOnly = true, b05IsManualStep = true, b05Comment = strComment, b05IsCommentRestriction = false };
            if (a45ids_restrict_to != null && a45ids_restrict_to.Count > 0)
            {
                c.b05IsCommentRestriction = true;   //komentář má omezený okruh čtenářů
            }
            int intB05ID = _mother.b05Workflow_HistoryBL.Save(c);

            if (c.b05IsCommentRestriction)
            {
                _db.RunSql("INSERT INTO b04WorkflowComment_Restriction(b05ID,a45ID) SELECT @pid,a45ID FROM a45EventRole WHERE a45ID IN (" + string.Join(",", a45ids_restrict_to) + ")");
            }
            if (string.IsNullOrEmpty(strUploadGUID) == false)
            {
                _mother.o27AttachmentBL.SaveChangesAndUpload(strUploadGUID, 101, intA01ID);
            }

            return intB05ID;
        }

        private void TrySendNotificationsToStep(BO.a01Event rec, BO.b06WorkflowStep recB06, string strComment)
        {
            //posílá notifikační zprávy v rámci workflow kroku
            var lisB11 = _mother.b06WorkflowStepBL.GetListB11(recB06.pid);
            if (lisB11.Count() == 0) return; //nejsou definovány notifikační pravidla k workflow kroku
            var lisA41 = _mother.a41PersonToEventBL.GetList(new BO.myQuery("a41") { a01id = rec.pid });
            foreach(var c in lisB11)
            {
                var persons = GetAllPersonsOfEventRole(rec, c.a45ID, c.j04ID, c.j11ID, lisA41);
                handle_send_notification(rec, persons, c.b65ID, strComment);
            }
        }
        private void TrySendNotificationsToStatus(BO.a01Event rec,int intB02ID,string strComment)
        {
            //posílá notifikační zprávy v rámci workflow stavu
            var lisB07 = _mother.b02WorkflowStatusBL.GetListB07(intB02ID);
            if (lisB07.Count() == 0) return;    //nejsou definovány notifikační pravidla k workflow kroku
            var lisA41 = _mother.a41PersonToEventBL.GetList(new BO.myQuery("a41") { a01id = rec.pid });
            foreach(var c in lisB07)
            {
                var persons = GetAllPersonsOfEventRole(rec, c.a45ID, c.j04ID, c.j11ID, lisA41);
                handle_send_notification(rec, persons, c.b65ID, strComment);                                                    
            }
        }
        private void handle_send_notification(BO.a01Event rec,List<BO.j02Person> persons,int b65id, string strComment)
        {
            if (persons.Count() == 0) return;
            var recB65 = _mother.b65WorkflowMessageBL.MailMergeRecord(b65id, rec.pid, null);

            if (recB65.b65MessageBody.Contains("#comment#"))
            {
                recB65.b65MessageBody = recB65.b65MessageBody.Replace("#comment#", strComment);
            }
            else
            {
                if (!string.IsNullOrEmpty(strComment))
                {
                    recB65.b65MessageBody = recB65.b65MessageBody + Microsoft.VisualBasic.Constants.vbCrLf + "---------------------" + Microsoft.VisualBasic.Constants.vbCrLf + strComment;
                }
            }

            var recX40 = new BO.x40MailQueue() { x29ID = 101, x40DataPID = rec.pid, x40IsAutoNotification = true, x40Subject = recB65.b65MessageSubject, x40Body = recB65.b65MessageBody };
            recX40.x40Recipient = string.Join(",", persons.Select(p => p.j02Email));
            _mother.MailBL.SendMessage(recX40, false);
        }

        private void RunB09Commands_Slaves(BO.a01Event rec, int intB06ID,BO.b10WorkflowCommandCatalog_Binding prikaz)
        {
            if (rec.a01ChildsCount > 0 && rec.a01ParentID == 0)
            {
                //spuštění SQL příkazů podřízených dětí vůči matce
                var mq = new BO.myQuery("a01");
                mq.a01parentid = rec.pid;
                if (prikaz.a10ID_TargetUpdate > 0)
                {
                    mq.a10id = prikaz.a10ID_TargetUpdate;
                }
                var lisChilds = _mother.a01EventBL.GetList(mq);                               
                foreach (var c in lisChilds)
                {
                    if (prikaz.b09SQL != null)
                    {
                        //spustit příkaz u podřízených akcí
                        _db.RunSql(_db.ParseMergeSQL(prikaz.b09SQL, c.pid.ToString(), prikaz.b10Parameter1, prikaz.b10Parameter2));
                    }

                    if (prikaz.b02ID_TargetUpdate > 0)
                    {
                        //nahodit cílový stav u podřízených akcí
                        SaveStepWithChangeStatus(rec, _mother.b06WorkflowStepBL.Load(intB06ID), _mother.b02WorkflowStatusBL.Load(prikaz.b02ID_TargetUpdate), null, false);
                    }
                }
                
            }
        }
        private void RunB09Commands_By_Status(BO.a01Event rec,int intB02ID_Target)
        {
            //spuštění případných příkazů spojených s nahozením nového stavu u akce
            foreach (var prikaz in _mother.b02WorkflowStatusBL.GetListB10(intB02ID_Target).Where(p => p.b09SQL != null))
            {
                switch (prikaz.b10TargetScopeFlag)
                {
                    case b10TargetScopeEnum.ThisAndSlaves:
                    case b10TargetScopeEnum.SlavesOnly:
                        if (prikaz.b10TargetScopeFlag == BO.b10TargetScopeEnum.ThisAndSlaves && (prikaz.a10ID_TargetUpdate==0 || rec.a10ID==prikaz.a10ID_TargetUpdate))
                        {
                            _db.RunSql(_db.ParseMergeSQL(prikaz.b09SQL, rec.pid.ToString(), prikaz.b10Parameter1, prikaz.b10Parameter2));
                        }                        
                        if (rec.a01ChildsCount > 0 && rec.a01ParentID == 0)
                        {
                            //spuštění SQL příkazů podřízených dětí vůči matce
                            var mq = new BO.myQuery("a01");
                            if (prikaz.a10ID_TargetUpdate > 0)
                            {
                                mq.a10id = prikaz.a10ID_TargetUpdate;
                            }
                            mq.a01parentid = rec.pid;
                            var lisChilds = _mother.a01EventBL.GetList(mq);                            
                            foreach (var c in lisChilds)
                            {
                                _db.RunSql(_db.ParseMergeSQL(prikaz.b09SQL, c.pid.ToString(), prikaz.b10Parameter1, prikaz.b10Parameter2));
                            }
                        }
                        break;
                        
                    case b10TargetScopeEnum.ThisOnly:
                        if (prikaz.a10ID_TargetUpdate == 0 || rec.a10ID == prikaz.a10ID_TargetUpdate)
                        {
                            _db.RunSql(_db.ParseMergeSQL(prikaz.b09SQL, rec.pid.ToString(), prikaz.b10Parameter1, prikaz.b10Parameter2));
                        }                        
                        break;
                    
                    case b10TargetScopeEnum.ParentOnly:
                        if (rec.a01ParentID > 0)
                        {
                            if (prikaz.a10ID_TargetUpdate == 0 || _mother.a01EventBL.Load(rec.a01ParentID).a10ID == prikaz.a10ID_TargetUpdate)
                            {
                                _db.RunSql(_db.ParseMergeSQL(prikaz.b09SQL, rec.a01ParentID.ToString(), prikaz.b10Parameter1, prikaz.b10Parameter2));
                            }
                            
                            
                        }
                        break;
                }
                
                
            }

            
        }

        private void RunB09Commands_By_Step(BO.a01Event rec,BO.b06WorkflowStep recB06)
        {            
            //spuštění případných příkazů spojených s krokem
            foreach (var prikaz in _mother.b06WorkflowStepBL.GetListB10(recB06.pid).Where(p => p.b09SQL != null || p.b02ID_TargetUpdate > 0))
            {
                switch (prikaz.b10TargetScopeFlag)
                {
                    case b10TargetScopeEnum.ThisAndSlaves:  //tato a podřízené akce
                        if (prikaz.b09SQL != null && (prikaz.a10ID_TargetUpdate==0 || rec.a10ID==prikaz.a10ID_TargetUpdate))
                        {
                            _db.RunSql(_db.ParseMergeSQL(prikaz.b09SQL, rec.pid.ToString(), prikaz.b10Parameter1, prikaz.b10Parameter2));
                        }
                        RunB09Commands_Slaves(rec, recB06.pid, prikaz);

                        break;
                    case b10TargetScopeEnum.ThisOnly:   //pouze tato akce
                        if (prikaz.a10ID_TargetUpdate==0 || rec.a10ID == prikaz.a10ID_TargetUpdate)
                        {
                            _db.RunSql(_db.ParseMergeSQL(prikaz.b09SQL, rec.pid.ToString(), prikaz.b10Parameter1, prikaz.b10Parameter2));
                            if (prikaz.b02ID_TargetUpdate > 0)
                            {
                                //nahodit cílový stav u akce
                                SaveStepWithChangeStatus(rec, recB06, _mother.b02WorkflowStatusBL.Load(prikaz.b02ID_TargetUpdate), null, false);
                            }
                        }
                        
                        break;

                    case b10TargetScopeEnum.SlavesOnly: //pouze podřízené akce
                        RunB09Commands_Slaves(rec, recB06.pid, prikaz);
                        break;
                    case b10TargetScopeEnum.ParentOnly: //pouze nadřízená akce
                        if (rec.a01ParentID > 0)
                        {
                            if (prikaz.a10ID_TargetUpdate==0 || prikaz.a10ID_TargetUpdate == _mother.a01EventBL.Load(rec.a01ParentID).a10ID)
                            {
                                _db.RunSql(_db.ParseMergeSQL(prikaz.b09SQL, rec.a01ParentID.ToString(), prikaz.b10Parameter1, prikaz.b10Parameter2));
                                if (prikaz.b02ID_TargetUpdate > 0)
                                {
                                    //nahodit cílový stav u akce
                                    SaveStepWithChangeStatus(_mother.a01EventBL.Load(rec.a01ParentID), recB06, _mother.b02WorkflowStatusBL.Load(prikaz.b02ID_TargetUpdate), null, false);
                                }
                            }
                            
                        }

                        break;
                }


            }


        }

        private List<BO.j02Person> GetAllPersonsOfEventRole(BO.a01Event rec,int intA45ID,int intJ04ID,int intJ11ID,IEnumerable<BO.a41PersonToEvent> lisA41)
        {
            //vrátí seznam osob
            var ret = new List<BO.j02Person>();

            if (intA45ID > 0)
            {
                foreach(var c in lisA41.Where(p=>(int)p.a45ID==intA45ID && p.j02ID>0))
                {
                    if (!ret.Exists(p => p.j02ID == c.j02ID))
                    {
                        ret.Add(_mother.j02PersonBL.Load(c.j02ID));
                    }
                }
            }
            if (intJ04ID > 0)
            {
                var recJ04 = _mother.j04UserRoleBL.Load(intJ04ID);
                var lis = new List<BO.j02Person>();
                switch (recJ04.j04RelationFlag)
                {
                    case j04RelationFlagEnum.NoRelation:
                        lis = _mother.j02PersonBL.GetList(new BO.myQueryJ02() { j04id = recJ04.pid,IsRecordValid=true }).ToList();
                        break;
                    case j04RelationFlagEnum.A05:
                        lis = _mother.j02PersonBL.GetList(new BO.myQueryJ02() {a05id=rec.a05ID, j04id = recJ04.pid, IsRecordValid = true }).ToList();
                        break;
                    case j04RelationFlagEnum.A03:
                        var lisA39 = _mother.a39InstitutionPersonBL.GetList(new BO.myQuery("a39") { a03id = rec.a03ID, j04id = recJ04.pid }).ToList();
                        foreach(var c in lisA39)
                        {
                            lis.Add(_mother.j02PersonBL.Load(c.j02ID));
                        }
                        break;                        
                }
                foreach(var c in lis)
                {
                    if (!ret.Exists(p => p.j02ID == c.pid))
                    {
                        ret.Add(c);
                    }
                }
            }
            if (intJ11ID > 0)
            {
                var lis = _mother.j02PersonBL.GetList(new BO.myQueryJ02() { IsRecordValid = true, j11id = intJ11ID });
                foreach (var c in lis)
                {
                    if (!ret.Exists(p => p.j02ID == c.pid))
                    {
                        ret.Add(c);
                    }
                }
            }

            return ret;
        }

        private bool AutoGenerateToDoFromWorkflow(BO.a01Event rec,BO.b06WorkflowStep recB06)
        {
            //krok generuje úkol
            var recH07 = _mother.h07ToDoTypeBL.Load(recB06.b06ToDo_h07ID);
            var recH04 = new BO.h04ToDo() { a01ID = rec.pid, h07ID = recH07.pid, j03ID_Creator = _mother.CurrentUser.pid, j02ID_Owner = rec.j02ID_Issuer, h04Name = recB06.b06ToDo_Name, h04Description=recB06.b06ToDo_Description };
            if (recH07.h07IsToDo)
            {
                recH04.h05ID = 1;
                
            }
            InhaleTodoKeyDatesFromWorkflow(ref recH04, rec, recB06);
           
            var intH04ID=_mother.h04ToDoBL.Save(recH04, CompleteTodoReceivers(rec, recB06).Select(p => p.j02ID).ToList(), null);
            if (intH04ID > 0)
            {
                if (recB06.b06ToDo_IsSendMail)
                {//odeslat notifikaci na úkol
                    _mother.h04ToDoBL.NotifyByMail(intH04ID);
                }
                return true;
            }
            else
            {
                return false;
            }
                
        }
        private List<BO.h06ToDoReceiver> CompleteTodoReceivers(BO.a01Event rec, BO.b06WorkflowStep recB06)
        {//složit tým řešitelů nového úkolu
            var ret = new List<BO.h06ToDoReceiver>();
            var lisA41 = _mother.a41PersonToEventBL.GetList(new BO.myQuery("a41") { a01id = rec.pid });
            switch (recB06.b06ToDo_ReceiverFlag)
            {
                case 1: //vedoucí+členové
                    lisA41 = lisA41.Where(p => p.a45ID == EventRoleENUM.Vedouci || p.a45ID == EventRoleENUM.Ctenar);
                    break;
                case 2: //pouze vedoucí
                    lisA41 = lisA41.Where(p => p.a45ID == EventRoleENUM.Vedouci);
                    break;
                case 4: //pouze přizvané osoby
                    lisA41 = lisA41.Where(p => p.a45ID == EventRoleENUM.PrizvanaOsoba);
                    break;

            }
            foreach(var c in lisA41)
            {
                ret.Add(new BO.h06ToDoReceiver() { j02ID = c.j02ID, h06TodoRole=h06TodoRoleEnum.Resitel });
            }
            return ret;
        }
        private void InhaleTodoKeyDatesFromWorkflow(ref BO.h04ToDo recH04,BO.a01Event rec,BO.b06WorkflowStep recB06)
        {
            if (recB06.b06ToDo_DeadlineField.ToLower() == "a01datefrom")
            {
                recH04.h04Deadline = Convert.ToDateTime(rec.a01DateFrom).AddDays(recB06.b06ToDo_DeadlineMove);
            }
            if (recB06.b06ToDo_DeadlineField.ToLower() == "a01dateuntil")
            {
                recH04.h04Deadline = Convert.ToDateTime(rec.a01DateUntil).AddDays(recB06.b06ToDo_DeadlineMove);
            }

            if (recB06.b06ToDo_CapacityPlanFromField.ToLower() == "a01datefrom")
            {
                recH04.h04CapacityPlanFrom = Convert.ToDateTime(rec.a01DateFrom).AddDays(recB06.b06ToDo_CapacityPlanFromMove);
            }
            if (recB06.b06ToDo_CapacityPlanFromField.ToLower() == "a01dateuntil")
            {
                recH04.h04CapacityPlanFrom = Convert.ToDateTime(rec.a01DateUntil).AddDays(recB06.b06ToDo_CapacityPlanFromMove);
            }

            if (recB06.b06ToDo_CapacityPlanUntilField.ToLower() == "a01datefrom")
            {
                recH04.h04CapacityPlanUntil = Convert.ToDateTime(rec.a01DateFrom).AddDays(recB06.b06ToDo_CapacityPlanUntilMove);
            }
            if (recB06.b06ToDo_CapacityPlanUntilField.ToLower() == "a01dateuntil")
            {
                recH04.h04CapacityPlanUntil = Convert.ToDateTime(rec.a01DateUntil).AddDays(recB06.b06ToDo_CapacityPlanUntilMove);
            }

            if ((recH04.h04CapacityPlanFrom == null || recH04.h04CapacityPlanUntil==null)==false)
            {
                recH04.h04CapacityPlanFrom = OcistiDatumUkoluOVikendy(Convert.ToDateTime(recH04.h04CapacityPlanFrom), recB06.b06ToDo_CapacityPlanFromMove);
                recH04.h04CapacityPlanUntil = OcistiDatumUkoluOVikendy(Convert.ToDateTime(recH04.h04CapacityPlanUntil), recB06.b06ToDo_CapacityPlanUntilMove);

                var d = Convert.ToDateTime(recH04.h04CapacityPlanUntil);
                recH04.h04CapacityPlanUntil = new DateTime(d.Year, d.Month, d.Day).AddDays(1).AddSeconds(-1);
                d = Convert.ToDateTime(recH04.h04CapacityPlanFrom);
                recH04.h04CapacityPlanFrom = new DateTime(d.Year, d.Month, d.Day);

            }
        }

        private DateTime OcistiDatumUkoluOVikendy(DateTime d,int intPosunVuciAkci)
        {   //postarat se o to, aby datum d nespadal do víkendů
            switch (d.DayOfWeek)
            {
                case DayOfWeek.Saturday:
                    if (intPosunVuciAkci > 0)
                    {
                        d = d.AddDays(2);
                    }
                    else
                    {
                        d = d.AddDays(-1);
                    }
                    break;
                case DayOfWeek.Sunday:
                    if (intPosunVuciAkci > 0)
                    {
                        d = d.AddDays(1);
                    }
                    else
                    {
                        d = d.AddDays(-2);
                    }
                    break;
            }
            return d;
        }
    }
}
