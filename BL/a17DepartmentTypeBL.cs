using System;
using System.Collections.Generic;
using System.Text;

namespace BL
{
    public interface Ia17DepartmentTypeBL
    {
        public BO.a17DepartmentType Load(int pid);
        public IEnumerable<BO.a17DepartmentType> GetList(BO.myQuery mq);
        public int Save(BO.a17DepartmentType rec);

    }
    class a17DepartmentTypeBL : BaseBL, Ia17DepartmentTypeBL
    {
        public a17DepartmentTypeBL(BL.Factory mother) : base(mother)
        {

        }


        private string GetSQL1(string strAppend = null)
        {
            sb("SELECT a.*,a17UIVCode+' - '+a17UIVCode as CodePlusName,");
            sb(_db.GetSQL1_Ocas("a17"));            
            sb(" FROM a17DepartmentType a");            
            sb(strAppend);            
            return sbret();
        }
        public BO.a17DepartmentType Load(int pid)
        {
            return _db.Load<BO.a17DepartmentType>(GetSQL1(" WHERE a.a17ID=@pid"), new { pid = pid });
        }

        public IEnumerable<BO.a17DepartmentType> GetList(BO.myQuery mq)
        {
            if (mq.explicit_orderby == null) { mq.explicit_orderby = "a.a17Name"; };
            DL.FinalSqlCommand fq = DL.basQuery.ParseFinalSql(GetSQL1(), mq, _mother.CurrentUser);
            return _db.GetList<BO.a17DepartmentType>(fq.FinalSql, fq.Parameters);
        }



        public int Save(BO.a17DepartmentType rec)
        {
            if (ValidateBeforeSave(rec) == false)
            {
                return 0;
            }
            var p = new DL.Params4Dapper();
            p.AddInt("pid", rec.a17ID);            
            p.AddString("a17Name", rec.a17Name);
            p.AddString("a17UIVCode", rec.a17UIVCode);
            p.AddString("a17Description", rec.a17Description);           
            p.AddBool("a17IsDefault", rec.a17IsDefault);

            int intPID = _db.SaveRecord("a17DepartmentType", p.getDynamicDapperPars(), rec);



            return intPID;
        }

        public bool ValidateBeforeSave(BO.a17DepartmentType rec)
        {
            if (string.IsNullOrEmpty(rec.a17Name))
            {
                this.AddMessage("Chybí vyplnit [Název]."); return false;
            }
            if (string.IsNullOrEmpty(rec.a17UIVCode))
            {
                this.AddMessage("Chybí vyplnit [Kód]."); return false;
            }


            return true;
        }

    }
}
