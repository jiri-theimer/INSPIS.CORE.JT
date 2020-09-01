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
        public bool RunWorkflowStep(BO.a01Event rec, BO.b06WorkflowStep recB06, List<BO.a41PersonToEvent> lisNominee, string strComment, string strUploadGUID, bool bolManualStep);
    }
    class WorkflowBL: BaseBL,IWorkflowBL
    {
        public WorkflowBL(BL.Factory mother) : base(mother)
        {

        }

        public bool RunWorkflowStep(BO.a01Event rec, BO.b06WorkflowStep recB06, List<BO.a41PersonToEvent> lisNominee, string strComment, string strUploadGUID, bool bolManualStep)
        {
            if (ValidateWorkflowStepBeforeRun(rec,recB06,lisNominee,strComment, strUploadGUID, bolManualStep)==false){
                return false;
            }


            return true;
        }

        private bool ValidateWorkflowStepBeforeRun(BO.a01Event rec, BO.b06WorkflowStep recB06, List<BO.a41PersonToEvent> lisNominee, string strComment, string strUploadGUID, bool bolManualStep)
        {
            if (recB06.pid == 0)
            {
                this.AddMessage("Musíte zvolit z nabídky konkrétní krok!");return false;
            }
            if (recB06.b06IsCommentRequired && string.IsNullOrEmpty(strComment)==true)
            {
                this.AddMessage(string.Format("Krok {0} vyžaduje zapsat komentář!",recB06.b06Name)); return false;
            }
            if (recB06.b06IsNomineeRequired && (lisNominee==null || lisNominee.Count()==0))
            {
                this.AddMessage(string.Format("Krok {0} vyžaduje nominaci!", recB06.b06Name)); return false;
            }
            if (lisNominee != null)
            {
                if (lisNominee.Where(p => p.b06ID_NomineeSource == 0).Count() > 0)
                {
                    this.AddMessage("V řádku předané nominace chybí identifikace kroku.");return false;
                }
                if (lisNominee.Where(p => p.a45ID == 0).Count() > 0)
                {
                    this.AddMessage("V řádku předané nominace chybí identifikace role v akci."); return false;
                }
                if (lisNominee.Where(p => p.j02ID == 0 && p.j11ID==0).Count() > 0)
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
                    this.AddMessage(recB06.b06ValidateBeforeErrorMessage);return false;
                }                                
            }
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
                foreach(var cB13 in lisB13)
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

            if (lisA11_TestForms.Count() > 0)
            {
                //existují formuláře ke kontrole
                foreach (var c11 in lisA11_TestForms)
                {

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
    }
}
