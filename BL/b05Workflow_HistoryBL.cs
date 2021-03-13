using System;
using System.Collections.Generic;

namespace BL
{
    public interface Ib05Workflow_HistoryBL
    {
        public BO.b05Workflow_History Load(int pid);
        public BO.b05Workflow_History LoadLast(int a01id);
        public IEnumerable<BO.b05Workflow_History> GetList(BO.myQuery mq);
        public int Save(BO.b05Workflow_History rec);
        public IEnumerable<BO.b04WorkflowComment_Restriction> GetList_b04(int b05id);
    }
    class b05Workflow_HistoryBL : BaseBL, Ib05Workflow_HistoryBL
    {

        public b05Workflow_HistoryBL(BL.Factory mother) : base(mother)
        {

        }


        private string GetSQL1(string strAppend = null,int intTopRecsOnly=0)
        {
            if (intTopRecsOnly>0)
            {
                sb(string.Format("SELECT TOP {0} a.*,",intTopRecsOnly));
            }
            else
            {
                sb("SELECT a.*,");
            }            
            sb(_db.GetSQL1_Ocas("b05", false, false));
            sb(",b06.b06Name,b06.b06IsHistoryAllowedToAll,b02from.b02name as b02Name_From,b02to.b02Name as b02Name_To");
            sb(",j03.j03Login,dbo._core_j02_fullname_asc(j02.j02TitleBeforeName,j02.j02FirstName,j02.j02LastName,j02.j02TitleAfterName) as Person,j02.j02ID");
            sb(",a45_nominee.a45Name,a01.a01Signature");
            sb(" FROM b05Workflow_History a INNER JOIN a01Event a01 ON a.a01ID=a01.a01ID LEFT OUTER JOIN b06WorkflowStep b06 on a.b06ID=b06.b06ID");
            sb(" LEFT OUTER JOIN j11Team a11_j11 on a.j11ID_Nominee=a11_j11.j11ID LEFT OUTER JOIN j02Person a11_j02 ON a.j02ID_Nominee=a11_j02.j02ID");
            sb(" LEFT OUTER JOIN j03user j03 on a.j03ID_Sys=j03.j03ID LEFT OUTER JOIN j02Person j02 on j03.j02ID=j02.j02ID");
            sb(" LEFT OUTER JOIN b02WorkflowStatus b02from on a.b02ID_From=b02from.b02ID LEFT OUTER JOIN b02WorkflowStatus b02to on a.b02ID_To=b02to.b02ID");
            sb(" LEFT OUTER JOIN a45EventRole a45_nominee ON a.a45ID_Nominee=a45_nominee.a45ID");
            sb(strAppend);
            return sbret();
        }
        public BO.b05Workflow_History Load(int pid)
        {
            return _db.Load<BO.b05Workflow_History>(GetSQL1(" WHERE a.b05ID=@pid"), new { pid = pid });
        }
        public BO.b05Workflow_History LoadLast(int a01id)
        {
            return _db.Load<BO.b05Workflow_History>(GetSQL1(" WHERE a.a01ID=@a01id ORDER BY a.b05ID DESC",1), new { a01id = a01id });
        }

        public IEnumerable<BO.b05Workflow_History> GetList(BO.myQuery mq)
        {
            if (mq.explicit_orderby == null) mq.explicit_orderby = "a.b05ID DESC";
            DL.FinalSqlCommand fq = DL.basQuery.GetFinalSql(GetSQL1(), mq, _mother.CurrentUser);
            return _db.GetList<BO.b05Workflow_History>(fq.FinalSql, fq.Parameters);
        }

        public IEnumerable<BO.b04WorkflowComment_Restriction> GetList_b04(int b05id)
        {
            return _db.GetList<BO.b04WorkflowComment_Restriction>("select a.*,b.a45Name FROM b04WorkflowComment_Restriction a INNER JOIN a45EventRole b ON a.a45ID=b.a45ID WHERE a.b05ID=@b05id", new { b05id = b05id });
        }

        public int Save(BO.b05Workflow_History rec)
        {
            var p = new DL.Params4Dapper();
            p.AddInt("pid", rec.b05ID,true);
            p.AddInt("a01ID", rec.a01ID, true);
            if (rec.j03ID_Sys == 0) { rec.j03ID_Sys = _db.CurrentUser.pid; };
            p.AddInt("j03id_sys", rec.j03ID_Sys, true);
            p.AddInt("b06ID", rec.b06ID, true);
            p.AddInt("b02ID_From", rec.b02ID_From,true);
            p.AddInt("b02ID_To", rec.b02ID_To,true);
            p.AddInt("j02ID_Nominee", rec.j02ID_Nominee, true);
            p.AddInt("j11ID_Nominee", rec.j11ID_Nominee, true);
            p.AddInt("a45ID_Nominee", rec.a45ID_Nominee,true);
                                  
            p.AddBool("b05IsManualStep", rec.b05IsManualStep);
            p.AddBool("b05IsCommentOnly", rec.b05IsCommentOnly);
            p.AddBool("b05IsCommentRestriction", rec.b05IsCommentRestriction);
            p.AddBool("b05IsBatchUpdate", rec.b05IsBatchUpdate);

            p.AddString("b05Comment", rec.b05Comment);
            p.AddString("b05SQL", rec.b05SQL);


            return _db.SaveRecord("b05Workflow_History", p, rec, false);
        }
    }
}
