using System;
using System.Collections.Generic;
using System.Text;

namespace BL
{
    public interface If22ReplySetBL
    {
        public BO.f22ReplySet Load(int pid);
        public IEnumerable<BO.f22ReplySet> GetList(BO.myQuery mq);
        public int Save(BO.f22ReplySet rec, List<int> f21ids);


    }
    class f22ReplySetBL : BaseBL, If22ReplySetBL
    {
        public f22ReplySetBL(BL.Factory mother) : base(mother)
        {

        }


        private string GetSQL1(string strAppend = null)
        {
            sb("SELECT a.*,");
            sb(_db.GetSQL1_Ocas("f22"));
            sb(" FROM f22ReplySet a");
            sb(strAppend);

            return sbret();
        }
        public BO.f22ReplySet Load(int pid)
        {
            return _db.Load<BO.f22ReplySet>(GetSQL1(" WHERE a.f22ID=@pid"), new { pid = pid });
        }

        public IEnumerable<BO.f22ReplySet> GetList(BO.myQuery mq)
        {

            DL.FinalSqlCommand fq = DL.basQuery.GetFinalSql(GetSQL1(), mq, _mother.CurrentUser);
            return _db.GetList<BO.f22ReplySet>(fq.FinalSql, fq.Parameters);
        }



        public int Save(BO.f22ReplySet rec,List<int>f21ids)
        {
            if (!ValidateBeforeSave(rec))
            {
                return 0;
            }
            var p = new DL.Params4Dapper();
            p.AddInt("pid", rec.f22ID);
            p.AddString("f22Name", rec.f22Name);            
            p.AddInt("f22Ordinal", rec.f22Ordinal);
            p.AddString("f22Description", rec.f22Description);                                    
            if (rec.f22UC == null) { rec.f22UC = BO.BAS.GetGuid(); }
            p.AddString("f22UC", rec.f22UC);


            int intPID = _db.SaveRecord("f22ReplySet", p.getDynamicDapperPars(), rec);
            if (rec.pid > 0)
            {
                _db.RunSql("DELETE FROM f43ReplyUnitToSet WHERE f22ID=@pid", new { pid = intPID });
            }
            if (f21ids.Count > 0)
            {
                _db.RunSql("INSERT INTO f43ReplyUnitToSet(f22ID,f21ID) SELECT @pid,f21ID FROM f21ReplyUnit WHERE f21ID IN (" + string.Join(",", f21ids) + ")", new { pid = intPID });
            }

            return intPID;
        }

        public bool ValidateBeforeSave(BO.f22ReplySet rec)
        {
            if (string.IsNullOrEmpty(rec.f22Name))
            {
                this.AddMessage("Chybí vyplnit [Název]."); return false;
            }

            return true;
        }

    }
}
