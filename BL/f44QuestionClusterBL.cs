using System;
using System.Collections.Generic;
using System.Text;

namespace BL
{
    public interface If44QuestionClusterBL
    {
        public BO.f44QuestionCluster Load(int pid);
        public IEnumerable<BO.f44QuestionCluster> GetList(BO.myQuery mq);
        public int Save(BO.f44QuestionCluster rec);


    }
    class f44QuestionClusterBL : BaseBL, If44QuestionClusterBL
    {
        public f44QuestionClusterBL(BL.Factory mother) : base(mother)
        {

        }


        private string GetSQL1(string strAppend = null)
        {
            sb("SELECT a.*,");
            sb(_db.GetSQL1_Ocas("f44"));
            sb(" FROM f44QuestionCluster a");
            sb(strAppend);

            return sbret();
        }
        public BO.f44QuestionCluster Load(int pid)
        {
            return _db.Load<BO.f44QuestionCluster>(GetSQL1(" WHERE a.f44ID=@pid"), new { pid = pid });
        }

        public IEnumerable<BO.f44QuestionCluster> GetList(BO.myQuery mq)
        {
            if (mq.explicit_orderby == null) { mq.explicit_orderby = "a.f44Ordinal"; };
            DL.FinalSqlCommand fq = DL.basQuery.GetFinalSql(GetSQL1(), mq, _mother.CurrentUser);
            return _db.GetList<BO.f44QuestionCluster>(fq.FinalSql, fq.Parameters);
        }



        public int Save(BO.f44QuestionCluster rec)
        {
            if (!ValidateBeforeSave(rec))
            {
                return 0;
            }
            var p = new DL.Params4Dapper();
            p.AddInt("pid", rec.f44ID);
            p.AddInt("f44Ordinal", rec.f44Ordinal);
            p.AddString("f44Name", rec.f44Name);
            p.AddString("f44Code", rec.f44Code);


            int intPID = _db.SaveRecord("f44QuestionCluster", p, rec);
            

            return intPID;
        }

        public bool ValidateBeforeSave(BO.f44QuestionCluster rec)
        {
            if (string.IsNullOrEmpty(rec.f44Name))
            {
                this.AddMessage("Chybí vyplnit [Název]."); return false;
            }            

            return true;
        }

    }
}
