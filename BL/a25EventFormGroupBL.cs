using System;
using System.Collections.Generic;
using System.Text;

namespace BL
{
    public interface Ia25EventFormGroupBL
    {
        public BO.a25EventFormGroup Load(int pid);
        public IEnumerable<BO.a25EventFormGroup> GetList(BO.myQuery mq);
        public int Save(BO.a25EventFormGroup rec);

    }
    class a25EventFormGroupBL : BaseBL, Ia25EventFormGroupBL
    {
        public a25EventFormGroupBL(BL.Factory mother) : base(mother)
        {

        }


        private string GetSQL1(string strAppend = null)
        {
            sb("SELECT a.*,");
            sb(_db.GetSQL1_Ocas("a25"));
            sb(" FROM a25EventFormGroup a");
            sb(strAppend);
            return sbret();
        }
        public BO.a25EventFormGroup Load(int pid)
        {
            return _db.Load<BO.a25EventFormGroup>(GetSQL1(" WHERE a.a25ID=@pid"), new { pid = pid });
        }

        public IEnumerable<BO.a25EventFormGroup> GetList(BO.myQuery mq)
        {
            DL.FinalSqlCommand fq = DL.basQuery.GetFinalSql(GetSQL1(), mq, _mother.CurrentUser);
            return _db.GetList<BO.a25EventFormGroup>(fq.FinalSql, fq.Parameters);
        }



        public int Save(BO.a25EventFormGroup rec)
        {
            if (ValidateBeforeSave(rec) == false)
            {
                return 0;
            }
            var p = new DL.Params4Dapper();
            p.AddInt("pid", rec.a25ID);
            p.AddString("a25Name", rec.a25Name);
            p.AddString("a25Color", rec.a25Color);
          
            int intPID = _db.SaveRecord("a25EventFormGroup", p, rec);



            return intPID;
        }

        public bool ValidateBeforeSave(BO.a25EventFormGroup rec)
        {
            if (string.IsNullOrEmpty(rec.a25Name))
            {
                this.AddMessage("Chybí vyplnit [Název]."); return false;
            }


            return true;
        }

    }
}
