using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BL
{
    public interface Ix39ConnectStringBL
    {
        public BO.x39ConnectString Load(int pid);        
        public IEnumerable<BO.x39ConnectString> GetList(BO.myQuery mq);
        public int Save(BO.x39ConnectString rec);

    }
    class x39ConnectStringBL : BaseBL, Ix39ConnectStringBL
    {
        public x39ConnectStringBL(BL.Factory mother) : base(mother)
        {

        }


        private string GetSQL1(string strAppend = null)
        {
            sb("SELECT a.*,");
            sb(_db.GetSQL1_Ocas("x39"));
            sb(" FROM x39ConnectString a");
            sb(strAppend);
            return sbret();
        }
        public BO.x39ConnectString Load(int pid)
        {
            return _db.Load<BO.x39ConnectString>(GetSQL1(" WHERE a.x39ID=@pid"), new { pid = pid });
        }
        

        public IEnumerable<BO.x39ConnectString> GetList(BO.myQuery mq)
        {
            DL.FinalSqlCommand fq = DL.basQuery.GetFinalSql(GetSQL1(), mq, _mother.CurrentUser);
            return _db.GetList<BO.x39ConnectString>(fq.FinalSql, fq.Parameters);
        }



        public int Save(BO.x39ConnectString rec)
        {
            if (ValidateBeforeSave(rec) == false)
            {
                return 0;
            }
            var p = new DL.Params4Dapper();
            p.AddInt("pid", rec.x39ID);
            p.AddString("x39Code", rec.x39Code);
            p.AddString("x39Name", rec.x39Name);
            p.AddString("x39Value", rec.x39Value);
            p.AddString("x39Description", rec.x39Description);

            int intPID = _db.SaveRecord("x39ConnectString", p, rec);



            return intPID;
        }

        public bool ValidateBeforeSave(BO.x39ConnectString rec)
        {
            if (string.IsNullOrEmpty(rec.x39Code))
            {
                this.AddMessage("Chybí vyplnit [Kód]."); return false;
            }
            if (string.IsNullOrEmpty(rec.x39Value))
            {
                this.AddMessage("Chybí vyplnit [Hodnota]."); return false;
            }
            if (string.IsNullOrEmpty(rec.x39Name))
            {
                this.AddMessage("Chybí vyplnit [Název]."); return false;
            }
            var lis = GetList(new BO.myQuery("x39") { IsRecordValid = null });
           if (lis.Where(p => p.x39Code.ToLower() == rec.x39Code.ToLower() && p.pid !=rec.pid).Count()>0)
            {
                this.AddMessage("[Kód] musí být unikátní."); return false;
            }
            return true;
        }

    }
}
