using System;
using System.Collections.Generic;
using System.Text;

namespace BL
{
    public interface Ia05RegionBL
    {
        public BO.a05Region Load(int pid);
        public IEnumerable<BO.a05Region> GetList(BO.myQuery mq);
        public int Save(BO.a05Region rec);

    }
    class a05RegionBL : BaseBL, Ia05RegionBL
    {
        public a05RegionBL(BL.Factory mother) : base(mother)
        {

        }


        private string GetSQL1(string strAppend = null)
        {
            sb("SELECT a.*,");
            sb(_db.GetSQL1_Ocas("a05"));            
            sb(" FROM a05Region a");            
            sb(strAppend);
            return sbret();
        }
        public BO.a05Region Load(int pid)
        {
            return _db.Load<BO.a05Region>(GetSQL1(" WHERE a.a05ID=@pid"), new { pid = pid });
        }

        public IEnumerable<BO.a05Region> GetList(BO.myQuery mq)
        {
            DL.FinalSqlCommand fq = DL.basQuery.ParseFinalSql(GetSQL1(), mq, _mother.CurrentUser);
            return _db.GetList<BO.a05Region>(fq.FinalSql, fq.Parameters);
        }



        public int Save(BO.a05Region rec)
        {
            if (ValidateBeforeSave(rec) == false)
            {
                return 0;
            }
            var p = new DL.Params4Dapper();
            p.AddInt("pid", rec.a05ID);            
            p.AddString("a05Name", rec.a05Name);
            p.AddString("a05UIVCode", rec.a05UIVCode);
            p.AddString("a05RZCode", rec.a05RZCode);
            p.AddString("a05VUSCCode", rec.a05VUSCCode);
            p.AddInt("a05Ordinal", rec.a05Ordinal);
           

            int intPID = _db.SaveRecord("a05Region", p.getDynamicDapperPars(), rec);



            return intPID;
        }

        public bool ValidateBeforeSave(BO.a05Region rec)
        {
            if (string.IsNullOrEmpty(rec.a05Name))
            {
                this.AddMessage("Chybí vyplnit [Název]."); return false;
            }
           

            return true;
        }

    }
}
