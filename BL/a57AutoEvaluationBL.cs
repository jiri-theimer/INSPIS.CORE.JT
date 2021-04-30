using System;
using System.Collections.Generic;
using System.Text;

namespace BL
{
    public interface Ia57AutoEvaluationBL
    {
        public BO.a57AutoEvaluation Load(int pid);
        public IEnumerable<BO.a57AutoEvaluation> GetList(BO.myQuery mq);
        public int Save(BO.a57AutoEvaluation rec);


    }
    class a57AutoEvaluationBL : BaseBL, Ia57AutoEvaluationBL
    {
        public a57AutoEvaluationBL(BL.Factory mother) : base(mother)
        {

        }


        private string GetSQL1(string strAppend = null)
        {
            sb("SELECT a.*,a08.a08Name,a10.a10Name,");
            sb(_db.GetSQL1_Ocas("a57"));
            sb(" FROM a57AutoEvaluation a LEFT OUTER JOIN a08Theme a08 ON a.a08ID=a08.a08ID LEFT OUTER JOIN a10EventType a10 ON a.a10ID=a10.a10ID");
            sb(strAppend);
            return sbret();
        }
        public BO.a57AutoEvaluation Load(int pid)
        {
            return _db.Load<BO.a57AutoEvaluation>(GetSQL1(" WHERE a.a57ID=@pid"), new { pid = pid });
        }


        public IEnumerable<BO.a57AutoEvaluation> GetList(BO.myQuery mq)
        {
            if (mq.explicit_orderby == null) { mq.explicit_orderby = "a.a57Name"; };
            DL.FinalSqlCommand fq = DL.basQuery.GetFinalSql(GetSQL1(), mq, _mother.CurrentUser);
            return _db.GetList<BO.a57AutoEvaluation>(fq.FinalSql, fq.Parameters);
        }



        public int Save(BO.a57AutoEvaluation rec)
        {
            if (ValidateBeforeSave(rec) == false)
            {
                return 0;
            }

            var p = new DL.Params4Dapper();
            p.AddInt("pid", rec.a57ID);
            p.AddString("a57Name", rec.a57Name);
            p.AddInt("a08ID", rec.a08ID,true);
            p.AddInt("a10ID", rec.a10ID, true);            
            p.AddString("a57Description", rec.a57Description);
            p.AddDateTime("a57CreateFrom", rec.a57CreateFrom);
            p.AddDateTime("a57CreateUntil", rec.a57CreateUntil);
            
            int intPID = _db.SaveRecord("a57AutoEvaluation", p, rec);



            return intPID;
        }

        public bool ValidateBeforeSave(BO.a57AutoEvaluation rec)
        {
            if (string.IsNullOrEmpty(rec.a57Name))
            {
                this.AddMessage("Chybí vyplnit [Název]."); return false;
            }
            if (rec.a10ID==0 && rec.a08ID==0)
            {
                this.AddMessage("Vazba na [Typ akce] a [Téma akce] je povinná."); return false;
            }
            
            return true;
        }

    }
}
