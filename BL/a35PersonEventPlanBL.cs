using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace BL
{
    public interface Ia35PersonEventPlanBL
    {
        public BO.a35PersonEventPlan Load(int pid);
        public IEnumerable<BO.a35PersonEventPlan> GetList(BO.myQueryA35 mq);
        public int Save(BO.a35PersonEventPlan rec);
        public IEnumerable<BO.a35TimeLine> GetListTimeLine(BO.myQueryA35 mq);
        public IEnumerable<BO.a35TimeLinePersonal> GetListTimeLinePersonal(BO.myQueryA35 mq);

    }
    class a35PersonEventPlanBL : BaseBL, Ia35PersonEventPlanBL
    {

        public a35PersonEventPlanBL(BL.Factory mother) : base(mother)
        {

        }


        private string GetSQL1(string strAppend = null)
        {
            sb("SELECT a.*,");
            sb(_db.GetSQL1_Ocas("a35", false, false));
            sb(",a01.a01ParentID");
            sb(",j02.j02FirstName,j02.j02LastName,j02.j02TitleBeforeName,j02.j02TitleAfterName");
            sb(" FROM a35PersonEventPlan a INNER JOIN j02Person j02 ON a.j02ID=j02.j02ID INNER JOIN a01Event a01 ON a.a01ID=a01.a01ID");            
            sb(strAppend);
            return sbret();
        }
        public BO.a35PersonEventPlan Load(int pid)
        {
            return _db.Load<BO.a35PersonEventPlan>(GetSQL1(" WHERE a.a35ID=@pid"), new { pid = pid });
        }

        public IEnumerable<BO.a35PersonEventPlan> GetList(BO.myQueryA35 mq)
        {            
            DL.FinalSqlCommand fq = DL.basQuery.GetFinalSql(GetSQL1(), mq, _mother.CurrentUser);
            return _db.GetList<BO.a35PersonEventPlan>(fq.FinalSql, fq.Parameters);
        }
        public IEnumerable<BO.a35TimeLine> GetListTimeLine(BO.myQueryA35 mq)
        {
            sb("SELECT a.j02ID,a.a01ID,a.a35PlanDate,COUNT(*) as Krat");
            sb(" FROM a35PersonEventPlan a INNER JOIN j02Person j02 ON a.j02ID=j02.j02ID");            

            DL.FinalSqlCommand fq = DL.basQuery.GetFinalSql(sbret(), mq, _mother.CurrentUser);
            
            var lis = _db.GetList<BO.a35TimeLine>(fq.FinalSql+ " GROUP BY a.j02ID,a.a01ID,a.a35PlanDate", fq.Parameters);
            
            return lis;
        }
        public IEnumerable<BO.a35TimeLinePersonal> GetListTimeLinePersonal(BO.myQueryA35 mq)
        {
            sb("SELECT a.a01ID,min(a01.a03ID) as a03ID,a.a35PlanDate,COUNT(*) as Krat");
            sb(",min(a01Signature) as a01Signature,min(a03Name) as a03Name,min(a03REDIZO) as a03REDIZO,min(a01DateFrom) as a01DateFrom,min(a01DateUntil) as a01DateUntil");
            sb(",min(a10Name) as a10Name,min(b02Name) as b02Name,min(a10ViewUrl_Page) as a10ViewUrl_Page");
            sb(" FROM a35PersonEventPlan a INNER JOIN a01Event a01 ON a.a01ID=a01.a01ID");
            sb(" INNER JOIN b02WorkflowStatus b02 ON a01.b02ID=b02.b02ID");
            sb(" INNER JOIN a10EventType a10 ON a01.a10ID=a10.a10ID");
            sb(" LEFT OUTER JOIN a03Institution a03 ON a01.a03ID=a03.a03ID");

            DL.FinalSqlCommand fq = DL.basQuery.GetFinalSql(sbret(), mq, _mother.CurrentUser);

            var lis = _db.GetList<BO.a35TimeLinePersonal>(fq.FinalSql + " GROUP BY a.a01ID,a.a35PlanDate", fq.Parameters);

            return lis;
        }

        public int Save(BO.a35PersonEventPlan rec)
        {
           
            var p = new DL.Params4Dapper();
            p.AddInt("pid", rec.a35ID);            
            p.AddInt("a01ID", rec.a01ID, true);            
            p.AddInt("j02ID", rec.j02ID, true);
            p.AddDateTime("a35PlanDate", rec.a35PlanDate);

            int inta35ID = _db.SaveRecord("a35PersonEventPlan", p.getDynamicDapperPars(), rec, false);
           
            return inta35ID;
        }
       

        
    }
}
