using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace BL
{
    public interface Ia41PersonToEventBL
    {
        public BO.a41PersonToEvent Load(int pid);
        public BO.a41PersonToEvent LoadByJ02ID(int j02id,int a01id,int intExcludePid);
        public IEnumerable<BO.a41PersonToEvent> GetList(BO.myQuery mq);
        public int Save(BO.a41PersonToEvent rec, bool bolAppend2WorkflowHistory, BO.b06WorkflowStep recB06 = null);
        public void UpdateChildEvents(BO.a01Event recParent);
        public bool ValidateBeforeSave(BO.a41PersonToEvent c);
    }
    class a41PersonToEventBL : BaseBL, Ia41PersonToEventBL
    {

        public a41PersonToEventBL(BL.Factory mother) : base(mother)
        {

        }


        private string GetSQL1(string strAppend = null)
        {
            sb("SELECT a.*,");
            sb(_db.GetSQL1_Ocas("a41",false,false));
            sb(",a11_a45.a45Name,a11_a45.a45IsPlan,a11_a45.a45IsManual");
            sb(",a11_j11.j11Name,a11_j02.j02IsInvitedPerson,a11_j02.j02FirstName,a11_j02.j02LastName,a11_j02.j02TitleBeforeName,a11_j02.j02TitleAfterName");            
            sb(" FROM a41PersonToEvent a INNER JOIN a45EventRole a11_a45 ON a.a45ID=a11_a45.a45ID");
            sb(" LEFT OUTER JOIN j11Team a11_j11 on a.j11ID=a11_j11.j11ID LEFT OUTER JOIN j02Person a11_j02 ON a.j02ID=a11_j02.j02ID");            
            sb(strAppend);
            return sbret();
        }
        public BO.a41PersonToEvent Load(int pid)
        {
            return _db.Load<BO.a41PersonToEvent>(GetSQL1(" WHERE a.a41ID=@pid"), new { pid = pid });
        }
        public BO.a41PersonToEvent LoadByJ02ID(int j02id,int a01id, int intExcludePid)
        {
            return _db.Load<BO.a41PersonToEvent>(GetSQL1(" WHERE a.j02ID=@j02id AND a.a01ID=@a01id AND a.a41ID<>@excludepid"), new {j02id=j02id,a01id=a01id, excludepid = intExcludePid });
        }

        public IEnumerable<BO.a41PersonToEvent> GetList(BO.myQuery mq)
        {
            if (mq.explicit_orderby == null) mq.explicit_orderby = "a.a41ID DESC";
            DL.FinalSqlCommand fq = DL.basQuery.ParseFinalSql(GetSQL1(), mq, _mother.CurrentUser);
            return _db.GetList<BO.a41PersonToEvent>(fq.FinalSql, fq.Parameters);
        }

        public int Save(BO.a41PersonToEvent rec,bool bolAppend2WorkflowHistory,BO.b06WorkflowStep recB06=null)
        {           
            if (ValidateBeforeSave(rec) == false)
            {
                return 0;
            }
            var p = new DL.Params4Dapper();
            p.AddInt("pid", rec.a41ID);                                            
            p.AddEnumInt("a45ID", rec.a45ID);
            p.AddInt("a01ID", rec.a01ID, true);
            p.AddInt("j11ID", rec.j11ID, true);
            p.AddInt("j02ID", rec.j02ID, true);
            p.AddInt("b06ID_NomineeSource", rec.b06ID_NomineeSource, true);
            p.AddBool("a41IsAllocateAllPeriod", rec.a41IsAllocateAllPeriod);
         

            int intA41ID = _db.SaveRecord("a41PersonToEvent", p.getDynamicDapperPars(), rec,false);
            _db.RunSql("UPDATE a01Event set a01LeaderInLine=dbo._core_a41_get_role_inline(@pid,2),a01MemberInLine=dbo._core_a41_get_role_inline(@pid,1) WHERE a01ID=@pid", new { pid = rec.a01ID });
            if (bolAppend2WorkflowHistory)
            {
                var recB05 = new BO.b05Workflow_History() { a01ID = rec.a01ID, b05IsNominee = true, j02ID_Nominee = rec.j02ID, j11ID_Nominee = rec.j11ID };
                recB05.a45ID_Nominee = (int)rec.a45ID;
                if (recB06 != null)
                {
                    recB05.b06ID = recB06.pid;
                }
                if (rec.j02ID > 0)
                {
                    recB05.b05Comment = _mother.j02PersonBL.Load(rec.j02ID).FullNameAsc;
                }
                if (rec.j11ID > 0)
                {
                    recB05.b05Comment = _mother.j11TeamBL.Load(rec.j02ID).j11Name;
                }
                _mother.b05Workflow_HistoryBL.Save(recB05);
            }
           
            return intA41ID;
        }
        public void UpdateChildEvents(BO.a01Event recParent)
        {   //aktualizovat účastníky i v podřízených dětech matky
            if (recParent.a01ChildsCount > 0 && recParent.a01ParentID == 0)
            {   
                _db.RunSql("DELETE FROM a41PersonToEvent WHERE a01ID IN (SELECT a01ID FROM a01Event WHERE a01ParentID=@pid)", new { pid = recParent.pid });
                var mq = new BO.myQuery("a01");
                mq.a01parentid = recParent.pid;
                foreach(var c in _mother.a01EventBL.GetList(mq))
                {
                    sbinit();
                    sb("INSERT INTO a41PersonToEvent(a01ID,a45ID,j02ID,j11ID,a41IsAllocateAllPeriod,a41DateInsert,a41DateUpdate,a41UserInsert,a41UserUpdate)");
                    sb(" SELECT @pid,a45ID,j02ID,j11ID,a41IsAllocateAllPeriod,a41DateInsert,a41DateUpdate,a41UserInsert,a41UserUpdate");
                    sb(" FROM a41PersonToEvent WHERE a01ID=@parentpid");
                    _db.RunSql(sbret(), new { pid = c.pid, parentpid =recParent.pid});

                    _db.RunSql("UPDATE a01Event set a01LeaderInLine=dbo._core_a41_get_role_inline(@pid,2),a01MemberInLine=dbo._core_a41_get_role_inline(@pid,1) WHERE a01ID=@pid", new { pid = recParent.pid });
                }
            }
        }

        public bool ValidateBeforeSave(BO.a41PersonToEvent c)
        {
            
            if (c.j02ID==0 && c.j11ID == 0)
            {
                this.AddMessage("Chybí obsazení osoby nebo týmu.");return false;
            }
            if (c.a45ID == 0)
            {
                this.AddMessage("Chybí role v akci."); return false;
            }
            if (LoadByJ02ID(c.j02ID, c.a01ID,c.pid) != null)
            {
                this.AddMessageTranslated(string.Format(_mother.tra("Osoba [{0}] již byla zařazena mezi účastníky této akce."), LoadByJ02ID(c.j02ID,c.a01ID, c.pid).PersonAsc)); return false;
            }
            if (c.a45ID == BO.EventRoleENUM.PrizvanaOsoba && _mother.j02PersonBL.Load(c.j02ID).j02IsInvitedPerson == false)
            {
                this.AddMessageTranslated(string.Format(_mother.tra("V obsazení akce je osoba [{0}] zařazena jako 'Přizvaná'. Ale v nastavení osobního profilu není uvedeno, že může být přizvanou osobou."), _mother.j02PersonBL.Load(c.j02ID).FullNameAsc)); return false;
            }
          
            

            return true;
        }
    }
}
