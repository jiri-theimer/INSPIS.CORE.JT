using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Net.Http.Headers;
using UI.Models;

namespace UI.Controllers
{

    public class FileUploadController : BaseController
    {
        public IActionResult Index(string guid, int x29id, string prefix, int o13id, int recpid)
        {
            var v = new FileUploadViewModel() { Guid = guid, x29ID = x29id, o13ID = o13id,RecPid=recpid };
            if (prefix != null)
            {
                var ce = Factory.EProvider.ByPrefix(prefix);
                if (ce == null)
                {
                    return this.StopPageSubform("prefix not found");
                }
                if (ce.x29ID == 0)
                {
                    return this.StopPageSubform("prefix: " + prefix + ", x29ID = 0");
                }
                v.x29ID = ce.x29ID;
            }
            if (v.RecPid > 0)
            {
                var mq = new BO.myQueryO27() { x29id = v.x29ID,recpid=v.RecPid };
                v.lisO27 = Factory.o27AttachmentBL.GetList(mq,v.Guid);
                
            }
            if (Factory.o27AttachmentBL.GetTempFiles(guid).Count() > 0)
            {
                return RedirectToAction("DoUpload", new { guid = v.Guid, x29id = v.x29ID, o13id = v.o13ID,recpid=v.RecPid });
            }
            else
            {                                
                return View(v);
            }
            
        }
        public IActionResult SingleUpload(string guid,string targetflag)
        {
            if (string.IsNullOrEmpty(guid) == true)
            {
                return this.StopPageSubform("guid missing");
            }
            var v = new FileUploadSingleViewModel() { Guid = guid,TargetFlag=targetflag };
            v.lisTempFiles = Factory.o27AttachmentBL.GetTempFiles(v.Guid);
            return View(v);
        }
        [HttpPost]
        public async Task<IActionResult> SingleUpload(FileUploadSingleViewModel v, List<IFormFile> files)
        {

            var tempDir = Factory.App.TempFolder + "\\";

            foreach (var formFile in files)
            {
                if (formFile.Length > 0)
                {
                    // full path to file in temp location
                    //var strTempFullPath = Path.GetTempFileName();
                    //příklad infox: image/png|99330|2019-11-13_104250.png|8ab14290cd8d4a929d518d2f9e663ecd_2019-11-13_104250.png|8ab14290cd8d4a929d518d2f9e663ecd|1|Popisek|Číslo jednací|101

                    var strTempFullPath = tempDir + v.Guid + "_" + formFile.FileName;
                    

                    System.IO.File.WriteAllText(tempDir + v.Guid + ".infox", formFile.ContentType + "|" + formFile.Length.ToString() + "|" + formFile.FileName + "|" + v.Guid + "_" + formFile.FileName + "|" + v.Guid + "|0|||0");


                    using (var stream = new FileStream(strTempFullPath, FileMode.Create))
                    {
                        await formFile.CopyToAsync(stream);
                    }
                }
            }

            v.lisTempFiles = Factory.o27AttachmentBL.GetTempFiles(v.Guid);

            
            return View(v);
            
        }

        public IActionResult DoUpload(string guid, int x29id,string prefix,int o13id,int recpid)
        {
            if (x29id == 0 || string.IsNullOrEmpty(guid)==true)
            {
                return this.StopPageSubform("x29id or guid is missing");
            }
            var v = new FileUploadViewModel() { Guid = guid, x29ID = x29id, o13ID = o13id, RecPid = recpid };
            

            RefreshStateDoUpload(v);
            
            return View(v);
        }
        [HttpPost]
        public async Task<IActionResult> DoUpload(FileUploadViewModel v,List<IFormFile> files)
        {                                   
            var tempDir = Factory.App.TempFolder + "\\";

            foreach (var formFile in files)
            {
                if (formFile.Length > 0)
                {
                    // full path to file in temp location
                    //var strTempFullPath = Path.GetTempFileName();
                    //příklad infox: image/png|99330|2019-11-13_104250.png|8ab14290cd8d4a929d518d2f9e663ecd_2019-11-13_104250.png|8ab14290cd8d4a929d518d2f9e663ecd|1|Popisek|Číslo jednací|101

                    var strTempFullPath = tempDir + v.Guid + "_" + formFile.FileName;
                    if (v.o27Label != null)
                    {
                        v.o27Label = v.o27Label.Replace("|", "");
                    }
                    else
                    {
                        v.o27Label = "";
                    }
                    if (v.o27PressMark != null)
                    {
                        v.o27PressMark = v.o27PressMark.Replace("|", "");
                    }
                    else
                    {
                        v.o27PressMark = "";
                    }
                    
                    System.IO.File.AppendAllText(tempDir + v.Guid + "_" + formFile.FileName + ".infox", formFile.ContentType + "|" + formFile.Length.ToString() + "|" + formFile.FileName + "|" + v.Guid + "_" + formFile.FileName+"|"+v.Guid+"|"+v.o13ID.ToString()+"|"+v.o27Label+"|"+v.o27PressMark+"|"+v.x29ID.ToString());
                    

                    using (var stream = new FileStream(strTempFullPath, FileMode.Create))
                    {
                        await formFile.CopyToAsync(stream);
                    }
                }
            }

            RefreshStateDoUpload(v);

            // process uploaded files
            // Don't rely on or trust the FileName property without validation.
            return View(v);
            //return Ok(new { count = files.Count, size, tempfiles });
        }

        
        private void RefreshStateDoUpload(FileUploadViewModel v)
        {
            
            v.lisO13 = Factory.o13AttachmentTypeBL.GetList(new BO.myQueryO13() { x29id = v.x29ID });
            
            if (v.x29ID==101 && Factory.App.Implementation != "HD")
            {
                v.IsUsePressMark = true;
            }
            v.lisTempFiles = Factory.o27AttachmentBL.GetTempFiles(v.Guid);
            if (v.RecPid > 0)
            {
                
                v.lisO27 = Factory.o27AttachmentBL.GetList(new BO.myQueryO27() { x29id = v.x29ID, recpid = v.RecPid }, v.Guid);
            }
        }

        [HttpGet]
        public ActionResult FileDownloadInline(string downloadguid)
        {
            var c = Factory.o27AttachmentBL.LoadByGuid(downloadguid);            

            string fullPath = Factory.App.UploadFolder + "\\" + c.o27ArchiveFolder + "\\" + c.o27ArchiveFileName;
            if (System.IO.File.Exists(fullPath))
            {
                Response.Headers["Content-Disposition"] = string.Format("inline; filename={0}", c.o27OriginalFileName);
                var fileContentResult = new FileContentResult(System.IO.File.ReadAllBytes(fullPath), c.o27ContentType);

                return fileContentResult;
            }
            else
            {
                return FileDownloadNotFound(c);
            }

        }
        

        public ActionResult FileDownloadTempFile(string tempfilename)
        {
            if (!System.IO.File.Exists(Factory.App.TempFolder + "\\" + tempfilename))
            {
                return FileDownloadNotFound(new BO.o27Attachment() { o27OriginalFileName = tempfilename, o27ArchiveFolder = "TEMP" });
            }
            BO.o27Attachment recO27 = Factory.o27AttachmentBL.InhaleFileByInfox(Factory.App.TempFolder + "\\" + tempfilename + ".infox");
            if (recO27 == null)
            {
                //neexistuje infox soubor, ale temp file existuje
                recO27 = new BO.o27Attachment() { o27OriginalFileName=tempfilename };
                
            }
           
            if (string.IsNullOrEmpty(recO27.o27ContentType) == true)
            {
                byte[] fileBytes = System.IO.File.ReadAllBytes(Factory.App.TempFolder + "\\" + tempfilename);
                Response.Headers["Content-Type"] = "application/octet-stream";
                Response.Headers["Content-Length"] = fileBytes.Length.ToString();
                return File(fileBytes, "application/octet-stream", tempfilename);
            }
            else
            {
                Response.Headers["Content-Disposition"] = string.Format("inline; filename={0}", recO27.o27OriginalFileName);
                var fileContentResult = new FileContentResult(System.IO.File.ReadAllBytes(Factory.App.TempFolder + "\\" + tempfilename), recO27.o27ContentType);
                return fileContentResult;                
            }

        }

        public ActionResult FileDownloadTempFileNDB(string tempfilename,string contenttype,string downloadfilename)
        {
            if (!System.IO.File.Exists(Factory.App.TempFolder + "\\" + tempfilename))
            {
                return FileDownloadNotFound(new BO.o27Attachment() { o27OriginalFileName = tempfilename, o27ArchiveFolder = "TEMP" });
            }
            Response.Headers["Content-Disposition"] = string.Format("inline; filename={0}", downloadfilename);
            var fileContentResult = new FileContentResult(System.IO.File.ReadAllBytes(Factory.App.TempFolder + "\\" + tempfilename), contenttype);
            return fileContentResult;
        }






        public ActionResult FileDownloadNotFound(BO.o27Attachment c)
        {
            var fullPath = Factory.App.TempFolder + "\\notfound.txt";
            System.IO.File.WriteAllText(fullPath, string.Format("Soubor [{0}] na serveru [??????\\{1}] neexistuje!", c.o27OriginalFileName, c.o27ArchiveFolder));
            Response.Headers["Content-Disposition"] = string.Format("inline; filename={0}", "notfound.txt");
            var fileContentResult = new FileContentResult(System.IO.File.ReadAllBytes(fullPath), "text/plain");
            return fileContentResult;
        }

        public BO.Result ChangeTempFileLabel(string tempfilename,string newlabel)
        {
            if (string.IsNullOrEmpty(newlabel) == true)
            {
                newlabel = "";
            }
            newlabel = newlabel.Replace("|", "");
            string strInfoxFullPath = Factory.App.TempFolder + "\\" + tempfilename + ".infox";
            var rec = Factory.o27AttachmentBL.InhaleFileByInfox(strInfoxFullPath);
            if (rec == null)
            {
                return new BO.Result(true, "infox is null");
            }
            //System.IO.File.WriteAllText(Factory.App.TempFolder + "\\" + tempfilename + ".popis", newlabel);
            System.IO.File.WriteAllText(strInfoxFullPath, rec.o27ContentType + "|" + rec.o27FileSize.ToString() + "|" + rec.o27OriginalFileName + "|" + rec.o27GUID + "_" + rec.o27OriginalFileName + "|" + rec.o27GUID + "|" + rec.o13ID.ToString() + "|" + newlabel + "|" + rec.o27PressMark+"|"+rec.x29ID.ToString());

            return new BO.Result(false);
        }
        public BO.Result ChangeFileLabel(string fileguid, string newlabel)
        {
            if (string.IsNullOrEmpty(newlabel) == true)
            {
                newlabel = "";
            }
            newlabel = newlabel.Replace("|", "");

            var rec = Factory.o27AttachmentBL.LoadByGuid(fileguid);
            if (rec == null)
            {
                return new BO.Result(true, "file not found");
            }
            rec.o27Label = newlabel;
            if (Factory.o27AttachmentBL.Save(rec) > 0)
            {
                return new BO.Result(false);
            }
            else
            {
                return new BO.Result(true,"Chyba");
            }
                       
        }
        public bool Delete2Temp(string fileguid,string guid)
        {
            var rec = Factory.o27AttachmentBL.LoadByGuid(fileguid);
            var recTemp = new BO.p85Tempbox() { p85GUID = guid, p85DataPID = rec.pid,p85OtherKey1=rec.x29ID,p85OtherKey2=rec.o27DataPID };
            Factory.p85TempboxBL.Save(recTemp);
            //this.AddMessage("Odstranění přílohy je třeba potvrdit tlačítkem [Uložit změny]", "info");
            return true; 

        }



    }
}