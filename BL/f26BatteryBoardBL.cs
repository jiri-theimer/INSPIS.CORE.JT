using System;
using System.Collections.Generic;
using System.Text;

namespace BL
{
    public interface If26BatteryBoardBL
    {
        public BO.f26BatteryBoard Load(int pid);
        public IEnumerable<BO.f26BatteryBoard> GetList(BO.myQuery mq);
        public int Save(BO.f26BatteryBoard rec);


    }
    class f26BatteryBoardBL : BaseBL, If26BatteryBoardBL
    {
        public f26BatteryBoardBL(BL.Factory mother) : base(mother)
        {

        }


        private string GetSQL1(string strAppend = null)
        {
            sb("SELECT a.*,f18.f18Name,f18.f06ID,");
            sb(_db.GetSQL1_Ocas("f26"));
            sb(" FROM f26BatteryBoard a INNER JOIN f18FormSegment f18 ON a.f18ID=f18.f18ID");
            sb(strAppend);

            return sbret();
        }
        public BO.f26BatteryBoard Load(int pid)
        {
            return _db.Load<BO.f26BatteryBoard>(GetSQL1(" WHERE a.f26ID=@pid"), new { pid = pid });
        }

        public IEnumerable<BO.f26BatteryBoard> GetList(BO.myQuery mq)
        {
            if (mq.explicit_orderby == null) mq.explicit_orderby = "a.f26Ordinal";
            DL.FinalSqlCommand fq = DL.basQuery.GetFinalSql(GetSQL1(), mq, _mother.CurrentUser);
            return _db.GetList<BO.f26BatteryBoard>(fq.FinalSql, fq.Parameters);
        }



        public int Save(BO.f26BatteryBoard rec)
        {
            if (!ValidateBeforeSave(rec))
            {
                return 0;
            }
            var p = new DL.Params4Dapper();
            p.AddInt("pid", rec.f26ID);
            p.AddInt("f18ID", rec.f18ID, true);
            p.AddString("f26ColumnHeaders", rec.f26ColumnHeaders);
            p.AddInt("f26Ordinal", rec.f26Ordinal);                        
            p.AddString("f26Hint", rec.f26Hint);
            p.AddString("f26Name", rec.f26Name);
            p.AddString("f26SupportingText", rec.f26SupportingText);
            p.AddString("f26Description", rec.f26Description);
            if (rec.f26UC == null) { rec.f26UC = BO.BAS.GetGuid(); }
            p.AddString("f26UC", rec.f26UC);


            int intPID = _db.SaveRecord("f26BatteryBoard", p.getDynamicDapperPars(), rec);


            return intPID;
        }

        public bool ValidateBeforeSave(BO.f26BatteryBoard rec)
        {
            if (string.IsNullOrEmpty(rec.f26Name))
            {
                this.AddMessage("Chybí vyplnit [Název]."); return false;
            }
            if (rec.f18ID == 0)
            {
                this.AddMessage("Chybí vyplnit [Segment formuláře]."); return false;
            }
           
            return true;
        }

    }
}
