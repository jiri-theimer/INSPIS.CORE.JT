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

        public int SaveWorkflowComment(int intA01ID, string strComment, List<int> a45ids_restrict_to);
        public int RunWorkflowStep(BO.a01Event rec, BO.b06WorkflowStep recB06, List<BO.a41PersonToEvent> lisNominee, string strComment, string strUploadGUID, bool bolManualStep);
        public List<BO.a11EventForm> GetForms4Validation(BO.a01Event rec, BO.b06WorkflowStep recB06);
    }
    class WorkflowBL : BaseBL, IWorkflowBL
    {
        public WorkflowBL(BL.Factory mother) : base(mother)
        {

        }

        public int RunWorkflowStep(BO.a01Event rec, BO.b06WorkflowStep recB06, List<BO.a41PersonToEvent> lisNominee, string strComment, string strUploadGUID, bool bolManualStep)
        {
            if (ValidateWorkflowStepBeforeRun(rec, recB06, lisNominee, strComment, strUploadGUID, bolManualStep) == false)
            {
                return 0;
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
            if (string.IsNullOrEmpty(recB06.b06RunSQL)==false)
            {   //krok má spouštět SQL dotaz
                _db.RunSql(_db.ParseMergeSQL(recB06.b06RunSQL, rec.pid.ToString()));
            }

            if (recB06 != null)
            {
                RunB09Commands(rec, recB06.pid, recB06.b02ID_Target);
            }
            else
            {
                RunB09Commands(rec, 0, 0);
            }
            

            return rec.pid;
        }

        private int SaveStepWithChangeStatus(BO.a01Event rec, BO.b06WorkflowStep recB06, BO.b02WorkflowStatus recB02, string strComment, bool bolManualStep)
        {
            _db.RunSql("UPDATE a01Event SET b02ID=@b02id WHERE a01ID=@pid", new { b02id = recB06.b02ID_Target, pid = rec.pid });
            var slave = _db.Load<BO.GetInteger>("SELECT TOP 1 a01ID as Value FROM a01Event WHERE a01ParentID=@pid", new { pid = rec.pid });
            if (slave != null && slave.Value > 0)
            {
                //aktualizovat statusy u dětí matky
                _db.RunSql("UPDATE a01Event SET b02ID=@b02id WHERE a01ParentID=@pid", new { b02id = recB06.b02ID_Target, pid = rec.pid });
            }
            //zápis do historie workflow
            var c = new BO.b05Workflow_History() { a01ID = rec.pid, b05IsCommentOnly = false, b05IsManualStep = bolManualStep, b05Comment = strComment, b02ID_From = rec.b02ID, b02ID_To = recB06.b02ID_Target };
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
            if (recB06.b06IsRunOneInstanceOnly)
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



            return true;
        }

        public int SaveWorkflowComment(int intA01ID, string strComment, List<int> a45ids_restrict_to)
        {
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

            return intB05ID;
        }


        private void RunB09Commands(BO.a01Event rec,int intB06ID,int intB02ID_Target)
        {
            if (intB06ID > 0)
            {
                //spuštění případných příkazů spojených s krokem
                foreach(var prikaz in _mother.b06WorkflowStepBL.GetListB10(intB06ID).Where(p=>p.b09SQL !=null)){
                    _db.RunSql(_db.ParseMergeSQL(prikaz.b09SQL, rec.pid.ToString(), prikaz.b10Parameter1, prikaz.b10Parameter2));
                    if (rec.a01ChildsCount > 0 && rec.a01ParentID==0)
                    {
                        //spuštění SQL příkazů podřízených dětí vůči matce
                        var mq = new BO.myQuery("a01");
                        mq.a01parentid = rec.pid;
                        var lisChilds = _mother.a01EventBL.GetList(mq);
                        foreach (var c in lisChilds)
                        {
                            _db.RunSql(_db.ParseMergeSQL(prikaz.b09SQL, c.pid.ToString(), prikaz.b10Parameter1, prikaz.b10Parameter2));
                        }
                    }
                }
            }
            if (intB02ID_Target > 0)
            {
                //spuštění případných příkazů spojených s cílovým statusem
                foreach (var prikaz in _mother.b02WorkflowStatusBL.GetListB10(intB02ID_Target).Where(p => p.b09SQL != null))
                {
                    _db.RunSql(_db.ParseMergeSQL(prikaz.b09SQL, rec.pid.ToString(), prikaz.b10Parameter1, prikaz.b10Parameter2));
                    if (rec.a01ChildsCount > 0 && rec.a01ParentID == 0)
                    {
                        //spuštění SQL příkazů podřízených dětí vůči matce
                        var mq = new BO.myQuery("a01");
                        mq.a01parentid = rec.pid;
                        var lisChilds = _mother.a01EventBL.GetList(mq);
                        foreach (var c in lisChilds)
                        {
                            _db.RunSql(_db.ParseMergeSQL(prikaz.b09SQL, c.pid.ToString(), prikaz.b10Parameter1, prikaz.b10Parameter2));
                        }
                    }
                }
            }
        }
    }
}
