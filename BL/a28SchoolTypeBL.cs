using System;
using System.Collections.Generic;
using System.Text;

namespace BL
{
    public interface Ia28SchoolTypeBL
    {
        public BO.a28SchoolType Load(int pid);
        public IEnumerable<BO.a28SchoolType> GetList(BO.myQuery mq);
        public int Save(BO.a28SchoolType rec);


    }
    class a28SchoolTypeBL : BaseBL, Ia28SchoolTypeBL
    {
        public a28SchoolTypeBL(BL.Factory mother) : base(mother)
        {

        }


        private string GetSQL1(string strAppend = null)
        {
            sb("SELECT a.*,");
            sb(_db.GetSQL1_Ocas("a28"));
            sb(" FROM a28SchoolType a");
            sb(strAppend);
            return sbret();
        }
        public BO.a28SchoolType Load(int pid)
        {
            return _db.Load<BO.a28SchoolType>(GetSQL1(" WHERE a.a28ID=@pid"), new { pid = pid });
        }


        public IEnumerable<BO.a28SchoolType> GetList(BO.myQuery mq)
        {
            if (mq.explicit_orderby == null) { mq.explicit_orderby = "a.a28Ordinary,a.a28Name"; };
            DL.FinalSqlCommand fq = DL.basQuery.GetFinalSql(GetSQL1(), mq, _mother.CurrentUser);
            return _db.GetList<BO.a28SchoolType>(fq.FinalSql, fq.Parameters);
        }



        public int Save(BO.a28SchoolType rec)
        {

            if (ValidateBeforeSave(rec) == false)
            {
                return 0;
            }

            var p = new DL.Params4Dapper();
            p.AddInt("pid", rec.a28ID);
            p.AddString("a28Name", rec.a28Name);
            p.AddString("a28Code", rec.a28Code);
            p.AddInt("a28Ordinary", rec.a28Ordinary);
           
            int intPID = _db.SaveRecord("a28SchoolType", p.getDynamicDapperPars(), rec);



            return intPID;
        }

        public bool ValidateBeforeSave(BO.a28SchoolType rec)
        {
            if (string.IsNullOrEmpty(rec.a28Name))
            {
                this.AddMessage("Chybí vyplnit [Název]."); return false;
            }
            
           


            return true;
        }

    }
}
