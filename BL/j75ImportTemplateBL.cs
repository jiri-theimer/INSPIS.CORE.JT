using System;
using System.Collections.Generic;
using System.Text;

namespace BL
{
    public interface Ij75ImportTemplateBL
    {
        public BO.j75ImportTemplate Load(int pid);
        public IEnumerable<BO.j75ImportTemplate> GetList(BO.myQuery mq);
        public int Save(BO.j75ImportTemplate rec);
        public bool Delete(int pid);

    }
    class j75ImportTemplateBL : BaseBL, Ij75ImportTemplateBL
    {
        public j75ImportTemplateBL(BL.Factory mother) : base(mother)
        {

        }


        private string GetSQL1(string strAppend = null)
        {
            sb("SELECT a.*,");
            sb(_db.GetSQL1_Ocas("j75"));
            sb(" FROM j75ImportTemplate a");
            sb(strAppend);
            return sbret();
        }
        public BO.j75ImportTemplate Load(int pid)
        {
            return _db.Load<BO.j75ImportTemplate>(GetSQL1(" WHERE a.j75ID=@pid"), new { pid = pid });
        }


        public IEnumerable<BO.j75ImportTemplate> GetList(BO.myQuery mq)
        {
            if (mq.explicit_orderby == null) { mq.explicit_orderby = "a.j75Name"; };
            DL.FinalSqlCommand fq = DL.basQuery.ParseFinalSql(GetSQL1(), mq, _mother.CurrentUser);
            return _db.GetList<BO.j75ImportTemplate>(fq.FinalSql, fq.Parameters);
        }

        public bool Delete(int pid)
        {
            _db.RunSql("DELETE FROM j75ImportTemplate WHERE j75ID=@pid", new { pid = pid });
            return true;
        }

        public int Save(BO.j75ImportTemplate rec)
        {

            if (ValidateBeforeSave(rec) == false)
            {
                return 0;
            }

            var p = new DL.Params4Dapper();
            p.AddInt("pid", rec.j75ID);
            p.AddString("j75Name", rec.j75Name);
            p.AddInt("x29ID", rec.x29ID,true);
            p.AddInt("j03ID", rec.j03ID, true);
            p.AddInt("a06ID", rec.a06ID, true);
            p.AddString("j75Pairs", rec.j75Pairs);
           
            int intPID = _db.SaveRecord("j75ImportTemplate", p.getDynamicDapperPars(), rec);

            return intPID;
        }

        public bool ValidateBeforeSave(BO.j75ImportTemplate rec)
        {
            if (string.IsNullOrEmpty(rec.j75Name))
            {
                this.AddMessage("Chybí vyplnit [Název]."); return false;
            }
         


            return true;
        }

    }
}
