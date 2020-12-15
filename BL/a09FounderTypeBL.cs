using System;
using System.Collections.Generic;
using System.Text;

namespace BL
{
    public interface Ia09FounderTypeBL
    {
        public BO.a09FounderType Load(int pid);
        public IEnumerable<BO.a09FounderType> GetList(BO.myQuery mq);
        public int Save(BO.a09FounderType rec);

    }
    class a09FounderTypeBL : BaseBL, Ia09FounderTypeBL
    {
        public a09FounderTypeBL(BL.Factory mother) : base(mother)
        {

        }


        private string GetSQL1(string strAppend = null)
        {
            sb("SELECT a.*,");
            sb(_db.GetSQL1_Ocas("a09"));
            sb(" FROM a09FounderType a");
            sb(strAppend);
            return sbret();
        }
        public BO.a09FounderType Load(int pid)
        {
            return _db.Load<BO.a09FounderType>(GetSQL1(" WHERE a.a09ID=@pid"), new { pid = pid });
        }

        public IEnumerable<BO.a09FounderType> GetList(BO.myQuery mq)
        {
            DL.FinalSqlCommand fq = DL.basQuery.GetFinalSql(GetSQL1(), mq, _mother.CurrentUser);
            return _db.GetList<BO.a09FounderType>(fq.FinalSql, fq.Parameters);
        }



        public int Save(BO.a09FounderType rec)
        {
            if (ValidateBeforeSave(rec) == false)
            {
                return 0;
            }
            var p = new DL.Params4Dapper();
            p.AddInt("pid", rec.a09ID);
            p.AddString("a09Name", rec.a09Name);
            p.AddString("a09UIVCode", rec.a09UIVCode);
            p.AddInt("a09Ordinal", rec.a09Ordinal);
            p.AddString("a09Description", rec.a09Description);
            p.AddInt("a09Ordinal", rec.a09Ordinal);


            int intPID = _db.SaveRecord("a09FounderType", p.getDynamicDapperPars(), rec);



            return intPID;
        }

        public bool ValidateBeforeSave(BO.a09FounderType rec)
        {
            if (string.IsNullOrEmpty(rec.a09Name))
            {
                this.AddMessage("Chybí vyplnit [Název]."); return false;
            }


            return true;
        }

    }
}
