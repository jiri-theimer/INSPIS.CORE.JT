using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace BL
{
    public interface Ia35PersonEventPlanBL
    {
        public BO.a35PersonEventPlan Load(int pid);
        public IEnumerable<BO.a35PersonEventPlan> GetList(BO.myQuery mq);
        public int Save(BO.a35PersonEventPlan rec);
        public IEnumerable<BO.a35TimeLine> GetListTimeLine(BO.myQuery mq);


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

        public IEnumerable<BO.a35PersonEventPlan> GetList(BO.myQuery mq)
        {            
            DL.FinalSqlCommand fq = DL.basQuery.ParseFinalSql(GetSQL1(), mq, _mother.CurrentUser);
            return _db.GetList<BO.a35PersonEventPlan>(fq.FinalSql, fq.Parameters);
        }
        public IEnumerable<BO.a35TimeLine> GetListTimeLine(BO.myQuery mq)
        {
            sb("SELECT a.j02ID,a.a01ID,a.a35PlanDate,COUNT(*) as Krat");
            sb(" FROM a35PersonEventPlan a INNER JOIN j02Person j02 ON a.j02ID=j02.j02ID");            

            DL.FinalSqlCommand fq = DL.basQuery.ParseFinalSql(sbret(), mq, _mother.CurrentUser);
            
            var lis = _db.GetList<BO.a35TimeLine>(fq.FinalSql+ " GROUP BY a.j02ID,a.a01ID,a.a35PlanDate", fq.Parameters);
            
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
