using System;
using System.Collections.Generic;
using System.Text;

namespace BL
{
    public interface Ia18DepartmentDomainBL
    {
        public BO.a18DepartmentDomain Load(int pid);
        public IEnumerable<BO.a18DepartmentDomain> GetList(BO.myQuery mq);
        public int Save(BO.a18DepartmentDomain rec);
        public BO.a18DepartmentDomain LoadByCode(string code, int pid_exclude);

    }
    class a18DepartmentDomainBL : BaseBL, Ia18DepartmentDomainBL
    {
        public a18DepartmentDomainBL(BL.Factory mother) : base(mother)
        {

        }


        private string GetSQL1(string strAppend = null)
        {
            sb("SELECT a.*,case when a.a18SubCode2 is not null then a.a18SubCode1+a.a18SubCode2 end as Skupina,case when LEN(a.a18Code)>=5 then LEFT(a.a18Code,5) end as Obor,");
            sb(_db.GetSQL1_Ocas("a18"));
            sb(" FROM a18DepartmentDomain a");
            sb(strAppend);
            return sbret();
        }
        public BO.a18DepartmentDomain Load(int pid)
        {
            return _db.Load<BO.a18DepartmentDomain>(GetSQL1(" WHERE a.a18ID=@pid"), new { pid = pid });
        }
        public BO.a18DepartmentDomain LoadByCode(string code, int pid_exclude)
        {
            return _db.Load<BO.a18DepartmentDomain>(GetSQL1(" WHERE a.a18Code LIKE @code AND a.a18ID<>@pid_exclude"), new { code = code, pid_exclude = pid_exclude });
        }

        public IEnumerable<BO.a18DepartmentDomain> GetList(BO.myQuery mq)
        {
            if (mq.explicit_orderby == null) { mq.explicit_orderby = "a.a18Name"; };
            DL.FinalSqlCommand fq = DL.basQuery.ParseFinalSql(GetSQL1(), mq, _mother.CurrentUser);
            return _db.GetList<BO.a18DepartmentDomain>(fq.FinalSql, fq.Parameters);
        }



        public int Save(BO.a18DepartmentDomain rec)
        {
            rec.a18Code = rec.a18Code.Trim();
            if (ValidateBeforeSave(rec) == false)
            {
                return 0;
            }
            rec.a18SubCode1 = rec.a18Code.Substring(0, 1);
            if (rec.a18Code.Length >= 2) { rec.a18SubCode2 = rec.a18Code.Substring(1, 1); };
            if (rec.a18Code.Length >= 4) { rec.a18SubCode3 = rec.a18Code.Substring(2, 2); };
            if (rec.a18Code.Length >= 6) { rec.a18SubCode4 = rec.a18Code.Substring(4, 1); };

            var p = new DL.Params4Dapper();
            p.AddInt("pid", rec.a18ID);
            p.AddString("a18Name", rec.a18Name);
            p.AddString("a18Code", rec.a18Code);
            p.AddString("a18SubCode1", rec.a18SubCode1);
            p.AddString("a18SubCode2", rec.a18SubCode2);
            p.AddString("a18SubCode3", rec.a18SubCode3);
            p.AddString("a18SubCode4", rec.a18SubCode4);
            

            int intPID = _db.SaveRecord("a18DepartmentDomain", p.getDynamicDapperPars(), rec);



            return intPID;
        }

        public bool ValidateBeforeSave(BO.a18DepartmentDomain rec)
        {
            if (string.IsNullOrEmpty(rec.a18Name))
            {
                this.AddMessage("Chybí vyplnit [Název]."); return false;
            }
            if (string.IsNullOrEmpty(rec.a18Code))
            {
                this.AddMessage("Chybí vyplnit [Kód]."); return false;
            }
            if (LoadByCode(rec.a18Code,rec.pid) != null)
            {
                this.AddMessageTranslated(string.Format(_mother.tra("Záznam s kódem[{0}] již existuje v záznamu: {1}."), rec.a18Code, LoadByCode(rec.a18Code, rec.pid).a18Name));return false;
            }

            return true;
        }

    }
}
