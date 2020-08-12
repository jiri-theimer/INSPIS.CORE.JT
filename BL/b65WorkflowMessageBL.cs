using System;
using System.Collections.Generic;
using System.Text;

namespace BL
{
    public interface Ib65WorkflowMessageBL
    {
        public BO.b65WorkflowMessage Load(int pid);
        public BO.b65WorkflowMessage LoadByCode(string code, int pid_exclude);
        public IEnumerable<BO.b65WorkflowMessage> GetList(BO.myQuery mq);
        public int Save(BO.b65WorkflowMessage rec);

    }
    class b65WorkflowMessageBL : BaseBL, Ib65WorkflowMessageBL
    {
        public b65WorkflowMessageBL(BL.Factory mother) : base(mother)
        {

        }


        private string GetSQL1(string strAppend = null)
        {
            sb("SELECT a.*,x29.x29Name,");
            sb(_db.GetSQL1_Ocas("b65"));
            sb(" FROM b65WorkflowMessage a LEFT OUTER JOIN x29Entity x29 ON a.x29ID=x29.x29ID");
            sb(strAppend);
            return sbret();
        }
        public BO.b65WorkflowMessage Load(int pid)
        {
            return _db.Load<BO.b65WorkflowMessage>(GetSQL1(" WHERE a.b65ID=@pid"), new { pid = pid });
        }
        public BO.b65WorkflowMessage LoadByCode(string code, int pid_exclude)
        {
            return _db.Load<BO.b65WorkflowMessage>(GetSQL1(" WHERE a.b65PID LIKE @code AND a.b65ID<>@pid_exclude"), new { code = code, pid_exclude = pid_exclude });
        }


        public IEnumerable<BO.b65WorkflowMessage> GetList(BO.myQuery mq)
        {
            if (mq.explicit_orderby == null) { mq.explicit_orderby = "a.b65Name"; };
            DL.FinalSqlCommand fq = DL.basQuery.ParseFinalSql(GetSQL1(), mq, _mother.CurrentUser);
            return _db.GetList<BO.b65WorkflowMessage>(fq.FinalSql, fq.Parameters);
        }



        public int Save(BO.b65WorkflowMessage rec)
        {
            if (ValidateBeforeSave(rec) == false)
            {
                return 0;
            }
            var p = new DL.Params4Dapper();
            p.AddInt("pid", rec.b65ID);
            p.AddInt("x29ID", rec.x29ID, true);
            p.AddString("b65Name", rec.b65Name);
            p.AddString("b65MessageSubject", rec.b65MessageSubject);           
            p.AddString("b65MessageBody", rec.b65MessageBody);
          
            int intPID = _db.SaveRecord("b65WorkflowMessage", p.getDynamicDapperPars(), rec);
           
            return intPID;
        }

        public bool ValidateBeforeSave(BO.b65WorkflowMessage rec)
        {
            if (string.IsNullOrEmpty(rec.b65Name))
            {
                this.AddMessage("Chybí vyplnit [Název]."); return false;
            }
            if (string.IsNullOrEmpty(rec.b65MessageSubject))
            {
                this.AddMessage("Chybí vyplnit [Předmět zprávy]."); return false;
            }

            if (rec.x29ID == 0)
            {
                this.AddMessage("Chybí vyplnit [Entita]."); return false;
            }
            


            return true;
        }

    }
}
