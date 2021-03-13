using System;
using System.Collections.Generic;
using System.Text;

namespace BL
{
    public interface Ij26HolidayBL
    {
        public BO.j26Holiday Load(int pid);
        public IEnumerable<BO.j26Holiday> GetList(BO.myQuery mq);
        public int Save(BO.j26Holiday rec);


    }
    class j26HolidayBL : BaseBL, Ij26HolidayBL
    {
        public j26HolidayBL(BL.Factory mother) : base(mother)
        {

        }


        private string GetSQL1(string strAppend = null)
        {
            sb("SELECT a.*,");
            sb(_db.GetSQL1_Ocas("j26"));
            sb(" FROM j26Holiday a");
            sb(strAppend);
            return sbret();
        }
        public BO.j26Holiday Load(int pid)
        {
            return _db.Load<BO.j26Holiday>(GetSQL1(" WHERE a.j26ID=@pid"), new { pid = pid });
        }


        public IEnumerable<BO.j26Holiday> GetList(BO.myQuery mq)
        {            
            DL.FinalSqlCommand fq = DL.basQuery.GetFinalSql(GetSQL1(), mq, _mother.CurrentUser);
            return _db.GetList<BO.j26Holiday>(fq.FinalSql, fq.Parameters);
        }



        public int Save(BO.j26Holiday rec)
        {

            if (ValidateBeforeSave(rec) == false)
            {
                return 0;
            }

            var p = new DL.Params4Dapper();
            p.AddInt("pid", rec.j26ID);
            p.AddString("j26Name", rec.j26Name);
            p.AddDateTime("j26Date", rec.j26Date);
           
            int intPID = _db.SaveRecord("j26Holiday", p, rec);



            return intPID;
        }

        public bool ValidateBeforeSave(BO.j26Holiday rec)
        {
            if (string.IsNullOrEmpty(rec.j26Name))
            {
                this.AddMessage("Chybí vyplnit [Název]."); return false;
            }
           
            return true;
        }

    }
}
