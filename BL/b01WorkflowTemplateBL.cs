using System;
using System.Collections.Generic;
using System.Text;

namespace BL
{
    public interface Ib01WorkflowTemplateBL
    {
        public BO.b01WorkflowTemplate Load(int pid);        
        public IEnumerable<BO.b01WorkflowTemplate> GetList(BO.myQuery mq);
        public int Save(BO.b01WorkflowTemplate rec);

    }
    class b01WorkflowTemplateBL : BaseBL, Ib01WorkflowTemplateBL
    {
        public b01WorkflowTemplateBL(BL.Factory mother) : base(mother)
        {

        }


        private string GetSQL1(string strAppend = null)
        {
            sb("SELECT a.*,");
            sb(_db.GetSQL1_Ocas("b01"));
            sb(" FROM b01WorkflowTemplate a");
            sb(strAppend);
            return sbret();
        }
        public BO.b01WorkflowTemplate Load(int pid)
        {
            return _db.Load<BO.b01WorkflowTemplate>(GetSQL1(" WHERE a.b01ID=@pid"), new { pid = pid });
        }
       

        public IEnumerable<BO.b01WorkflowTemplate> GetList(BO.myQuery mq)
        {
            if (mq.explicit_orderby == null) { mq.explicit_orderby = "a.b01Name"; };
            DL.FinalSqlCommand fq = DL.basQuery.GetFinalSql(GetSQL1(), mq, _mother.CurrentUser);
            return _db.GetList<BO.b01WorkflowTemplate>(fq.FinalSql, fq.Parameters);
        }



        public int Save(BO.b01WorkflowTemplate rec)
        {
            if (ValidateBeforeSave(rec) == false)
            {
                return 0;
            }
            var p = new DL.Params4Dapper();
            p.AddInt("pid", rec.b01ID);            
            p.AddString("b01Name", rec.b01Name);
            p.AddString("b01Ident", rec.b01Ident);
            if (rec.b01UC == null) { rec.b01UC = BO.BAS.GetGuid(); }
            p.AddString("b01UC", rec.b01UC);

            int intPID = _db.SaveRecord("b01WorkflowTemplate", p.getDynamicDapperPars(), rec);

            return intPID;
        }

        public bool ValidateBeforeSave(BO.b01WorkflowTemplate rec)
        {
            if (string.IsNullOrEmpty(rec.b01Name))
            {
                this.AddMessage("Chybí vyplnit [Název]."); return false;
            }
           

         


            return true;
        }

    }
}
