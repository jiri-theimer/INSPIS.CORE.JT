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
        public IActionResult SendMail(int x40id)
        {
            var v = new Models.SendMailViewModel();
            v.Rec = new BO.x40MailQueue();
            v.Rec.x29ID = 502;
            v.Rec.x40DataPID = Factory.CurrentUser.j02ID;

            v.Rec.j40ID = BO.BAS.InInt(Factory.CBL.LoadUserParam("SendMail_j40ID"));
            v.Rec.j40Name = Factory.CBL.LoadUserParam("SendMail_j40Name");            
            v.Rec.x40MessageGuid= BO.BAS.GetGuid();
            v.UploadGuid = BO.BAS.GetGuid();

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
                InhaleMimeMessage(ref vtemp,v.UploadGuid);
                v.Rec.x40MessageGuid = BO.BAS.GetGuid();    //jednoznačný guid do nové zprávy

            }

            return View(v);
        }
        [HttpPost]        
        public IActionResult SendMail(Models.SendMailViewModel v)
        {
            if (ModelState.IsValid)
            {
                foreach(BO.o27Attachment c in Factory.o27AttachmentBL.GetTempFiles(v.UploadGuid))
                {
                    Factory.MailBL.AddAttachment(c.FullPath,c.o27OriginalFileName,c.o27ContentType);
                }

                //System.IO.File.AppendAllText("c:\\temp\\hovado.txt", "Try SendMessage: " + DateTime.Now.ToString()+", message: "+ v.Rec.x40Subject);
                BO.Result r = Factory.MailBL.SendMessage(v.Rec);
                if (v.Rec.j40ID > 0)
                {
                    Factory.CBL.SetUserParam("SendMail_j40ID", v.Rec.j40ID.ToString());
                    Factory.CBL.SetUserParam("SendMail_j40Name", v.Rec.j40Name);
                }
                
                if (r.Flag==BO.ResultEnum.Success)  //případná chybová hláška je již naplněná v BL vrstvě
                {
                    v.SetJavascript_CallOnLoad(v.Rec.pid);
                    return View(v);
                }
                

            }

            return View(v);
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
            string fullPath = Factory.App.UploadFolder+"\\"+rec.x40EmlFolder+"\\"+ rec.x40MessageGuid+".eml";
           
            
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


        private void InhaleMimeMessage(ref x40RecMessage v,string strDestGUID)
        {
            string fullPath = Factory.App.UploadFolder + "\\" + v.Rec.x40EmlFolder + "\\" + v.Rec.x40MessageGuid + ".eml";

            if (System.IO.File.Exists(fullPath)==false)
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