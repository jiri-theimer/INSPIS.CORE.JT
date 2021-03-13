using System;
using System.Collections.Generic;
using System.Text;

namespace BL
{
    public interface If12FormTypeBL
    {
        public BO.f12FormType Load(int pid);
        public IEnumerable<BO.f12FormType> GetList(BO.myQuery mq);
        public int Save(BO.f12FormType rec);


    }
    class f12FormTypeBL : BaseBL, If12FormTypeBL
    {
        public f12FormTypeBL(BL.Factory mother) : base(mother)
        {

        }


        private string GetSQL1(string strAppend = null)
        {
            sb("SELECT a.*,dbo._core_f12_get_parent_inline(a.f12ID) as ParentPath,f12parent.f12Name as ParentName,");
            sb("case when a.f12TreeLevel>1 then replace(space(2*(a.f12TreeLevel-1)),' ','-') else '' END+a.f12Name as TreeItem,");
            sb(_db.GetSQL1_Ocas("f12"));
            sb(" FROM f12FormType a LEFT OUTER JOIN f12FormType f12parent ON a.f12ParentID=f12parent.f12ID");
            sb(strAppend);            

            return sbret();
        }
        public BO.f12FormType Load(int pid)
        {
            return _db.Load<BO.f12FormType>(GetSQL1(" WHERE a.f12ID=@pid"), new { pid = pid });
        }

        public IEnumerable<BO.f12FormType> GetList(BO.myQuery mq)
        {
            if (mq.explicit_orderby == null) { mq.explicit_orderby = "a.f12TreeIndex"; };
            DL.FinalSqlCommand fq = DL.basQuery.GetFinalSql(GetSQL1(), mq, _mother.CurrentUser);
            return _db.GetList<BO.f12FormType>(fq.FinalSql, fq.Parameters);
        }



        public int Save(BO.f12FormType rec)
        {
            if (!ValidateBeforeSave(rec))
            {
                return 0;
            }
            var p = new DL.Params4Dapper();
            p.AddInt("pid", rec.f12ID);
            p.AddInt("f12ParentID", rec.f12ParentID, true);            
            p.AddString("f12Name", rec.f12Name);            
            p.AddString("f12Description", rec.f12Description);
            
            
            int intPID = _db.SaveRecord("f12FormType", p, rec);
            if (intPID > 0)
            {
                _db.RunSql("exec dbo._core_f12_recalc_tree");
            }

            return intPID;
        }

        public bool ValidateBeforeSave(BO.f12FormType rec)
        {
            if (string.IsNullOrEmpty(rec.f12Name))
            {
                this.AddMessage("Chybí vyplnit [Název]."); return false;
            }

            if (rec.f12ParentID > 0)
            {
                var recParent = Load(rec.f12ParentID);
                if (rec.f12TreeIndexFrom<=recParent.f12TreeIndex && rec.f12TreeIndexTo >= recParent.f12TreeIndex)
                {
                    if (rec.f12TreeIndexFrom>0 || rec.f12TreeIndexTo>0 || recParent.f12TreeIndex > 0)
                    {
                        this.AddMessage("Stromové zatřídění položky není logické.");
                        return false;
                    }
                }
            }
            

            return true;
        }

    }
}
