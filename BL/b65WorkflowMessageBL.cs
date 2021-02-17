using System;
using System.Collections.Generic;
using System.Text;

namespace BL
{
    public interface Ib65WorkflowMessageBL
    {
        public BO.b65WorkflowMessage Load(int pid);
        public BO.b65WorkflowMessage LoadByCode(string code, int pid_exclude);
        public IEnumerable<BO.b65WorkflowMessage> GetList(BO.myQuery mq);
        public int Save(BO.b65WorkflowMessage rec);

        public BO.b65WorkflowMessage MailMergeRecord(BO.b65WorkflowMessage recB65, int datapid, string param1);
        public BO.b65WorkflowMessage MailMergeRecord(int b65id, int datapid, string param1);
        public string GetLinkUrl(int x29id, int datapid, BO.a01Event recA01 = null);

    }
    class b65WorkflowMessageBL : BaseBL, Ib65WorkflowMessageBL
    {
        public b65WorkflowMessageBL(BL.Factory mother) : base(mother)
        {

        }


        private string GetSQL1(string strAppend = null)
        {
            sb("SELECT a.*,x29.x29Name,x29.x29Prefix,");
            sb(_db.GetSQL1_Ocas("b65"));
            sb(" FROM b65WorkflowMessage a LEFT OUTER JOIN x29Entity x29 ON a.x29ID=x29.x29ID");
            sb(strAppend);
            return sbret();
        }
        public BO.b65WorkflowMessage Load(int pid)
        {
            return _db.Load<BO.b65WorkflowMessage>(GetSQL1(" WHERE a.b65ID=@pid"), new { pid = pid });
        }
        public BO.b65WorkflowMessage LoadByCode(string code, int pid_exclude)
        {
            return _db.Load<BO.b65WorkflowMessage>(GetSQL1(" WHERE a.b65PID LIKE @code AND a.b65ID<>@pid_exclude"), new { code = code, pid_exclude = pid_exclude });
        }


        public IEnumerable<BO.b65WorkflowMessage> GetList(BO.myQuery mq)
        {
            if (mq.explicit_orderby == null) { mq.explicit_orderby = "a.b65Name"; };
            DL.FinalSqlCommand fq = DL.basQuery.GetFinalSql(GetSQL1(), mq, _mother.CurrentUser);
            return _db.GetList<BO.b65WorkflowMessage>(fq.FinalSql, fq.Parameters);
        }



        public int Save(BO.b65WorkflowMessage rec)
        {
            if (ValidateBeforeSave(rec) == false)
            {
                return 0;
            }
            var p = new DL.Params4Dapper();
            p.AddInt("pid", rec.b65ID);
            p.AddInt("x29ID", rec.x29ID, true);
            p.AddString("b65Name", rec.b65Name);
            p.AddString("b65MessageSubject", rec.b65MessageSubject);           
            p.AddString("b65MessageBody", rec.b65MessageBody);
            p.AddEnumInt("b65SystemFlag", rec.b65SystemFlag);
          
            int intPID = _db.SaveRecord("b65WorkflowMessage", p.getDynamicDapperPars(), rec);
           
            return intPID;
        }

        public bool ValidateBeforeSave(BO.b65WorkflowMessage rec)
        {
            if (string.IsNullOrEmpty(rec.b65Name))
            {
                this.AddMessage("Chybí vyplnit [Název]."); return false;
            }
            if (string.IsNullOrEmpty(rec.b65MessageSubject))
            {
                this.AddMessage("Chybí vyplnit [Předmět zprávy]."); return false;
            }

            if (rec.x29ID == 0)
            {
                this.AddMessage("Chybí vyplnit [Entita]."); return false;
            }
            


            return true;
        }

        public BO.b65WorkflowMessage MailMergeRecord(int b65id, int datapid, string param1)
        {
            if (b65id == 0) return null;
            return MailMergeRecord(Load(b65id), datapid, param1);
        }
        public BO.b65WorkflowMessage MailMergeRecord(BO.b65WorkflowMessage recB65, int datapid, string param1)
        {
            if (recB65 == null) return null;
            var dt = _mother.gridBL.GetList4MailMerge(recB65.x29Prefix, datapid);
            if (dt.Rows.Count == 0) return recB65;

            var cMerge = new BO.CLS.MergeContent();
            recB65.b65MessageBody = cMerge.GetMergedContent(recB65.b65MessageBody, dt).Replace("#param1", param1, StringComparison.OrdinalIgnoreCase).Replace("#password#", param1);
            recB65.b65MessageSubject = cMerge.GetMergedContent(recB65.b65MessageSubject, dt).Replace("#param1", param1, StringComparison.OrdinalIgnoreCase);
            if (recB65.b65MessageBody.Contains("#link#") && !string.IsNullOrEmpty(_mother.App.UserUrl))
            {

                recB65.b65MessageBody = recB65.b65MessageBody.Replace("#link#", GetLinkUrl(recB65.x29ID, datapid));
            }
            return recB65;
        }

        public string GetLinkUrl(int x29id,int datapid,BO.a01Event recA01=null)
        {
            string strURL = _mother.App.UserUrl;
            if (BO.BAS.RightString(strURL, 1) != "/")
            {
                strURL += "/";
            }
            switch (x29id)
            {
                case 101:
                    if (recA01 == null)
                    {
                        recA01 = _mother.a01EventBL.Load(datapid);
                    }
                   
                    var recA10 = _mother.a10EventTypeBL.Load(recA01.a10ID);
                    if (recA10.a10ViewUrl_Page != null)
                    {
                        if (recA10.a10ViewUrl_Page.Contains("/"))
                        {
                            strURL += recA10.a10ViewUrl_Page;
                        }
                        else
                        {
                            strURL += "a01/" + recA10.a10ViewUrl_Page;
                        }

                    }
                    else
                    {
                        strURL += "a01/RecPage";
                    }
                    strURL += "?pid=" + datapid.ToString();
                    break;
                case 103:
                    strURL += "a03/RecPage?pid=" + datapid.ToString();
                    break;
                case 502:
                    strURL += "j02/RecPage?pid=" + datapid.ToString();
                    break;
            }

            return strURL;
        }

    }
}
