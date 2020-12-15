using System;
using System.Collections.Generic;

namespace BL
{
    public interface Ia04InspectorateBL
    {
        public BO.a04Inspectorate Load(int pid);
        public IEnumerable<BO.a04Inspectorate> GetList(BO.myQuery mq);
        public int Save(BO.a04Inspectorate rec);

    }
    class a04InspectorateBL : BaseBL, Ia04InspectorateBL
    {
        public a04InspectorateBL(BL.Factory mother) : base(mother)
        {

        }


        private string GetSQL1(string strAppend = null)
        {
            sb("SELECT a.*,");
            sb(_db.GetSQL1_Ocas("a04"));
            sb(",a05.a05name");
            sb(" FROM a04Inspectorate a");
            sb(" LEFT OUTER JOIN a05Region a05 ON a.a05id=a05.a05id");            
            sb(strAppend);
            return sbret();
        }
        public BO.a04Inspectorate Load(int pid)
        {
            return _db.Load<BO.a04Inspectorate>(GetSQL1(" WHERE a.a04ID=@pid"), new { pid = pid });
        }

        public IEnumerable<BO.a04Inspectorate> GetList(BO.myQuery mq)
        {
            DL.FinalSqlCommand fq = DL.basQuery.GetFinalSql(GetSQL1(), mq, _mother.CurrentUser);
            return _db.GetList<BO.a04Inspectorate>(fq.FinalSql, fq.Parameters);
        }



        public int Save(BO.a04Inspectorate rec)
        {
            if (ValidateBeforeSave(rec) == false)
            {
                return 0;
            }
            var p = new DL.Params4Dapper();
            p.AddInt("pid", rec.a04ID);
            p.AddInt("a05ID", rec.a05ID, true);
            p.AddString("a04Name", rec.a04Name);
            p.AddString("a04City", rec.a04City);
            p.AddString("a04Street", rec.a04Street);            
            p.AddString("a04PostCode", rec.a04PostCode);
            p.AddString("a04Phone", rec.a04Phone);
            p.AddString("a04Mobile", rec.a04Mobile);
            p.AddString("a04Fax", rec.a04Fax);
            p.AddString("a04Email", rec.a04Email);
            p.AddBool("a04IsRegional", rec.a04IsRegional);
            
            int intPID = _db.SaveRecord("a04Inspectorate", p.getDynamicDapperPars(), rec);

            if (rec.pid > 0 && intPID>0)    //vyčistit uživatelskou cache pro účty s vazbou na tento inspektorát
            {
                _db.RunSql("UPDATE j03User_CacheData set j03DateCache=convert(datetime,'01.01.2000',104) WHERE a04ID=@pid", new { pid = intPID });
            }

            return intPID;
        }

        public bool ValidateBeforeSave(BO.a04Inspectorate rec)
        {
            if (string.IsNullOrEmpty(rec.a04City))
            {
                this.AddMessage("Chybí vyplnit [Město]."); return false;
            }
            if (rec.a05ID==0)
            {
                this.AddMessage("Chybí vyplnit [Kraj]."); return false;
            }


            return true;
        }

    }
}
