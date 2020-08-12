using System;
using System.Collections.Generic;
using System.Text;

namespace BL
{
    public interface If29PortalQuestionTabBL
    {
        public BO.f29PortalQuestionTab Load(int pid);
        public IEnumerable<BO.f29PortalQuestionTab> GetList(BO.myQuery mq);
        public int Save(BO.f29PortalQuestionTab rec, List<int> a17ids);

    }
    class f29PortalQuestionTabBL : BaseBL, If29PortalQuestionTabBL
    {
        public f29PortalQuestionTabBL(BL.Factory mother) : base(mother)
        {

        }


        private string GetSQL1(string strAppend = null)
        {
            sb("SELECT a.*,");
            sb(_db.GetSQL1_Ocas("f29"));
            sb(" FROM f29PortalQuestionTab a");
            sb(strAppend);
            return sbret();
        }
        public BO.f29PortalQuestionTab Load(int pid)
        {
            return _db.Load<BO.f29PortalQuestionTab>(GetSQL1(" WHERE a.f29ID=@pid"), new { pid = pid });
        }

        public IEnumerable<BO.f29PortalQuestionTab> GetList(BO.myQuery mq)
        {
            if (mq.explicit_orderby == null) { mq.explicit_orderby = "a.f29Ordinal"; };
            DL.FinalSqlCommand fq = DL.basQuery.ParseFinalSql(GetSQL1(), mq, _mother.CurrentUser);
            return _db.GetList<BO.f29PortalQuestionTab>(fq.FinalSql, fq.Parameters);
        }



        public int Save(BO.f29PortalQuestionTab rec, List<int> a17ids)
        {
            if (!ValidateBeforeSave(rec,a17ids))
            {
                return 0;
            }
            var p = new DL.Params4Dapper();
            p.AddInt("pid", rec.f29ID);
            p.AddString("f29Name", rec.f29Name);
            p.AddString("f29Description", rec.f29Description);
            p.AddInt("f29Ordinal", rec.f29Ordinal);
            p.AddBool("f29IsSeparatePortalBox", rec.f29IsSeparatePortalBox);

            int intPID = _db.SaveRecord("f29PortalQuestionTab", p.getDynamicDapperPars(), rec);
            if (rec.pid > 0)
            {
                _db.RunSql("DELETE FROM f41PortalQuestionTab_a17Binding WHERE f29ID=@pid", new { pid = intPID });
            }
            if (a17ids.Count > 0)
            {
                _db.RunSql("INSERT INTO f41PortalQuestionTab_a17Binding(f29ID,a17ID) SELECT @pid,a17ID FROM a17DepartmentType WHERE a17ID IN (" + string.Join(",", a17ids) + ")", new { pid = intPID });
            }


            return intPID;
        }
        public bool ValidateBeforeSave(BO.f29PortalQuestionTab rec, List<int> a17ids)
        {
            if (string.IsNullOrEmpty(rec.f29Name))
            {
                this.AddMessage("Chybí vyplnit [Název]."); return false;
            }
            if (a17ids.Count == 0)
            {
                this.AddMessage("Musíte vybrat minimálně jeden typ činnosti."); return false;
            }

            return true;
        }

    }
}
