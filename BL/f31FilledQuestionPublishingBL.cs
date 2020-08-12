using System;
using System.Collections.Generic;
using System.Text;

namespace BL
{
    public interface If31FilledQuestionPublishingBL
    {
        public BO.f31FilledQuestionPublishing Load(int pid);
        public IEnumerable<BO.f31FilledQuestionPublishing> GetList(BO.myQuery mq);
        public int Save(BO.f31FilledQuestionPublishing rec);

    }
    class f31FilledQuestionPublishingBL : BaseBL, If31FilledQuestionPublishingBL
    {
        public f31FilledQuestionPublishingBL(BL.Factory mother) : base(mother)
        {

        }


        private string GetSQL1(string strAppend = null)
        {
            sb("SELECT a.*,");
            sb(_db.GetSQL1_Ocas("f31",false,false,true));
            sb(" FROM f31FilledQuestionPublishing a");
            sb(strAppend);
            return sbret();
        }
        public BO.f31FilledQuestionPublishing Load(int pid)
        {
            return _db.Load<BO.f31FilledQuestionPublishing>(GetSQL1(" WHERE a.f31ID=@pid"), new { pid = pid });
        }

        public IEnumerable<BO.f31FilledQuestionPublishing> GetList(BO.myQuery mq)
        {
            sb("SELECT a.*,");
            sb(_db.GetSQL1_Ocas("f31", false, false, true));
            sb(" FROM f31FilledQuestionPublishing a INNER JOIN a11EventForm a11 ON a.a11ID=a11.a11ID");
            if (mq.f18id > 0)
            {
                sb(" INNER JOIN f19Question f19 ON a.f19ID=f19.f19ID");
            }
           
            DL.FinalSqlCommand fq = DL.basQuery.ParseFinalSql(sbret(), mq, _mother.CurrentUser);
            return _db.GetList<BO.f31FilledQuestionPublishing>(fq.FinalSql, fq.Parameters);
        }



        public int Save(BO.f31FilledQuestionPublishing rec)
        {           
            var p = new DL.Params4Dapper();
            p.AddInt("pid", rec.f31ID);
            p.AddInt("j03id_sys", _mother.CurrentUser.pid,true);
            p.AddInt("a11ID", rec.a11ID,true);
            p.AddInt("f19ID", rec.f19ID, true);
            p.AddInt("f25ID", rec.f25ID, true);
            p.AddInt("f26ID", rec.f26ID, true);
            p.AddBool("f31IsPublished", rec.f31IsPublished);
           

            int intPID = _db.SaveRecord("f31FilledQuestionPublishing", p.getDynamicDapperPars(), rec);

            return intPID;
        }

     

    }
}
