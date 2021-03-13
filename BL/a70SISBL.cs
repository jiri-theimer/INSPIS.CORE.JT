using System;
using System.Collections.Generic;
using System.Text;

namespace BL
{
    public interface Ia70SISBL
    {
        public BO.a70SIS Load(int pid);
        public IEnumerable<BO.a70SIS> GetList(BO.myQuery mq);
        public int Save(BO.a70SIS rec);
        

    }
    class a70SISBL : BaseBL, Ia70SISBL
    {
        public a70SISBL(BL.Factory mother) : base(mother)
        {

        }


        private string GetSQL1(string strAppend = null)
        {
            sb("SELECT a.*,case a.a70ScopFlag when 2 then 'Pouze reálné školy' when 1 then 'Pouze testovací školy' when 3 then 'Všechny školy' when 4 then 'Obecně integrovaný' end as ScopeFlagAlias,");
            sb(_db.GetSQL1_Ocas("a70"));
            sb(" FROM a70SIS a");
            sb(strAppend);
            return sbret();
        }
        public BO.a70SIS Load(int pid)
        {
            return _db.Load<BO.a70SIS>(GetSQL1(" WHERE a.a70ID=@pid"), new { pid = pid });
        }
       

        public IEnumerable<BO.a70SIS> GetList(BO.myQuery mq)
        {
            if (mq.explicit_orderby == null) { mq.explicit_orderby = "a.a70Name"; };
            DL.FinalSqlCommand fq = DL.basQuery.GetFinalSql(GetSQL1(), mq, _mother.CurrentUser);
            return _db.GetList<BO.a70SIS>(fq.FinalSql, fq.Parameters);
        }



        public int Save(BO.a70SIS rec)
        {
            
            if (ValidateBeforeSave(rec) == false)
            {
                return 0;
            }
            
            var p = new DL.Params4Dapper();
            p.AddInt("pid", rec.a70ID);
            p.AddString("a70Name", rec.a70Name);
            p.AddString("a70Code", rec.a70Code);
            p.AddEnumInt("a70ScopFlag", rec.a70ScopFlag);
            p.AddString("a70Description", rec.a70Description);
            if (rec.a70ScopFlag == BO.a70ScopFlagENUM.GlobalSystem)
            {
                p.AddString("a70WsLogin", rec.a70WsLogin);
            }
            else
            {
                p.AddString("a70WsLogin", null);
            }
            

            int intPID = _db.SaveRecord("a70SIS", p, rec);



            return intPID;
        }

        public bool ValidateBeforeSave(BO.a70SIS rec)
        {
            if (string.IsNullOrEmpty(rec.a70Name))
            {
                this.AddMessage("Chybí vyplnit [Název]."); return false;
            }
            if (string.IsNullOrEmpty(rec.a70Code))
            {
                this.AddMessage("Chybí vyplnit [Kód]."); return false;
            }
            if (rec.a70ScopFlag==BO.a70ScopFlagENUM.GlobalSystem && string.IsNullOrEmpty(rec.a70WsLogin) == true)
            {
                this.AddMessage("Pro obecný integrovaný systém je povinné specifikovat [Login web služby]");return false;
            }
            

            return true;
        }

    }
}
