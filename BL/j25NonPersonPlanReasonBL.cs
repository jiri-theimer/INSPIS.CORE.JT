using System;
using System.Collections.Generic;

namespace BL
{
    public interface Ij25NonPersonPlanReasonBL
    {
        public BO.j25NonPersonPlanReason Load(int pid);
        public IEnumerable<BO.j25NonPersonPlanReason> GetList(BO.myQuery mq);
        public int Save(BO.j25NonPersonPlanReason rec);

    }
    class j25NonPersonPlanReasonBL : BaseBL, Ij25NonPersonPlanReasonBL
    {
        public j25NonPersonPlanReasonBL(BL.Factory mother) : base(mother)
        {

        }


        private string GetSQL1(string strAppend = null)
        {
            sb("SELECT a.*,");
            sb(_db.GetSQL1_Ocas("j25"));
            sb(" FROM j25NonPersonPlanReason a");
            sb(strAppend);
            return sbret();
        }
        public BO.j25NonPersonPlanReason Load(int pid)
        {
            return _db.Load<BO.j25NonPersonPlanReason>(GetSQL1(" WHERE a.j25ID=@pid"), new { pid = pid });
        }

        public IEnumerable<BO.j25NonPersonPlanReason> GetList(BO.myQuery mq)
        {
            DL.FinalSqlCommand fq = DL.basQuery.GetFinalSql(GetSQL1(), mq, _mother.CurrentUser);
            return _db.GetList<BO.j25NonPersonPlanReason>(fq.FinalSql, fq.Parameters);
        }



        public int Save(BO.j25NonPersonPlanReason rec)
        {
            if (ValidateBeforeSave(rec) == false)
            {
                return 0;
            }
            var p = new DL.Params4Dapper();
            p.AddInt("pid", rec.j25ID);
            p.AddString("j25Name", rec.j25Name);
           
            int intPID = _db.SaveRecord("j25NonPersonPlanReason", p.getDynamicDapperPars(), rec);



            return intPID;
        }

        public bool ValidateBeforeSave(BO.j25NonPersonPlanReason rec)
        {
            if (string.IsNullOrEmpty(rec.j25Name))
            {
                this.AddMessage("Chybí vyplnit [Název]."); return false;
            }



            return true;
        }

    }
}
