using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BL
{
    public interface Ib06WorkflowStepBL
    {
        public BO.b06WorkflowStep Load(int pid);
        public IEnumerable<BO.b06WorkflowStep> GetList(BO.myQuery mq);
        public IEnumerable<BO.b11WorkflowMessageToStep> GetListB11(int b06id);
        public IEnumerable<BO.b08WorkflowReceiverToStep> GetListB08(int b06id);
        public IEnumerable<BO.b13WorkflowRequiredFormsToStep> GetListB13(int b06id);
        public IEnumerable<BO.b10WorkflowCommandCatalog_Binding> GetListB10(int b06id);
        public int Save(BO.b06WorkflowStep rec, List<BO.b08WorkflowReceiverToStep> lisB08, List<BO.b11WorkflowMessageToStep> lisB11, List<BO.b13WorkflowRequiredFormsToStep> lisB13, List<int> o13ids, List<int> b12_j04ids, List<int> b12_a45ids, List<BO.b10WorkflowCommandCatalog_Binding> lisB10);

    }
    class b06WorkflowStepBL : BaseBL, Ib06WorkflowStepBL
    {
        public b06WorkflowStepBL(BL.Factory mother) : base(mother)
        {

        }


        private string GetSQL1(string strAppend = null)
        {
            sb("SELECT a.*,b02.b02Name,b01.b01Name,b01.b01ID,b02target.b02Name as TargetStatus,h07.h07Name,");
            sb(_db.GetSQL1_Ocas("b06"));
            sb(" FROM b06WorkflowStep a INNER JOIN b02WorkflowStatus b02 ON a.b02ID=b02.b02ID INNER JOIN b01WorkflowTemplate b01 ON b02.b01ID=b01.b01ID");
            sb(" LEFT OUTER JOIN b02WorkflowStatus b02target ON a.b02ID_Target=b02target.b02ID");
            sb(" LEFT OUTER JOIN h07ToDoType h07 ON a.b06ToDo_h07ID=h07.h07ID");
            sb(strAppend);
            return sbret();
        }
        public BO.b06WorkflowStep Load(int pid)
        {
            return _db.Load<BO.b06WorkflowStep>(GetSQL1(" WHERE a.b06ID=@pid"), new { pid = pid });
        }


        public IEnumerable<BO.b06WorkflowStep> GetList(BO.myQuery mq)
        {
            if (mq.explicit_orderby == null) { mq.explicit_orderby = "a.b02ID,a.b06Order"; };
            DL.FinalSqlCommand fq = DL.basQuery.ParseFinalSql(GetSQL1(), mq, _mother.CurrentUser);
            return _db.GetList<BO.b06WorkflowStep>(fq.FinalSql, fq.Parameters);
        }



        public int Save(BO.b06WorkflowStep rec, List<BO.b08WorkflowReceiverToStep> lisB08, List<BO.b11WorkflowMessageToStep> lisB11, List<BO.b13WorkflowRequiredFormsToStep> lisB13, List<int> o13ids, List<int> b12_j04ids, List<int> b12_a45ids, List<BO.b10WorkflowCommandCatalog_Binding> lisB10)
        {
            if (ValidateBeforeSave(ref rec, lisB08) == false)
            {
                return 0;
            }
            var p = new DL.Params4Dapper();
            p.AddInt("pid", rec.b06ID);
            p.AddInt("b02ID", rec.b02ID, true);
            p.AddInt("b02ID_Target", rec.b02ID_Target, true);
            p.AddInt("a45ID_NomineeTarget", rec.a45ID_NomineeTarget, true);
            p.AddInt("x26ID_Nominee_J02", rec.x26ID_Nominee_J02, true);
            p.AddInt("x26ID_Nominee_J11", rec.x26ID_Nominee_J11, true);

            p.AddString("b06Name", rec.b06Name);
            p.AddString("b06RunSQL", rec.b06RunSQL);
            p.AddString("b06ValidateBeforeRunSQL", rec.b06ValidateBeforeRunSQL);
            p.AddString("b06ValidateBeforeErrorMessage", rec.b06ValidateBeforeErrorMessage);

            p.AddInt("b06Order", rec.b06Order);
            p.AddString("b06ValidateAutoMoveSQL", rec.b06ValidateAutoMoveSQL);

            p.AddBool("b06IsCommentRequired", rec.b06IsCommentRequired);
            p.AddBool("b06IsEscalation_Timeout_Total", rec.b06IsEscalation_Timeout_Total);
            p.AddBool("b06IsEscalation_Timeout_SLA", rec.b06IsEscalation_Timeout_SLA);
            p.AddBool("b06IsManualStep", rec.b06IsManualStep);
            p.AddBool("b06IsNominee", rec.b06IsNominee);
            p.AddBool("b06IsNomineeRequired", rec.b06IsNomineeRequired);
            p.AddBool("b06IsHistoryAllowedToAll", rec.b06IsHistoryAllowedToAll);

            p.AddBool("b06IsFormAutoLock", rec.b06IsFormAutoLock);
            p.AddBool("b06IsFormValidationRequired", rec.b06IsFormValidationRequired);
            p.AddBool("b06IsAttachmentTestRequired", rec.b06IsAttachmentTestRequired);

            p.AddBool("b06IsAutoRun_Missing_Form", rec.b06IsAutoRun_Missing_Form);
            p.AddBool("b06IsAutoRun_Missing_Attachment", rec.b06IsAutoRun_Missing_Attachment);
            p.AddBool("b06IsRunOneInstanceOnly", rec.b06IsRunOneInstanceOnly);

            p.AddBool("b06IsToDoGeneration", rec.b06IsToDoGeneration);      //generování úkolu
            p.AddString("b06ToDo_Name", rec.b06ToDo_Name);
            p.AddString("b06ToDo_DeadlineField", rec.b06ToDo_DeadlineField);

            p.AddInt("b06ToDo_DeadlineMove", rec.b06ToDo_DeadlineMove);
            p.AddInt("b06ToDo_h07ID", rec.b06ToDo_h07ID,true);
            p.AddInt("b06ToDo_ReceiverFlag", rec.b06ToDo_ReceiverFlag);
            p.AddString("b06ToDo_Description", rec.b06ToDo_Description);
            p.AddString("b06ToDo_CapacityPlanFromField", rec.b06ToDo_CapacityPlanFromField);
            p.AddInt("b06ToDo_CapacityPlanFromMove", rec.b06ToDo_CapacityPlanFromMove);
            p.AddString("b06ToDo_CapacityPlanUntilField", rec.b06ToDo_CapacityPlanUntilField);
            p.AddInt("b06ToDo_CapacityPlanUntilMove", rec.b06ToDo_CapacityPlanUntilMove);
            p.AddBool("b06ToDo_IsSendMail", rec.b06ToDo_IsSendMail);

            if (rec.b06UC == null) { rec.b06UC = BO.BAS.GetGuid(); }
            p.AddString("b06UC", rec.b06UC);

            int intPID = _db.SaveRecord("b06WorkflowStep", p.getDynamicDapperPars(), rec);
            if (lisB08 != null)
            {
                if (rec.pid > 0)
                {
                    _db.RunSql("DELETE FROM b08WorkflowReceiverToStep WHERE b06ID=@pid", new { pid = intPID });
                }
                foreach (var c in lisB08)
                {
                    _db.RunSql("INSERT INTO b08WorkflowReceiverToStep(b06ID,a45ID,j11ID,j04ID,b06ID_NomineeSource) VALUES (@pid,@a45id,@j11id,@j04id,@b06id_nomineesource)", new { pid = intPID, a45id = BO.BAS.TestIntAsDbKey(c.a45ID), j11id = BO.BAS.TestIntAsDbKey(c.j11ID), j04id = BO.BAS.TestIntAsDbKey(c.j04ID), b06id_nomineesource = BO.BAS.TestIntAsDbKey(c.b06ID_NomineeSource) });
                }
            }
            if (lisB11 != null)
            {
                if (rec.pid > 0)
                {
                    _db.RunSql("DELETE FROM b11WorkflowMessageToStep WHERE b06ID=@pid", new { pid = intPID });
                }
                foreach (var c in lisB11)
                {
                    _db.RunSql("INSERT INTO b11WorkflowMessageToStep(b06ID,b65ID,a45ID,j04ID,j11ID) VALUES (@pid,@b65id,@a45id,@j04id,@j11id)", new { pid = intPID, b65id =BO.BAS.TestIntAsDbKey(c.b65ID), a45id = BO.BAS.TestIntAsDbKey(c.a45ID), j04id = BO.BAS.TestIntAsDbKey(c.j04ID), j11id = BO.BAS.TestIntAsDbKey(c.j11ID) });
                }
            }

            if (lisB13 != null)
            {
                if (rec.pid > 0)
                {
                    _db.RunSql("DELETE FROM b13WorkflowRequiredFormsToStep WHERE b06ID=@pid", new { pid = intPID });
                }
                if (lisB13.Count > 0)
                {
                    _db.RunSql("INSERT INTO b13WorkflowRequiredFormsToStep(b06ID,f06ID) SELECT @pid,f06ID FROM f06Form WHERE f06ID IN (" + string.Join(",", lisB13.Select(p=>p.f06ID)) + ")", new { pid = intPID });
                }
            }
            if (o13ids != null)
            {
                if (rec.pid > 0)
                {
                    _db.RunSql("DELETE FROM b14WorkflowRequiredAttachmentTypeToStep WHERE b06ID=@pid", new { pid = intPID });
                }
                if (o13ids.Count > 0)
                {
                    _db.RunSql("INSERT INTO b14WorkflowRequiredAttachmentTypeToStep(b06ID,o13ID) SELECT @pid,o13ID FROM o13AttachmentType WHERE o13ID IN (" + string.Join(",", o13ids) + ")", new { pid = intPID });
                }
            }
            if (b12_j04ids != null || b12_a45ids !=null)
            {
                if (rec.pid > 0)
                {
                    _db.RunSql("DELETE FROM b12WorkflowReceiverToHistory WHERE b06ID=@pid", new { pid = intPID });
                }
                if (b12_j04ids.Count > 0)
                {
                    _db.RunSql("INSERT INTO b12WorkflowReceiverToHistory(b06ID,j04ID) SELECT @pid,j04ID FROM j04UserRole WHERE j04ID IN (" + string.Join(",", b12_j04ids) + ")", new { pid = intPID });
                }
                if (b12_a45ids.Count > 0)
                {
                    _db.RunSql("INSERT INTO b12WorkflowReceiverToHistory(b06ID,a45ID) SELECT @pid,a45ID FROM a45EventRole WHERE a45ID IN (" + string.Join(",", b12_a45ids) + ")", new { pid = intPID });
                }
            }
            if (lisB10 != null)
            {
                if (rec.pid > 0)
                {
                    _db.RunSql("DELETE FROM b10WorkflowCommandCatalog_Binding WHERE b06ID=@pid", new { pid = intPID });
                }
                foreach (var c in lisB10)
                {
                    _db.RunSql("INSERT INTO b10WorkflowCommandCatalog_Binding(b06ID,b09ID,b10Parameter1) VALUES (@pid,@b09id,@par1)", new { pid = intPID, b09id = c.b09ID, par1 = c.b10Parameter1 });
                }
            }

            return intPID;
        }

        public bool ValidateBeforeSave(ref BO.b06WorkflowStep rec, List<BO.b08WorkflowReceiverToStep> lisB08)
        {            
            if (rec.b02ID == 0)
            {
                this.AddMessage("Chybí vazba na workflow stav."); return false;
            }
            if (rec.b02ID_Target==0 && string.IsNullOrEmpty(rec.b06Name))
            {
                this.AddMessage("Pokud se nemění cílový stav, název kroku je povinný."); return false;
            }

            if (rec.b06IsManualStep)
            {
                if (lisB08.Count() == 0)
                {
                    this.AddMessage("V nastavení kroku chybí určení, kdo provádí krok."); return false;
                }
                if (lisB08.Where(p=>p.a45ID==99 && p.j11ID==0).Count()>0)
                {
                    this.AddMessage("Není vybrán pojmenovaný seznam u přímo dosazeného příjemce kroku."); return false;
                }
                if (lisB08.Where(p => p.a45ID == 98 && p.b06ID_NomineeSource == 0).Count() > 0)
                {
                    this.AddMessage("Není vybrán zdrojový krok, v kterém proběhla nominace."); return false;
                }
            }
            if (rec.b06IsToDoGeneration)
            {
                if (string.IsNullOrEmpty(rec.b06ToDo_Name) == true)
                {
                    this.AddMessage("Chybí specifikace názvu (předmětu) úkolu.");return false;
                }
                if (string.IsNullOrEmpty(rec.b06ToDo_DeadlineField) == true)
                {
                    this.AddMessage("Chybí specifikace termínu úkolu."); return false;
                }
                if (rec.b06ToDo_h07ID==0)
                {
                    this.AddMessage("Chybí specifikace typu úkolu (lhůty)."); return false;
                }
            }
            if (rec.b06IsNominee)
            {
                if (rec.a45ID_NomineeTarget == 0)
                {
                    this.AddMessage("Musíte vyplnit volbu [Nominovaný obdrží v akci roli], protože je zaškrtnuto [V tomto kroku lze provést nominaci]."); return false;
                }
                if (rec.x26ID_Nominee_J02 == 0 && rec.x26ID_Nominee_J11==0)
                {
                    this.AddMessage("Vyberte zdroj nominovaných osob nebo zdroj seznamů osob."); return false;
                }
            }
            else
            {
                rec.b06IsNomineeRequired = false; rec.a45ID_NomineeTarget = 0;rec.x26ID_Nominee_J02 = 0;rec.x26ID_Nominee_J11 = 0;
            }


            return true;
        }

        public IEnumerable<BO.b08WorkflowReceiverToStep> GetListB08(int b06id)
        {
            sb("SELECT a.*,a45.a45Name,j11.j11Name,j04.j04Name,");
            sb(_db.GetSQL1_Ocas("b08", false, false, false));
            sb(" FROM b08WorkflowReceiverToStep a");
            sb(" LEFT OUTER JOIN a45EventRole a45 ON a.a45ID=a45.a45ID LEFT OUTER JOIN j11Team j11 ON a.j11ID=j11.j11ID LEFT OUTER JOIN j04UserRole j04 ON a.j04ID=j04.j04ID");
            sb(" WHERE a.b06ID=@b06id");

            return _db.GetList<BO.b08WorkflowReceiverToStep>(sbret(), new { b06id = b06id });
        }
        public IEnumerable<BO.b11WorkflowMessageToStep> GetListB11(int b06id)
        {
            sb("SELECT a.*,b65.b65Name,a45.a45Name,j04.j04Name,j11.j11Name,");
            sb(_db.GetSQL1_Ocas("b11", false, false, false));
            sb(" FROM b11WorkflowMessageToStep a INNER JOIN b65WorkflowMessage b65 ON a.b65ID=b65.b65ID");
            sb(" LEFT OUTER JOIN a45EventRole a45 ON a.a45ID=a45.a45ID LEFT OUTER JOIN j04UserRole j04 ON a.j04ID=j04.j04ID LEFT OUTER JOIN j11Team j11 ON a.j11ID=j11.j11ID");
            sb(" WHERE a.b06ID=@b06id");

            return _db.GetList<BO.b11WorkflowMessageToStep>(sbret(), new { b06id = b06id });
        }
        public IEnumerable<BO.b13WorkflowRequiredFormsToStep> GetListB13(int b06id)
        {
            sb("SELECT a.*,f06.f06Name,");
            sb(_db.GetSQL1_Ocas("b13", false, false, false));
            sb(" FROM b13WorkflowRequiredFormsToStep a INNER JOIN f06Form f06 ON a.f06ID=f06.f06ID");            
            sb(" WHERE a.b06ID=@b06id");
            return _db.GetList<BO.b13WorkflowRequiredFormsToStep>(sbret(), new { b06id = b06id });
        }
        public IEnumerable<BO.b10WorkflowCommandCatalog_Binding> GetListB10(int b06id)
        {
            sb("SELECT a.*,b09.b09Name,b09.b09ParametersCount,");
            sb(_db.GetSQL1_Ocas("b10", false, false, false));
            sb(" FROM b10WorkflowCommandCatalog_Binding a");
            sb(" INNER JOIN b09WorkflowCommandCatalog b09 ON a.b09ID=b09.b09ID");
            sb(" WHERE a.b06ID=@b06id");

            return _db.GetList<BO.b10WorkflowCommandCatalog_Binding>(sbret(), new { b06id = b06id });
        }

    }
}
