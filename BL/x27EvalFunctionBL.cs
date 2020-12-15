using System;
using System.Collections.Generic;
using System.Text;

namespace BL
{
    public interface Ix27EvalFunctionBL
    {
        public BO.x27EvalFunction Load(int pid);
        public IEnumerable<BO.x27EvalFunction> GetList(BO.myQuery mq);
        public int Save(BO.x27EvalFunction rec);

    }
    class x27EvalFunctionBL : BaseBL, Ix27EvalFunctionBL
    {
        public x27EvalFunctionBL(BL.Factory mother) : base(mother)
        {

        }


        private string GetSQL1(string strAppend = null)
        {
            sb("SELECT a.*,");
            sb(_db.GetSQL1_Ocas("x27"));            
            sb(" FROM x27EvalFunction a");            
            sb(strAppend);            
            return sbret();
        }
        public BO.x27EvalFunction Load(int pid)
        {
            return _db.Load<BO.x27EvalFunction>(GetSQL1(" WHERE a.x27ID=@pid"), new { pid = pid });
        }

        public IEnumerable<BO.x27EvalFunction> GetList(BO.myQuery mq)
        {            
            if (mq.explicit_orderby == null) { mq.explicit_orderby = "a.x27Ordinal"; };
            DL.FinalSqlCommand fq = DL.basQuery.GetFinalSql(GetSQL1(), mq, _mother.CurrentUser);
            return _db.GetList<BO.x27EvalFunction>(fq.FinalSql, fq.Parameters);
        }



        public int Save(BO.x27EvalFunction rec)
        {
            if (ValidateBeforeSave(rec) == false)
            {
                return 0;
            }
            var p = new DL.Params4Dapper();
            p.AddInt("pid", rec.x27ID);            
            p.AddString("x27Name", rec.x27Name);
            p.AddString("x27Description", rec.x27Description);
            p.AddInt("x27Ordinal", rec.x27Ordinal);
            p.AddString("x27Parameters", rec.x27Parameters);
            p.AddString("x27Returns", rec.x27Returns);
           

            int intPID = _db.SaveRecord("x27EvalFunction", p.getDynamicDapperPars(), rec);



            return intPID;
        }

        public bool ValidateBeforeSave(BO.x27EvalFunction rec)
        {
            if (string.IsNullOrEmpty(rec.x27Name) || string.IsNullOrEmpty(rec.x27Returns))
            {
                this.AddMessage("[Název] a [Návratová hodnota] jsou povinná pole."); return false;
            }



            return true;
        }

    }
}
