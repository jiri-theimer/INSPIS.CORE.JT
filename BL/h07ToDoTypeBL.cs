using System;
using System.Collections.Generic;
using System.Text;

namespace BL
{
    public interface Ih07ToDoTypeBL
    {
        public BO.h07ToDoType Load(int pid);
        public IEnumerable<BO.h07ToDoType> GetList(BO.myQuery mq);
        public int Save(BO.h07ToDoType rec);

    }
    class h07ToDoTypeBL : BaseBL, Ih07ToDoTypeBL
    {
        public h07ToDoTypeBL(BL.Factory mother) : base(mother)
        {

        }


        private string GetSQL1(string strAppend = null)
        {
            sb("SELECT a.*,b65.b65Name,");
            sb(_db.GetSQL1_Ocas("h07"));
            sb(" FROM h07ToDoType a LEFT OUTER JOIN b65WorkflowMessage b65 ON a.b65ID=b65.b65ID");
            sb(strAppend);
            return sbret();
        }
        public BO.h07ToDoType Load(int pid)
        {
            return _db.Load<BO.h07ToDoType>(GetSQL1(" WHERE a.h07ID=@pid"), new { pid = pid });
        }

        public IEnumerable<BO.h07ToDoType> GetList(BO.myQuery mq)
        {
            DL.FinalSqlCommand fq = DL.basQuery.ParseFinalSql(GetSQL1(), mq, _mother.CurrentUser);
            return _db.GetList<BO.h07ToDoType>(fq.FinalSql, fq.Parameters);
        }



        public int Save(BO.h07ToDoType rec)
        {
            if (ValidateBeforeSave(rec) == false)
            {
                return 0;
            }
            var p = new DL.Params4Dapper();
            p.AddInt("pid", rec.h07ID);
            p.AddInt("b65ID", rec.b65ID, true);
            p.AddString("h07Name", rec.h07Name);
            p.AddString("h07Description", rec.h07Description);
            p.AddBool("h07IsCapacityPlan", rec.h07IsCapacityPlan);
            p.AddBool("h07IsDefault", rec.h07IsDefault);
            p.AddBool("h07IsToDo", rec.h07IsToDo);


            int intPID = _db.SaveRecord("h07ToDoType", p.getDynamicDapperPars(), rec);



            return intPID;
        }

        public bool ValidateBeforeSave(BO.h07ToDoType rec)
        {
            if (string.IsNullOrEmpty(rec.h07Name))
            {
                this.AddMessage("Chybí vyplnit [Název]."); return false;
            }


            return true;
        }

    }
}
