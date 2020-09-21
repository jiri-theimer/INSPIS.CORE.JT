
using System;
using System.Collections.Generic;
using System.Text;

namespace BL
{
    public interface Ih04ToDoBL
    {
        public BO.h04ToDo Load(int pid);
        public IEnumerable<BO.h04ToDo> GetList(BO.myQuery mq);
        public IEnumerable<BO.h04TodoCapacity> GetListCapacity(BO.myQuery mq);
        public int Save(BO.h04ToDo rec, List<int> j02ids_resitel, List<int> j11ids_resitel, bool bolSendNotification);

    }
    class h04ToDoBL : BaseBL, Ih04ToDoBL
    {
        public h04ToDoBL(BL.Factory mother) : base(mother)
        {

        }


        private string GetSQL1(string strAppend = null)
        {
            sb("SELECT a.*,h07.h07Name,h07.h07IsDefault,h07.h07IsToDo,h07.h07IsCapacityPlan,h05.h05Name,a01.a01Signature,");
            sb(_db.GetSQL1_Ocas("h04"));
            sb(" FROM h04ToDo a INNER JOIN h07ToDoType h07 ON a.h07ID=h07.h07ID INNER JOIN a01Event a01 ON a.a01ID=a01.a01ID INNER JOIN j02Person j02 ON a.j02ID_Owner=j02.j02ID LEFT OUTER JOIN h05ToDoStatus h05 ON a.h05ID=h05.h05ID");
            sb(strAppend);
            return sbret();
        }
        public BO.h04ToDo Load(int pid)
        {
            return _db.Load<BO.h04ToDo>(GetSQL1(" WHERE a.h04ID=@pid"), new { pid = pid });
        }

        public IEnumerable<BO.h04ToDo> GetList(BO.myQuery mq)
        {
            if (mq.explicit_orderby == null) mq.explicit_orderby = "a.h04ID DESC";
            DL.FinalSqlCommand fq = DL.basQuery.ParseFinalSql(GetSQL1(), mq, _mother.CurrentUser);
            return _db.GetList<BO.h04ToDo>(fq.FinalSql, fq.Parameters);
        }
        public IEnumerable<BO.h04TodoCapacity> GetListCapacity(BO.myQuery mq)
        {
            sb("SELECT a.*,h07.h07Name,h07.h07IsDefault,h07.h07IsToDo,h07.h07IsCapacityPlan,h05.h05Name,h06.j02ID,a01.a01Signature,");            
            sb(_db.GetSQL1_Ocas("h04"));
            sb(" FROM h04ToDo a INNER JOIN h07ToDoType h07 ON a.h07ID=h07.h07ID INNER JOIN a01Event a01 ON a.a01ID=a01.a01ID INNER JOIN j02Person j02 ON a.j02ID_Owner=j02.j02ID LEFT OUTER JOIN h05ToDoStatus h05 ON a.h05ID=h05.h05ID");
            sb(" INNER JOIN h06ToDoReceiver h06 ON a.h04ID=h06.h04ID");

            DL.FinalSqlCommand fq = DL.basQuery.ParseFinalSql(sbret(), mq, _mother.CurrentUser);
            return _db.GetList<BO.h04TodoCapacity>(fq.FinalSql, fq.Parameters);
        }


        public int Save(BO.h04ToDo rec,List<int> j02ids_resitel, List<int> j11ids_resitel,bool bolSendNotification)
        {
            if (rec.h07ID == 0)
            {
                this.AddMessage("Chybí vyplnit [Typ]."); return 0;
            }
            var recH07 = _mother.h07ToDoTypeBL.Load(rec.h07ID);
            if (ValidateBeforeSave(rec,recH07, j02ids_resitel, j11ids_resitel) == false)
            {
                return 0;
            }
            var p = new DL.Params4Dapper();
            p.AddInt("pid", rec.h04ID);
            if (rec.pid > 0)
            {
                if (rec.j03ID_Creator == 0) rec.j03ID_Creator = _db.CurrentUser.pid;
                p.AddInt("j03ID_Creator", rec.j03ID_Creator, true);
            }
            if (rec.j02ID_Owner == 0) rec.j02ID_Owner = _db.CurrentUser.j02ID;
            p.AddInt("j02ID_Owner", rec.j02ID_Owner, true);

            p.AddInt("h07ID", rec.h07ID, true);
            if (recH07.h07IsToDo)
            {               
                p.AddInt("h05ID", rec.h05ID, true);
            }
            else
            {
                p.AddInt("h05ID",0,true);
            }
            if (recH07.h07IsCapacityPlan && rec.h04CapacityPlanUntil != null)
            {
                p.AddDateTime("h04CapacityPlanFrom", rec.h04CapacityPlanFrom);
                var d = Convert.ToDateTime(rec.h04CapacityPlanUntil);
                if (d.Hour==0 && d.Minute == 0)
                {
                    d = d.AddDays(1).AddMinutes(-1);
                }
                p.AddDateTime("h04CapacityPlanUntil", d);
            }
            else
            {
                p.AddDateTime("h04CapacityPlanFrom", null);
                p.AddDateTime("h04CapacityPlanUntil", null);
            }
            p.AddInt("a01ID", rec.a01ID, true);
            p.AddString("h04Name", rec.h04Name);
            p.AddString("h04Description", rec.h04Description);

            if (rec.h04Deadline != null)
            {
                var d = Convert.ToDateTime(rec.h04Deadline);
                if (d.Hour == 0 && d.Minute == 0) d = d.AddHours(12);
                p.AddDateTime("h04Deadline", d);
            }
            else
            {
                p.AddDateTime("h04Deadline", null);
            }
            
            

            p.AddBool("h04IsClosed", rec.h04IsClosed);
            p.AddDateTime("h04ReminderDate", rec.h04ReminderDate);


            int intPID = _db.SaveRecord("h04ToDo", p.getDynamicDapperPars(), rec);
            if (j02ids_resitel != null || j11ids_resitel != null)
            {
                if (rec.pid > 0) _db.RunSql("DELETE FROM h06ToDoReceiver WHERE h04ID=@pid", new { pid = intPID });
            }
            if (j02ids_resitel !=null && j02ids_resitel.Count > 0)
            {
                _db.RunSql("INSERT INTO h06ToDoReceiver(h04ID,j02ID,h06TodoRole) SELECT @pid,j02ID,1 FROM j02Person WHERE j02ID IN (" + string.Join(",", j02ids_resitel) + ")", new { pid = intPID });
            }
            if (j11ids_resitel != null && j11ids_resitel.Count > 0)
            {
                _db.RunSql("INSERT INTO h06ToDoReceiver(h04ID,j11ID,h06TodoRole) SELECT @pid,j11ID,1 FROM j11Team WHERE j11ID IN (" + string.Join(",", j11ids_resitel) + ")", new { pid = intPID });
            }


            return intPID;
        }

        public bool ValidateBeforeSave(BO.h04ToDo rec,BO.h07ToDoType recH07, List<int> j02ids_resitel, List<int> j11ids_resitel)
        {            
            if (rec.a01ID == 0)
            {
                this.AddMessage("Chybí vazba na akci."); return false;
            }
            if (string.IsNullOrEmpty(rec.h04Name))
            {
                this.AddMessage("Chybí vyplnit [Název]."); return false;
            }
            
            if (recH07.h07IsToDo && rec.h05ID == 0)
            {
                this.AddMessage("Chybí vyplnit [Stav úkolu]."); return false;
            }
            if (j02ids_resitel !=null && j11ids_resitel !=null && j02ids_resitel.Count==0 && j11ids_resitel.Count==0)
            {
                this.AddMessage("Okruh příjemců musí mít minimálně jednoho člena.");return false;
            }


            return true;
        }

    }
}
