using System;
using System.Collections.Generic;
using System.Text;

namespace BL
{
    public interface Ix32ReportTypeBL
    {
        public BO.x32ReportType Load(int pid);
        public IEnumerable<BO.x32ReportType> GetList(BO.myQuery mq);
        public int Save(BO.x32ReportType rec);


    }
    class x32ReportTypeBL : BaseBL, Ix32ReportTypeBL
    {
        public x32ReportTypeBL(BL.Factory mother) : base(mother)
        {

        }


        private string GetSQL1(string strAppend = null)
        {
            sb("SELECT a.*,dbo._core_x32_get_parent_inline(a.x32ID) as ParentPath,x32parent.x32Name as ParentName,");
            sb("case when a.x32TreeLevel>1 then replace(space(2*(a.x32TreeLevel-1)),' ','-') else '' END+a.x32Name as TreeItem,");
            sb(_db.GetSQL1_Ocas("x32"));
            sb(" FROM x32ReportType a LEFT OUTER JOIN x32ReportType x32parent ON a.x32ParentID=x32parent.x32ID");
            sb(strAppend);

            return sbret();
        }
        public BO.x32ReportType Load(int pid)
        {
            return _db.Load<BO.x32ReportType>(GetSQL1(" WHERE a.x32ID=@pid"), new { pid = pid });
        }

        public IEnumerable<BO.x32ReportType> GetList(BO.myQuery mq)
        {
            if (mq.explicit_orderby == null) { mq.explicit_orderby = "a.x32TreeIndex"; };
            DL.FinalSqlCommand fq = DL.basQuery.GetFinalSql(GetSQL1(), mq, _mother.CurrentUser);
            return _db.GetList<BO.x32ReportType>(fq.FinalSql, fq.Parameters);
        }



        public int Save(BO.x32ReportType rec)
        {
            if (!ValidateBeforeSave(rec))
            {
                return 0;
            }
            var p = new DL.Params4Dapper();
            p.AddInt("pid", rec.x32ID);
            p.AddInt("x32ParentID", rec.x32ParentID, true);
            p.AddString("x32Name", rec.x32Name);
            p.AddString("x32Description", rec.x32Description);
            p.AddInt("x32Ordinal", rec.x32Ordinal);

            int intPID = _db.SaveRecord("x32ReportType", p.getDynamicDapperPars(), rec);
            if (intPID > 0)
            {
                _db.RunSql("exec dbo._core_x32_recalc_tree");
            }

            return intPID;
        }

        public bool ValidateBeforeSave(BO.x32ReportType rec)
        {
            if (string.IsNullOrEmpty(rec.x32Name))
            {
                this.AddMessage("Chybí vyplnit [Název]."); return false;
            }

            if (rec.x32ParentID > 0)
            {
                var recParent = Load(rec.x32ParentID);
                if (rec.x32TreeIndexFrom <= recParent.x32TreeIndex && rec.x32TreeIndexTo >= recParent.x32TreeIndex)
                {
                    if (rec.x32TreeIndexFrom > 0 || rec.x32TreeIndexTo > 0 || recParent.x32TreeIndex > 0)
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
