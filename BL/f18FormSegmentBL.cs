using System;
using System.Collections.Generic;
using System.Text;

namespace BL
{
    public interface If18FormSegmentBL: BaseInterface
    {
        public BO.f18FormSegment Load(int pid);
        public IEnumerable<BO.f18FormSegment> GetList(BO.myQuery mq);
        public int Save(BO.f18FormSegment rec);


    }
    class f18FormSegmentBL : BaseBL, If18FormSegmentBL
    {
        public f18FormSegmentBL(BL.Factory mother) : base(mother)
        {

        }


        private string GetSQL1(string strAppend = null)
        {
            sb("SELECT a.*,dbo._core_f18_get_parent_inline(a.f18ID) as ParentPath,f18parent.f18Name as ParentName,");
            sb("case when a.f18TreeLevel>1 then replace(space(2*(a.f18TreeLevel-1)),' ','-') else '' END+a.f18Name as TreeItem,");
            sb(_db.GetSQL1_Ocas("f18"));
            sb(" FROM f18FormSegment a LEFT OUTER JOIN f18FormSegment f18parent ON a.f18ParentID=f18parent.f18ID");
            sb(strAppend);

            return sbret();
        }
        public BO.f18FormSegment Load(int pid)
        {
            return _db.Load<BO.f18FormSegment>(GetSQL1(" WHERE a.f18ID=@pid"), new { pid = pid });
        }

        public IEnumerable<BO.f18FormSegment> GetList(BO.myQuery mq)
        {
            if (mq.explicit_orderby == null) { mq.explicit_orderby = "a.f18TreeIndex"; };
            DL.FinalSqlCommand fq = DL.basQuery.ParseFinalSql(GetSQL1(), mq, _mother.CurrentUser);
            return _db.GetList<BO.f18FormSegment>(fq.FinalSql, fq.Parameters);
        }



        public int Save(BO.f18FormSegment rec)
        {
            if (!ValidateBeforeSave(rec))
            {
                return 0;
            }
            var p = new DL.Params4Dapper();
            p.AddInt("pid", rec.f18ID);
            p.AddInt("f06ID", rec.f06ID, true);
            p.AddInt("f18ParentID", rec.f18ParentID, true);
            p.AddString("f18Name", rec.f18Name);
            p.AddString("f18Text", rec.f18Text);
            p.AddString("f18SupportingText", rec.f18SupportingText);
            p.AddString("f18ReadonlyExpression", rec.f18ReadonlyExpression);
            p.AddString("f18RazorTemplate", rec.f18RazorTemplate);
            p.AddString("f18Description", rec.f18Description);
            p.AddInt("f18Ordinal", rec.f18Ordinal);
            if (rec.f18UC == null) { rec.f18UC = BO.BAS.GetGuid(); }
            p.AddString("f18UC", rec.f18UC);

            int intPID = _db.SaveRecord("f18FormSegment", p.getDynamicDapperPars(), rec);
            if (intPID > 0)
            {
                _db.RunSql("exec dbo._core_f18_recalc_tree @f06id",new { f06id = rec.f06ID });
            }

            return intPID;
        }

        public bool ValidateBeforeSave(BO.f18FormSegment rec)
        {
            if (string.IsNullOrEmpty(rec.f18Name))
            {
                this.AddMessage("Chybí vyplnit [Název]."); return false;
            }
            if (rec.f06ID==0)
            {
                this.AddMessage("Chybí vazba na formulář."); return false;
            }

            if (rec.f18ParentID > 0)
            {
                var recParent = Load(rec.f18ParentID);
                if (rec.f18TreeIndexFrom <= recParent.f18TreeIndex && rec.f18TreeIndexTo >= recParent.f18TreeIndex)
                {
                    if (rec.f18TreeIndexFrom > 0 || rec.f18TreeIndexTo > 0 || recParent.f18TreeIndex > 0)
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
