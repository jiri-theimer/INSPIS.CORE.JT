using System;
using System.Collections.Generic;
using System.Text;

namespace BL
{
    public interface Ia02InspectorBL
    {
        public BO.a02Inspector Load(int pid);
        public IEnumerable<BO.a02Inspector> GetList(BO.myQuery mq);
        public int Save(BO.a02Inspector rec);

    }
    class a02InspectorBL : BaseBL, Ia02InspectorBL
    {
        public a02InspectorBL(BL.Factory mother) : base(mother)
        {

        }


        private string GetSQL1(string strAppend = null)
        {
            sb("SELECT a.*,");
            sb("dbo._core_j02_fullname_asc(j02.j02TitleBeforeName,j02.j02FirstName,j02.j02LastName,j02.j02TitleAfterName) as Person,");
            sb("isnull(a04.a04Street,'')+isnull(', '+a04.a04PostCode,'')+isnull(' '+a04.a04City,'') as PostAddress,");
            sb("a05.a05Name,");
            sb(_db.GetSQL1_Ocas("a02"));
            sb(" FROM a02Inspector a INNER JOIN j02Person j02 ON a.j02ID=j02.j02ID INNER JOIN a04Inspectorate a04 ON a.a04ID=a04.a04ID INNER JOIN a05Region a05 ON a04.a05ID=a05.a05ID");
            sb(strAppend);
            return sbret();
        }
        public BO.a02Inspector Load(int pid)
        {
            return _db.Load<BO.a02Inspector>(GetSQL1(" WHERE a.a02ID=@pid"), new { pid = pid });
        }

        public IEnumerable<BO.a02Inspector> GetList(BO.myQuery mq)
        {
            DL.FinalSqlCommand fq = DL.basQuery.GetFinalSql(GetSQL1(), mq, _mother.CurrentUser);
            return _db.GetList<BO.a02Inspector>(fq.FinalSql, fq.Parameters);
        }



        public int Save(BO.a02Inspector rec)
        {
            if (ValidateBeforeSave(rec) == false)
            {
                return 0;
            }
            var p = new DL.Params4Dapper();
            p.AddInt("pid", rec.a02ID);
            p.AddInt("j02ID", rec.j02ID);
            p.AddInt("a04ID", rec.a04ID);

            int intPID = _db.SaveRecord("a02Inspector", p, rec);
            if (intPID > 0)
            {
                _mother.j03UserBL.RecoveryUserCache(0,rec.j02ID);
            }


            return intPID;
        }

        public bool ValidateBeforeSave(BO.a02Inspector rec)
        {
            if (rec.a04ID==0 || rec.j02ID==0)
            {
                this.AddMessage("[Jméno] a [Inspektorát] jsou povinná pole k vyplnění."); return false;
            }


            return true;
        }

    }
}
