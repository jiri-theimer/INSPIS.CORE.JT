using System;
using System.Collections.Generic;

namespace BL
{
    public interface Ij24NonPersonTypeBL
    {
        public BO.j24NonPersonType Load(int pid);
        public IEnumerable<BO.j24NonPersonType> GetList(BO.myQuery mq);
        public int Save(BO.j24NonPersonType rec);

    }
    class j24NonPersonTypeBL : BaseBL, Ij24NonPersonTypeBL
    {
        public j24NonPersonTypeBL(BL.Factory mother) : base(mother)
        {

        }


        private string GetSQL1(string strAppend = null)
        {
            sb("SELECT a.*,");
            sb(_db.GetSQL1_Ocas("j24"));            
            sb(" FROM j24NonPersonType a");            
            sb(strAppend);
            return sbret();
        }
        public BO.j24NonPersonType Load(int pid)
        {
            return _db.Load<BO.j24NonPersonType>(GetSQL1(" WHERE a.j24ID=@pid"), new { pid = pid });
        }

        public IEnumerable<BO.j24NonPersonType> GetList(BO.myQuery mq)
        {
            DL.FinalSqlCommand fq = DL.basQuery.ParseFinalSql(GetSQL1(), mq, _mother.CurrentUser);
            return _db.GetList<BO.j24NonPersonType>(fq.FinalSql, fq.Parameters);
        }



        public int Save(BO.j24NonPersonType rec)
        {
            if (ValidateBeforeSave(rec) == false)
            {
                return 0;
            }
            var p = new DL.Params4Dapper();
            p.AddInt("pid", rec.j24ID);           
            p.AddString("j24Name", rec.j24Name);          
            p.AddBool("j24IsDriver", rec.j24IsDriver);

            int intPID = _db.SaveRecord("j24NonPersonType", p.getDynamicDapperPars(), rec);



            return intPID;
        }

        public bool ValidateBeforeSave(BO.j24NonPersonType rec)
        {
            if (string.IsNullOrEmpty(rec.j24Name))
            {
                this.AddMessage("Chybí vyplnit [Název]."); return false;
            }
         


            return true;
        }

    }
}
