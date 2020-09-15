using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Mail;
using System.Linq;
using BO;
using System.IO;

namespace BL
{
    public interface IMailBL
    {
        public BO.Result SendMessage(int j40id, MailMessage message); //v Result.pid vrací x40id
        public BO.Result SendMessage(int j40id, string toEmail, string toName, string subject, string body, bool ishtml,int x29id,int recpid); //v Result.pid vrací x40id
        public BO.Result SendMessage(BO.x40MailQueue rec);  //v Result.pid vrací x40id
        public void AddAttachment(string fullpath, string displayname, string contenttype = null);
        public void AddAttachment(Attachment att);
        public BO.j40MailAccount LoadJ40(int pid);
        public BO.j40MailAccount LoadDefaultJ40();
        public IEnumerable<BO.j40MailAccount> GetListJ40();
        public int SaveJ40(BO.j40MailAccount rec);
        public BO.x40MailQueue LoadMessageByPid(int x40id);
        public BO.x40MailQueue LoadMessageByGuid(string guid);
        public bool SaveMailJob2Temp(string strJobGuid, BO.x40MailQueue recX40, string strUploadGuid, List<BO.x43MailQueue_Recipient> lisX43);
        public IEnumerable<BO.x40MailQueue> GetList(BO.myQuery mq);
        public int SaveX40(MailMessage m, BO.x40MailQueue rec);
        public void StopPendingMessagesInBatch(string batchguid);
        public void RestartMessagesInBatch(string batchguid);

    }
    class MailBL : BaseBL, IMailBL
    {
        private BO.j40MailAccount _account;
        public MailBL(BL.Factory mother) : base(mother)
        {

        }
        private List<Attachment> _attachments;


        private string GetSQL1(string strAppend = null)
        {
            sb("SELECT a.*,dbo._core_j02_get_fullname(a.j02ID) as Person,");
            sb(_db.GetSQL1_Ocas("j40"));
            sb(" FROM j40MailAccount a");
            sb(strAppend);
            return sbret();
        }       

        public BO.j40MailAccount LoadJ40(int pid)
        {
            return _db.Load<BO.j40MailAccount>(GetSQL1(" WHERE a.j40ID=@pid"), new { pid = pid });            
        }
        public BO.j40MailAccount LoadDefaultJ40()
        {
            return _db.Load<BO.j40MailAccount>(string.Format("{0} WHERE (a.j02ID=@j02id AND a.j40UsageFlag=1) OR a.j40UsageFlag=2 ORDER BY a.j40UsageFlag", GetSQL1()), new { j02id = _mother.CurrentUser.j02ID });
        }
        public IEnumerable<BO.j40MailAccount> GetListJ40()
        {
            return _db.GetList<BO.j40MailAccount>(GetSQL1());
        }
        
       
        public int SaveJ40(BO.j40MailAccount rec)
        {
            if (rec.pid==0 && rec.j40SmtpUseDefaultCredentials == false && string.IsNullOrEmpty(rec.j40SmtpPassword)==true)
            {
                this.AddMessage("Zadané nastavení vyžaduje zadat heslo.");return 0;
            }
            var p = new DL.Params4Dapper();

            p.AddInt("pid", rec.j40ID);
            if (rec.j02ID == 0) rec.j02ID = _db.CurrentUser.j02ID;
            p.AddInt("j02ID", rec.j02ID, true);
            p.AddInt("j40UsageFlag", (int)rec.j40UsageFlag);
            p.AddString("j40SmtpHost", rec.j40SmtpHost);
            p.AddInt("j40SmtpPort", rec.j40SmtpPort);
            p.AddString("j40SmtpName", rec.j40SmtpName);
            p.AddString("j40SmtpEmail", rec.j40SmtpEmail);
            p.AddString("j40SmtpLogin", rec.j40SmtpLogin);
            p.AddString("j40SmtpPassword", rec.j40SmtpPassword);
            p.AddBool("j40SmtpUseDefaultCredentials", rec.j40SmtpUseDefaultCredentials);
            p.AddBool("j40SmtpEnableSsl", rec.j40SmtpEnableSsl);
            p.AddBool("j40SmtpUsePersonalReply", rec.j40SmtpUsePersonalReply);
            p.AddString("j40ImapHost", rec.j40ImapHost);
            p.AddString("j40ImapLogin", rec.j40ImapLogin);
            p.AddString("j40ImapPassword", rec.j40ImapPassword);
            p.AddInt("j40ImapPort", rec.j40ImapPort);

            return _db.SaveRecord("j40MailAccount", p.getDynamicDapperPars(), rec);
        }

        private string GetSQLMessage(string strAppend = null)
        {
            sb("SELECT a.*,case when a.x40DatetimeProcessed is not null then a.x40DatetimeProcessed else a.x40DateInsert end as MessageTime,");
            sb(_db.GetSQL1_Ocas("x40", false, false));
            sb(" FROM x40MailQueue a");
            sb(strAppend);
            return sbret();
        }
        public BO.x40MailQueue LoadMessageByPid(int x40id)
        {
            sb(GetSQLMessage(" WHERE a.x40ID=@pid"));               
            return _db.Load<BO.x40MailQueue>(sbret(), new { pid = x40id });
        }
        public BO.x40MailQueue LoadMessageByGuid(string guid)
        {
            sb(GetSQLMessage(" WHERE a.x40MessageGuid=@guid"));            
            return _db.Load<BO.x40MailQueue>(sbret(), new { guid = guid });
        }

        public void AddAttachment(string fullpath,string displayname,string contenttype=null)
        {
            if (_attachments == null) _attachments = new List<Attachment>();
            var att = new Attachment(fullpath);
            att.Name = displayname;
            if (contenttype != null)
            {
                att.ContentType = new System.Net.Mime.ContentType(contenttype);
            }            
            _attachments.Add(att);
        }
        public void AddAttachment(Attachment att)
        {
            if (_attachments == null) _attachments = new List<Attachment>();
            _attachments.Add(att);
        }
        private BO.x40MailQueue InhaleMessageSender(int j40id,BO.x40MailQueue rec)
        {
            if (j40id > 0)
            {
                _account = LoadJ40(j40id);
            }
            else
            {
                _account = LoadDefaultJ40();                
            }
            if (_account == null)
            {
                return new BO.x40MailQueue() { j40ID = 0 };
            }
            rec.j40ID = _account.pid;
            rec.x40SenderAddress = _account.j40SmtpEmail;
            rec.x40SenderName = _account.j40SmtpName;            
            if (_account.j40SmtpUsePersonalReply)
            {
                //rec.x40SenderAddress = _mother.CurrentUser.j02Email;
                rec.x40SenderName = _mother.CurrentUser.FullName;
            }
                                    
            return rec;
        }
        public BO.Result SendMessage(int j40id, string toEmail, string toName, string subject, string body, bool ishtml, int x29id, int recpid)  //v BO.Result.pid vrací x40id
        {

            BO.x40MailQueue rec = new BO.x40MailQueue() { x40Recipient = toEmail, x40Subject = subject, x40Body = body, x40IsHtmlBody = ishtml,x40MessageGuid=BO.BAS.GetGuid(),x29ID=x29id,x40DataPID=recpid };
            rec = InhaleMessageSender(j40id,rec);
            return SendMessage(rec);
           
        }
        public BO.Result SendMessage(BO.x40MailQueue rec)  //v BO.Result.pid vrací x40id
        {
            rec = InhaleMessageSender(rec.j40ID, rec);            
            MailMessage m = new MailMessage() { Body = rec.x40Body, Subject = rec.x40Subject,IsBodyHtml=rec.x40IsHtmlBody};                        

            m.From = new MailAddress(rec.x40SenderAddress, rec.x40SenderName);
            var lis = new List<string>();
            if (String.IsNullOrEmpty(rec.x40Recipient) == false)
            {
                lis = BO.BAS.ConvertString2List(rec.x40Recipient.Replace(";", ","), ",");
                foreach (string s in lis)
                {
                    m.To.Add(new MailAddress(s));
                }
            }
            if (String.IsNullOrEmpty(rec.x40Cc) == false)
            {
                lis = BO.BAS.ConvertString2List(rec.x40Cc.Replace(";", ","), ",");
                foreach (string s in lis)
                {
                    m.CC.Add(new MailAddress(s));
                }
            }
            if (String.IsNullOrEmpty(rec.x40Bcc) == false)
            {
                lis = BO.BAS.ConvertString2List(rec.x40Bcc.Replace(";", ","), ",");
                foreach (string s in lis)
                {
                    m.Bcc.Add(new MailAddress(s));
                }
            }

           

            return handle_smtp_finish(m, rec);
        }
        public BO.Result SendMessage(int j40id, MailMessage message)
        {
            var rec = new BO.x40MailQueue();
            if (message.From == null)
            {
                rec = InhaleMessageSender(j40id, rec);
                message.From = new MailAddress(rec.x40SenderAddress, rec.x40SenderName);
            }
            return handle_smtp_finish(message,rec);
        }


        private BO.Result handle_smtp_finish(MailMessage m,BO.x40MailQueue rec)     //finální odeslání zprávy
        {
            if (_account == null)
            {
                return handle_result_error("Chybí poštovní účet odesílatele");
            }
            if (m.From == null)
            {
                return handle_result_error( "Chybí odesílatel zprávy");
            }
            if (m.To.Count == 0)
            {
                return handle_result_error("Chybí příjemce zprávy");
            }
            if (string.IsNullOrEmpty(m.Body) == true)
            {
                return handle_result_error("Chybí text zprávy.");
            }
            if (string.IsNullOrEmpty(m.Subject) == true)
            {
                return handle_result_error("Chybí předmět zprávy.");
            }
           
            if (_account.j40SmtpUsePersonalReply)
            {
                m.ReplyToList.Add(new MailAddress(_mother.CurrentUser.j02Email, _mother.CurrentUser.FullName));
            }

            BO.Result ret = new BO.Result(false);
            
            using (SmtpClient client = new SmtpClient(_account.j40SmtpHost, _account.j40SmtpPort))
            {
                client.UseDefaultCredentials = _account.j40SmtpUseDefaultCredentials;
                if (client.UseDefaultCredentials == false)
                {                    
                    client.Credentials = new System.Net.NetworkCredential(_account.j40SmtpLogin, _account.j40SmtpPassword);
                }


                m.BodyEncoding = Encoding.UTF8;
                m.SubjectEncoding = Encoding.UTF8;
                m.Headers.Add("Message-ID", rec.x40MessageGuid);


                if (_attachments != null)
                {
                    foreach (var att in _attachments)
                    {
                        m.Attachments.Add(att);
                    }
                }
                
                
                client.DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory;    
                client.PickupDirectoryLocation = _mother.App.TempFolder;                
                client.Send(m);//nejdříve uložit eml soubor do temp složky
                string strFullPath = FindEmlFileByGuid(rec.x40MessageGuid); //najít vygenerovaný eml file podle jeho Message-ID
                if (strFullPath != "")
                {
                    rec.x40EmlFolder = "eml\\"+DateTime.Now.Year.ToString() + "\\" + DateTime.Now.Month.ToString();
                    rec.x40EmlFileSize =(int)(new System.IO.FileInfo(strFullPath).Length);
                    if (!System.IO.Directory.Exists(_mother.App.UploadFolder + "\\" + rec.x40EmlFolder))
                    {
                        System.IO.Directory.CreateDirectory(_mother.App.UploadFolder + "\\" + rec.x40EmlFolder);
                    }
                    string strDestPath = _mother.App.UploadFolder + "\\" + rec.x40EmlFolder + "\\" + rec.x40MessageGuid + ".eml";
                    if (!File.Exists(strDestPath))
                    {
                        File.Move(strFullPath, strDestPath);    //přejmenovat nalezený eml file na guid
                    }
                    
                    
                }
                client.DeliveryMethod = SmtpDeliveryMethod.Network; //nyní opravdu odeslat
                
                try
                {
                    client.Send(m);
                    rec.x40ErrorMessage = "";
                    rec.x40Status = BO.x40StateFlag.Proceeded;
                    ret.pid = SaveX40(m, rec);
                    ret.Flag = ResultEnum.Success;

                }
                catch (Exception ex)
                {

                    this.AddMessageTranslated(ex.Message);
                    rec.x40ErrorMessage = ex.Message;
                    rec.x40Status = BO.x40StateFlag.Error;
                    ret.pid = SaveX40(m, rec);
                    ret.Flag = ResultEnum.Failed;
                    ret.Message = rec.x40ErrorMessage;
                }

                
            }

            return ret;


        }

        public void StopPendingMessagesInBatch(string batchguid)
        {
            _db.RunSql("UPDATE x40MailQueue set x40Status=4 WHERE x40BatchGuid=@guid AND x40Status=1", new {guid = batchguid });
            _db.RunSql("UPDATE a42Qes set a42JobState=7 WHERE a42JobGuid=@guid", new { guid = batchguid }); //doplňkově nastavit stav pro INEZ
        }
        public void RestartMessagesInBatch(string batchguid)
        {
            _db.RunSql("UPDATE x40MailQueue set x40Status=1 WHERE x40BatchGuid=@guid AND x40Status=4", new { guid = batchguid });
            _db.RunSql("UPDATE a42Qes set a42JobState=6 WHERE a42JobGuid=@guid", new { guid = batchguid }); //doplňkově nastavit stav pro INEZ
        }

        public int SaveX40(MailMessage m,BO.x40MailQueue rec)
        {            
            var p = new DL.Params4Dapper();
            p.AddInt("pid", rec.x40ID);
            if (string.IsNullOrEmpty(rec.x40MessageGuid) == true)
            {
                rec.x40MessageGuid = BO.BAS.GetGuid();
            }
            p.AddString("x40MessageGuid", rec.x40MessageGuid);
            p.AddString("x40BatchGuid", rec.x40BatchGuid);
            p.AddString("x40AttachmentsGuid", rec.x40AttachmentsGuid);
            p.AddInt("j40ID", rec.j40ID, true);
            p.AddInt("x29ID", rec.x29ID, true);
            if (rec.j03ID_Creator == 0) rec.j03ID_Creator = _mother.CurrentUser.pid;
            p.AddInt("j03ID_Creator", rec.j03ID_Creator, true);
            p.AddInt("x40DataPID", rec.x40DataPID, true);
            p.AddInt("x40MailID", rec.x40MailID, true);
            if (m != null)
            {
                p.AddString("x40SenderAddress", m.From.Address);
                p.AddString("x40SenderName", m.From.DisplayName);
                p.AddString("x40Recipient", String.Join(",", m.To.Select(p => p.Address)));
                p.AddString("x40Bcc", String.Join(",", m.Bcc.Select(p => p.Address)));
                p.AddString("x40Cc", String.Join(",", m.CC.Select(p => p.Address)));
                p.AddString("x40Subject", m.Subject);
                p.AddString("x40Body", m.Body);
                p.AddBool("x40IsHtmlBody", m.IsBodyHtml);
                p.AddString("x40Attachments", String.Join(",", m.Attachments.Select(p => p.Name)));
            }
            else
            {
                p.AddString("x40SenderAddress", rec.x40SenderAddress);
                p.AddString("x40SenderName", rec.x40SenderName);
                p.AddString("x40Recipient", rec.x40Recipient);
                p.AddString("x40Bcc", rec.x40Bcc);
                p.AddString("x40Cc", rec.x40Cc);
                p.AddString("x40Subject", rec.x40Subject);
                p.AddString("x40Body", rec.x40Body);
                p.AddBool("x40IsHtmlBody", rec.x40IsHtmlBody);
                p.AddString("x40Attachments", rec.x40Attachments);
            }
            
            p.AddBool("x40IsProcessed", rec.x40IsProcessed);            
            p.AddDateTime("x40DatetimeProcessed", rec.x40DatetimeProcessed);
            p.AddString("x40ErrorMessage", rec.x40ErrorMessage);
            p.AddEnumInt("x40Status", rec.x40Status);
            
            p.AddString("x40EmlFolder", rec.x40EmlFolder);
            p.AddInt("x40EmlFileSize", rec.x40EmlFileSize);
            
            return _db.SaveRecord("x40MailQueue", p.getDynamicDapperPars(), rec,false);
        }

        private BO.Result handle_result_error(string strError)
        {
            this.AddMessageTranslated(strError);
            return new BO.Result(true, strError);
        }

        private string FindEmlFileByGuid(string strGUID)
        {

            DirectoryInfo dir = new DirectoryInfo(_mother.App.TempFolder);
            
            foreach (FileInfo file in dir.GetFiles("*.eml").OrderByDescending(p => p.CreationTime))
            {
                StreamReader reader = file.OpenText();
                string s = "";
                while ((s = reader.ReadLine()) !=null)
                {
                    if (s.Contains("Message-ID"))
                    {
                        if (s.Contains(strGUID))
                        {
                            reader.Close();
                            return file.FullName;                           
                        }
                        reader.Close();
                        break;
                    }                    
                }

            }

            return "";
        }

        public bool SaveMailJob2Temp(string strJobGuid,BO.x40MailQueue recX40,string strUploadGuid, List<BO.x43MailQueue_Recipient> lisX43)
        {
            var recTemp = new BO.p85Tempbox() { p85Prefix = "x40", p85GUID = strJobGuid, p85FreeText01 = recX40.x40Subject,p85Message=recX40.x40Body,p85FreeText04=strUploadGuid };
            if (_mother.p85TempboxBL.Save(recTemp) == 0)
            {
                return false;
            }
            foreach (var c in lisX43)
            {
                recTemp = new BO.p85Tempbox() { p85Prefix = "x43", p85GUID = strJobGuid, p85FreeText01 = c.x43Email, p85FreeText02 = c.x43DisplayName, p85OtherKey1 = c.x29ID,p85OtherKey2=c.x43DataPID, p85OtherKey3=c.x43RecipientType, p85OtherKey4=c.TempA03ID };
                _mother.p85TempboxBL.Save(recTemp);
              
            }

            return true;
        }

        public IEnumerable<BO.x40MailQueue> GetList(BO.myQuery mq)
        {
            DL.FinalSqlCommand fq = DL.basQuery.ParseFinalSql("SELECT a.*,"+ _db.GetSQL1_Ocas("x40",false,false,true)+" FROM x40MailQueue a", mq, _mother.CurrentUser);
            return _db.GetList<BO.x40MailQueue>(fq.FinalSql, fq.Parameters);
        }

    }
}
