using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Security.Cryptography.Xml;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UI.Models;
using MimeKit;
using UI.Models.Record;

namespace UI.Controllers
{
    public class MailController : BaseController
    {
        public IActionResult MailBatchFramework(string batchguid, int limittoprecs, int statusid, string oper)
        {
            if (limittoprecs == 0) limittoprecs = 500;
            var v = new Models.MailBatchFramework() { BatchGuid = batchguid, LimitTopRecs = limittoprecs, QueryByStatusID = statusid, lisGUID = new List<string>() };
           
            var guids = Factory.MailBL.GetGuidsInQueque();
            if (guids.Count() > 0)
            {                
                foreach (var c in guids)
                {
                    v.lisGUID.Add(c.Value);
                }
            }

            if (!string.IsNullOrEmpty(v.BatchGuid))
            {
                if (!v.lisGUID.Contains(v.BatchGuid))
                {
                    v.lisGUID.Add(v.BatchGuid);
                }                                
            }
            if (v.lisGUID.Count() == 0)
            {
                return this.StopPage(false, Factory.tra("Momentálně neexistuje otevřená mail fronta."));
            }
            switch (oper)
            {
                case "stop":
                    Factory.MailBL.StopPendingMessagesInBatch(v.BatchGuid); //zastavit odesílání čekajících zpráv v dávce
                    break;
                case "restart":
                    Factory.MailBL.RestartMessagesInBatch(v.BatchGuid); //znovu zahájit odesílání čekajících zpráv v dávce
                    break;
            }

            var mq = new BO.myQuery("x40") { explicit_orderby = "a.x40MailID", explicit_sqlwhere = "a.x40BatchGuid='" + BO.BAS.GSS(v.BatchGuid) + "'" };
            v.lisX40 = Factory.MailBL.GetList(mq);
            v.TotalCountX40 = v.lisX40.Count();
            if (v.QueryByStatusID > 0)
            {

                v.lisX40 = v.lisX40.Where(p => (int)p.x40Status == v.QueryByStatusID);
            }
            if (v.LimitTopRecs > 0 && v.lisX40.Count() > v.LimitTopRecs)
            {
                v.lisX40 = v.lisX40.Take(v.LimitTopRecs);
            }
            v.RecA42 = Factory.a42QesBL.LoadByGuid(v.BatchGuid, 0);
            return View(v);
        }
        public IActionResult SendMail(int x40id,int b65id,int j02id,int x29id,int x40datapid,string param1)
        {
            if (x40datapid == 0) x40datapid = Factory.CurrentUser.j02ID;
            if (x29id == 0) x29id = 502;
            if (b65id == 0)
            {
                b65id = Factory.CBL.LoadUserParamInt("SendMail-" + x29id.ToString() + "-b65id");
            }

            var v = new Models.SendMailViewModel() { UploadGuid = BO.BAS.GetGuid(),b65ID=b65id,Param1=param1 };
            v.Rec = new BO.x40MailQueue();
            v.Rec.x29ID = x29id;
            v.Rec.x40DataPID = x40datapid;
            if (j02id > 0)
            {
                v.Rec.x40Recipient = Factory.j02PersonBL.Load(j02id).j02Email;
            }


            v.Rec.j40ID = BO.BAS.InInt(Factory.CBL.LoadUserParam("SendMail_j40ID"));
            v.Rec.j40Name = Factory.CBL.LoadUserParam("SendMail_j40Name");
            v.Rec.x40MessageGuid = BO.BAS.GetGuid();

            v.lisB65 = Factory.b65WorkflowMessageBL.GetList(new BO.myQuery("b65")).Where(p => p.x29ID == v.Rec.x29ID);
            if (v.b65ID > 0)
            {
                var sp = Inhale_MergeTemplate(v.b65ID, v.Rec.x40DataPID,v.Param1);               
                v.Rec.x40Body = sp.Value;
                v.Rec.x40Subject = sp.Key;
            }
           
            if (x40id > 0)
            {   //kopírování zprávy do nové podle vzoru x40id
                v.Rec = Factory.MailBL.LoadMessageByPid(x40id);
                v.Rec.x40Recipient = v.Rec.x40Recipient;
                v.Rec.x40Cc = v.Rec.x40Cc;
                v.Rec.x40Bcc = v.Rec.x40Bcc;
                v.Rec.x40Subject = v.Rec.x40Subject;
                v.Rec.x40Body = v.Rec.x40Body;

                var vtemp = new x40RecMessage();
                vtemp.Rec = v.Rec;
                InhaleMimeMessage(ref vtemp, v.UploadGuid);
                v.Rec.x40MessageGuid = BO.BAS.GetGuid();    //jednoznačný guid do nové zprávy

            }

            return View(v);
        }
        [HttpPost]
        public IActionResult SendMail(Models.SendMailViewModel v,string oper)
        {
            v.lisB65 = Factory.b65WorkflowMessageBL.GetList(new BO.myQuery("b65")).Where(p => p.x29ID == v.Rec.x29ID);
            
            if (ModelState.IsValid)
            {
                foreach (BO.o27Attachment c in Factory.o27AttachmentBL.GetTempFiles(v.UploadGuid))
                {
                    Factory.MailBL.AddAttachment(c.FullPath, c.o27OriginalFileName, c.o27ContentType);
                }

                //System.IO.File.AppendAllText("c:\\temp\\hovado.txt", "Try SendMessage: " + DateTime.Now.ToString()+", message: "+ v.Rec.x40Subject);
                BO.Result ret = new BO.Result(false);
                if (!string.IsNullOrEmpty(v.a03IDs))
                {   //poslat to vybraným institucím                    
                    var a03ids = BO.BAS.ConvertString2ListInt(v.a03IDs);
                    string strBatchGuid = BO.BAS.GetGuid();
                    if (v.IsTest)
                    {
                        strBatchGuid = "TEST" + strBatchGuid;
                    }
                    else
                    {
                        strBatchGuid = "REAL" + strBatchGuid;
                    }

                    foreach (int intA03ID in a03ids)
                    {
                        var recA03 = Factory.a03InstitutionBL.Load(intA03ID);
                        v.Rec.x40Recipient = recA03.a03Email;
                        v.Rec.x40MessageGuid = BO.BAS.GetGuid();
                        if (a03ids.Count() > 10)
                        {
                            //odesílat dávkově
                            v.Rec.x40BatchGuid = strBatchGuid;                            
                            v.Rec.x40Status = BO.x40StateFlag.InQueque;
                            Factory.MailBL.SaveX40(null, v.Rec);
                        }
                        else
                        {
                            //odeslat rovnou
                            ret = Factory.MailBL.SendMessage(v.Rec, v.IsTest);
                        }
                        
                    }   
                    if (a03ids.Count() > 10)
                    {
                        v.SetJavascript_CallOnLoad("/Mail/MailBatchFramework?batchguid=" + strBatchGuid);
                        return View(v);                        
                    }

                }
                else
                {
                    ret = Factory.MailBL.SendMessage(v.Rec, v.IsTest);
                }



                if (v.Rec.j40ID > 0)
                {
                    Factory.CBL.SetUserParam("SendMail_j40ID", v.Rec.j40ID.ToString());
                    Factory.CBL.SetUserParam("SendMail_j40Name", v.Rec.j40Name);
                }

                if (ret.Flag == BO.ResultEnum.Success)  //případná chybová hláška je již naplněná v BL vrstvě
                {
                    v.SetJavascript_CallOnLoad(v.Rec.pid);
                    return View(v);
                }


            }

            return View(v);
        }

        public BO.StringPair Inhale_MergeTemplate(int b65id,int datapid,string param1)
        {            
            var recB65 = Factory.b65WorkflowMessageBL.Load(b65id);              
            var dt = Factory.gridBL.GetList4MailMerge(recB65.x29Prefix, datapid);
            var cMerge = new BO.CLS.MergeContent();
            var ret = new BO.StringPair();            
            ret.Value = cMerge.GetMergedContent(recB65.b65MessageBody, dt).Replace("#param1",param1,StringComparison.OrdinalIgnoreCase).Replace("#password#",param1);
            ret.Key = cMerge.GetMergedContent(recB65.b65MessageSubject, dt).Replace("#param1", param1, StringComparison.OrdinalIgnoreCase);
            return ret;
        }
        public IActionResult Record(int pid)
        {
            var v = new Models.x40RecMessage();
            v.Rec = Factory.MailBL.LoadMessageByPid(pid);


            if (v.Rec == null)
            {
                return RecNotFound(v);
            }


            InhaleMimeMessage(ref v, v.Rec.x40MessageGuid);

            return View(v);
        }



        public ActionResult DownloadEmlFile(string guid)
        {
            var rec = Factory.MailBL.LoadMessageByGuid(guid);
            if (rec == null)
            {
                return this.NotFound(new x40RecMessage());

            }
            string fullPath = Factory.App.UploadFolder + "\\" + rec.x40EmlFolder + "\\" + rec.x40MessageGuid + ".eml";


            if (System.IO.File.Exists(fullPath))
            {
                Response.Headers["Content-Length"] = rec.x40EmlFileSize.ToString();
                Response.Headers["Content-Disposition"] = "inline; filename=poštovní_zpráva.eml";
                var fileContentResult = new FileContentResult(System.IO.File.ReadAllBytes(fullPath), "message/rfc822");

                return fileContentResult;
                //return File(System.IO.File.ReadAllBytes(fullPath), "message/rfc822", "poštovní_zpráva.eml");
            }
            else
            {
                return RedirectToAction("FileDownloadNotFound", "o23");
            }



        }


        private void InhaleMimeMessage(ref x40RecMessage v, string strDestGUID)
        {
            string fullPath = Factory.App.UploadFolder + "\\" + v.Rec.x40EmlFolder + "\\" + v.Rec.x40MessageGuid + ".eml";

            if (System.IO.File.Exists(fullPath) == false)
            {
                return;
            }

            v.MimeMessage = MimeMessage.Load(fullPath);
            v.MimeAttachments = new List<BO.StringPair>();

            foreach (var attachment in v.MimeMessage.Attachments)
            {
                if (attachment is MessagePart)
                {

                }
                else
                {
                    var part = (MimePart)attachment;
                    var fileName = part.FileName;
                    v.MimeAttachments.Add(new BO.StringPair() { Key = part.ContentType.MimeType, Value = fileName });

                    string strTempFullPath = this.Factory.App.TempFolder + "\\" + strDestGUID + "_" + fileName;
                    if (System.IO.File.Exists(strTempFullPath) == false)
                    {
                        string strInfoxFullPath = Factory.App.TempFolder + "\\" + strDestGUID + "_" + fileName + ".infox";
                        System.IO.File.WriteAllText(strInfoxFullPath, part.ContentType.MimeType + "|0| " + fileName + "|" + strDestGUID + "_" + fileName + "|" + strDestGUID + "|0||");



                        using (var fs = new FileStream(strTempFullPath, System.IO.FileMode.Create))
                        {
                            part.Content.DecodeTo(fs);  //uložit attachment soubor do tempu



                        }
                    }

                }


            }


        }
    }



}