using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BL
{
    public interface Ix31ReportBL
    {
        public BO.x31Report Load(int pid);
        public BO.x31Report LoadByCode(string code, int pid_exclude);
        public IEnumerable<BO.x31Report> GetList(BO.myQueryX31 mq);
        public int Save(BO.x31Report rec, List<int> x32ids, List<int> j04ids, List<int> a10ids, List<int> a08ids);
        public BO.o27Attachment LoadReportDoc(int x31id);
        public IEnumerable<BO.x34Report_Category> GetListX34();

    }
    class x31ReportBL : BaseBL, Ix31ReportBL
    {
        public x31ReportBL(BL.Factory mother) : base(mother)
        {

        }


        private string GetSQL1(string strAppend = null)
        {
            sb("SELECT a.*,x29.x29Name,");
            sb(_db.GetSQL1_Ocas("x31"));
            sb(" FROM x31Report a LEFT OUTER JOIN x29Entity x29 ON a.x29ID=x29.x29ID");
            sb(strAppend);
            return sbret();
        }
        public BO.x31Report Load(int pid)
        {
            return _db.Load<BO.x31Report>(GetSQL1(" WHERE a.x31ID=@pid"), new { pid = pid });
        }
        public BO.x31Report LoadByCode(string code, int pid_exclude)
        {
            return _db.Load<BO.x31Report>(GetSQL1(" WHERE a.x31PID LIKE @code AND a.x31ID<>@pid_exclude"), new { code = code, pid_exclude = pid_exclude });
        }


        public IEnumerable<BO.x31Report> GetList(BO.myQueryX31 mq)
        {
            if (mq.explicit_orderby == null) { mq.explicit_orderby = "a.x31Name"; };
            DL.FinalSqlCommand fq = DL.basQuery.GetFinalSql(GetSQL1(), mq, _mother.CurrentUser);
            return _db.GetList<BO.x31Report>(fq.FinalSql, fq.Parameters);
        }

        public IEnumerable<BO.x34Report_Category> GetListX34()
        {
                        
            return _db.GetList<BO.x34Report_Category>("select * from x34Report_Category");
        }

        public int Save(BO.x31Report rec, List<int> x32ids, List<int> j04ids, List<int> a10ids, List<int> a08ids)
        {
            if (ValidateBeforeSave(rec) == false)
            {
                return 0;
            }
            var p = new DL.Params4Dapper();
            p.AddInt("pid", rec.x31ID);
            p.AddInt("x29ID", rec.x29ID, true);
            p.AddString("x31Name", rec.x31Name);
            p.AddString("x31PID", rec.x31PID);
            p.AddEnumInt("x31ReportFormat", rec.x31ReportFormat);
            p.AddString("x31Description", rec.x31Description);
            p.AddBool("x31Is4SingleRecord", rec.x31Is4SingleRecord);
            p.AddString("x31DocSqlSource", rec.x31DocSqlSource);
            p.AddString("x31Translate", rec.x31Translate);
            if (a10ids != null)
            {
                if (a10ids.Count == 0)
                {
                    p.AddBool("x31IsAllA10IDs", true);
                }
                else
                {
                    p.AddBool("x31IsAllA10IDs", false);
                }
            }
            if (a08ids != null)
            {
                if (a08ids.Count == 0)
                {
                    p.AddBool("x31IsAllA08IDs", true);
                }
                else
                {
                    p.AddBool("x31IsAllA08IDs", false);
                }
            }

            int intPID = _db.SaveRecord("x31Report", p, rec);
            if (x32ids != null)
            {
                if (rec.pid > 0)
                {
                    _db.RunSql("DELETE FROM x34Report_Category WHERE x31ID=@pid", new { pid = intPID });
                }
                if (x32ids.Count > 0)
                {
                    _db.RunSql("INSERT INTO x34Report_Category(x31ID,x32ID) SELECT @pid,x32ID FROM x32ReportType WHERE x32ID IN (" + string.Join(",", x32ids) + ")", new { pid = intPID });
                }
            }
            if (j04ids != null)
            {
                if (rec.pid > 0)
                {
                    _db.RunSql("DELETE FROM x37ReportRestriction_UserRole WHERE x31ID=@pid", new { pid = intPID });
                }
                if (j04ids.Count > 0)
                {
                    _db.RunSql("INSERT INTO x37ReportRestriction_UserRole(x31ID,j04ID) SELECT @pid,j04ID FROM j04UserRole WHERE j04ID IN (" + string.Join(",", j04ids) + ")", new { pid = intPID });
                }
            }
            if (a10ids != null)
            {
                if (rec.pid > 0)
                {
                    _db.RunSql("DELETE FROM a23EventType_Report WHERE x31ID=@pid", new { pid = intPID });
                }
                if (a10ids.Count > 0)
                {
                    _db.RunSql("INSERT INTO a23EventType_Report(x31ID,a10ID) SELECT @pid,a10ID FROM a10EventType WHERE a10ID IN (" + string.Join(",", a10ids) + ")", new { pid = intPID });
                }
            }
            if (a08ids != null)
            {
                if (rec.pid > 0)
                {
                    _db.RunSql("DELETE FROM a27EventTheme_Report WHERE x31ID=@pid", new { pid = intPID });
                }
                if (a08ids.Count > 0)
                {
                    _db.RunSql("INSERT INTO a27EventTheme_Report(x31ID,a08ID) SELECT @pid,a08ID FROM a08Theme WHERE a08ID IN (" + string.Join(",", a08ids) + ")", new { pid = intPID });
                }
            }


            return intPID;
        }

        public bool ValidateBeforeSave(BO.x31Report rec)
        {
            if (string.IsNullOrEmpty(rec.x31Name))
            {
                this.AddMessage("Chybí vyplnit [Název sestavy]."); return false;
            }
            if (string.IsNullOrEmpty(rec.x31PID))
            {
                this.AddMessage("Chybí vyplnit [Kód sestavy]."); return false;
            }
            if (rec.x29ID == 0)
            {
                this.AddMessage("Chybí vyplnit [Entita]."); return false;
            }
            if (rec.x31ReportFormat == BO.x31ReportFormatEnum.DOC && rec.x31Is4SingleRecord == false)
            {
                this.AddMessage("Sestavy formátu DOCX mohou být pouze kontextové k vybranému záznamu"); return false;
            }
            if (rec.x31ReportFormat == BO.x31ReportFormatEnum.MSREPORTING)
            {
                if (string.IsNullOrEmpty(rec.x31MSReporting_ReportServerUrl))
                {
                    this.AddMessage("Chybí zadat [Report Server Url]."); return false;
                }
                if (string.IsNullOrEmpty(rec.x31MSReporting_ReportPath))
                {
                    this.AddMessage("Chybí zadat [Report soubor]."); return false;
                }

            }
            if (LoadByCode(rec.x31PID, rec.pid) != null)
            {
                this.AddMessageTranslated(string.Format(_mother.tra("V systému existuje jiná sestava s kódem: {0}."), rec.x31PID)); return false;
            }


            return true;
        }

        public BO.o27Attachment LoadReportDoc(int x31id)
        {
            var mq = new BO.myQueryO27() { recpid = x31id, x29id = 931 };            
            var lisO27 = _mother.o27AttachmentBL.GetList(mq, null);

            if (lisO27.Count() > 0)
            {
                return lisO27.First();
            }

            return null;
        }

    }
}
