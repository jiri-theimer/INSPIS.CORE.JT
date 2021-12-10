using System;
using System.Collections.Generic;
using System.Text;

namespace BL
{
    public interface If21ReplyUnitBL: BaseInterface
    {
        public BO.f21ReplyUnit Load(int pid);
        public IEnumerable<BO.f21ReplyUnit> GetList(BO.myQueryF21 mq);
        public IEnumerable<BO.f21ReplyUnitJoinedF19> GetListJoinedF19(BO.myQueryXX1 mq);
        public int Save(BO.f21ReplyUnit rec);


    }
    class f21ReplyUnitBL : BaseBL, If21ReplyUnitBL
    {
        public f21ReplyUnitBL(BL.Factory mother) : base(mother)
        {

        }


        private string GetSQL1(string strAppend = null)
        {
            sb("SELECT a.*,");
            sb(_db.GetSQL1_Ocas("f21"));
            sb(" FROM f21ReplyUnit a");
            sb(strAppend);

            return sbret();
        }
        public BO.f21ReplyUnit Load(int pid)
        {
            return _db.Load<BO.f21ReplyUnit>(GetSQL1(" WHERE a.f21ID=@pid"), new { pid = pid });
        }

        public IEnumerable<BO.f21ReplyUnit> GetList(BO.myQueryF21 mq)
        {
            if (mq.explicit_orderby == null)
            {
                mq.explicit_orderby = "a.f21Ordinal";
            }
            DL.FinalSqlCommand fq = DL.basQuery.GetFinalSql(GetSQL1(), mq, _mother.CurrentUser);
            return _db.GetList<BO.f21ReplyUnit>(fq.FinalSql, fq.Parameters);
        }
        public IEnumerable<BO.f21ReplyUnitJoinedF19> GetListJoinedF19(BO.myQueryXX1 mq)
        {
            if (mq.explicit_orderby == null)
            {
                mq.explicit_orderby = "a.f21Ordinal";
            }
            sb("SELECT a.*,f20.f19ID,f19.f18ID,f19.f19Name,f19.f23ID,f19.f19StatID,f19.f19IsMultiselect,f18.f18Name,f26.f26Name,f18.f06ID,f06.f06Name,f19.f19SupportingText,");
            sb(_db.GetSQL1_Ocas("f21"));
            sb(" FROM f21ReplyUnit a");
            sb(" INNER JOIN f20ReplyUnitToQuestion f20 ON a.f21ID=f20.f21ID INNER JOIN f19Question f19 ON f20.f19ID=f19.f19ID INNER JOIN f18FormSegment f18 ON f19.f18ID=f18.f18ID");
            sb(" INNER JOIN f06Form f06 ON f18.f06ID=f06.f06ID");
            sb(" LEFT OUTER JOIN f26BatteryBoard f26 ON f19.f26ID=f26.f26ID");

           
            DL.FinalSqlCommand fq = DL.basQuery.GetFinalSql(sbret(), mq, _mother.CurrentUser);
            
            return _db.GetList<BO.f21ReplyUnitJoinedF19>(fq.FinalSql, fq.Parameters);
        }


        public int Save(BO.f21ReplyUnit rec)
        {
            if (!ValidateBeforeSave(rec))
            {
                return 0;
            }
            var p = new DL.Params4Dapper();
            p.AddInt("pid", rec.f21ID);            
            p.AddString("f21Name", rec.f21Name);
            p.AddString("f21ExportValue", rec.f21ExportValue);
            p.AddInt("f21Ordinal", rec.f21Ordinal);
            p.AddString("f21MinValue", rec.f21MinValue);
            p.AddString("f21MaxValue", rec.f21MaxValue);                        
            p.AddString("f21Description", rec.f21Description);
            p.AddBool("f21IsNegation", rec.f21IsNegation);
            p.AddBool("f21IsCommentAllowed", rec.f21IsCommentAllowed);
            if (rec.f21UC == null) { rec.f21UC = BO.BAS.GetGuid(); }
            p.AddString("f21UC", rec.f21UC);


            int intPID = _db.SaveRecord("f21ReplyUnit", p, rec);


            return intPID;
        }

        public bool ValidateBeforeSave(BO.f21ReplyUnit rec)
        {
            if (string.IsNullOrEmpty(rec.f21Name))
            {
                this.AddMessage("Chybí vyplnit [Název]."); return false;
            }
            if (rec.f21Name.Length > 150)
            {
                this.AddMessage("Název jednotky může mít maximálně 150 znaků.");return false;
            }
           
            return true;
        }

    }
}
