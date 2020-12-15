using System;
using System.Collections.Generic;
using System.Text;

namespace BL
{
    public interface Ih11NoticeBoardBL
    {
        public BO.h11NoticeBoard Load(int pid);
        public string LoadHtmlContent(int pid);
        public IEnumerable<BO.h11NoticeBoard> GetList(BO.myQuery mq);
        public int Save(BO.h11NoticeBoard rec, List<int> j04ids, string strHtml, string strPlanText);


    }
    class h11NoticeBoardBL : BaseBL, Ih11NoticeBoardBL
    {
        public h11NoticeBoardBL(BL.Factory mother) : base(mother)
        {

        }


        private string GetSQL1(string strAppend = null)
        {
            sb("SELECT a.*,");
            sb(_db.GetSQL1_Ocas("h11"));
            sb(" FROM h11NoticeBoard a");
            sb(strAppend);
            return sbret();
        }
        public BO.h11NoticeBoard Load(int pid)
        {
            return _db.Load<BO.h11NoticeBoard>(GetSQL1(" WHERE a.h11ID=@pid"), new { pid = pid });
        }
        public string LoadHtmlContent(int pid)
        {
            return _db.Load<BO.GetString>("SELECT o11Html as Value FROM o11BigtextContent WHERE x29ID=311 AND o11DataPID=@pid", new { pid = pid }).Value;
        }
        public IEnumerable<BO.h11NoticeBoard> GetList(BO.myQuery mq)
        {
            if (mq.explicit_orderby == null) mq.explicit_orderby = "a.H11ID DESC";
            DL.FinalSqlCommand fq = DL.basQuery.GetFinalSql(GetSQL1(), mq, _mother.CurrentUser);
            return _db.GetList<BO.h11NoticeBoard>(fq.FinalSql, fq.Parameters);
        }



        public int Save(BO.h11NoticeBoard rec, List<int> j04ids,string strHtml,string strPlanText)
        {
            if (ValidateBeforeSave(rec) == false)
            {
                return 0;
            }
            int intPID = 0;
            using (var sc = new System.Transactions.TransactionScope())
            {   //jedna transakce
                var p = new DL.Params4Dapper();
                p.AddInt("pid", rec.h11ID);
                p.AddString("h11Name", rec.h11Name);
                if (j04ids.Count > 0)
                {
                    p.AddBool("h11IsPublic", false);
                }
                else
                {
                    p.AddBool("h11IsPublic", true);
                }
                
                if (rec.j02ID_Creator == 0)
                {
                    rec.j02ID_Creator = _mother.CurrentUser.j02ID;
                }
                p.AddInt("j02ID_Creator", rec.j02ID_Creator, true);

                intPID = _db.SaveRecord("h11NoticeBoard", p.getDynamicDapperPars(), rec);

                if (intPID > 0)
                {
                    p = new DL.Params4Dapper();
                    p.AddInt("pid", Load(intPID).o11ID);
                    p.AddInt("x29ID", 311);
                    p.AddInt("o11DataPID", intPID);
                    p.AddString("o11Name", rec.h11Name);
                    p.AddString("o11Html", strHtml);
                    p.AddString("o11PlainText", strPlanText);
                    int intO11ID = _db.SaveRecord("o11BigtextContent", p.getDynamicDapperPars(), rec);
                    _db.RunSql("UPDATE h11NoticeBoard set o11ID=@o11id WHERE h11ID=@pid", new { o11id = intO11ID, pid = intPID });
                }
                if (rec.pid > 0)
                {
                    _db.RunSql("DELETE FROM h12NoticeBoard_Permission WHERE h11ID=@pid", new { pid = intPID });
                }
                if (j04ids.Count > 0)
                {
                    _db.RunSql("INSERT INTO h12NoticeBoard_Permission(h11ID,j04ID) SELECT @pid,j04ID FROM j04UserRole WHERE j04ID IN (" + string.Join(",", j04ids) + ")", new { pid = intPID });
                }
                sc.Complete();   //potvrzení transakce
            }
            
            return intPID;
        }

        public bool ValidateBeforeSave(BO.h11NoticeBoard rec)
        {
            if (string.IsNullOrEmpty(rec.h11Name))
            {
                this.AddMessage("Chybí vyplnit [Název]."); return false;
            }


            return true;
        }

    }
}
