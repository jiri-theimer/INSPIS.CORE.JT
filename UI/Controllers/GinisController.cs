using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using System.Net.Http;
//using System.Text.Json;
//using System.Text.Json.Serialization;

using Newtonsoft.Json;
using UI.Models;
using UI.Models.Ginis;
using Microsoft.AspNetCore.StaticFiles;

namespace UI.Controllers
{
    public class GinisController : BaseController
    {
        private readonly IHttpClientFactory _httpclientfactory;

        public GinisController(IHttpClientFactory hcf)
        {
            _httpclientfactory = hcf;
        }
        public IActionResult ImportGinisDoc(int a01id)
        {
            var v = new ImportGinisDocViewModel() { a01id = a01id };

            RefreshState_ImportGinisDoc(v);
                      
            return View(v);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ImportGinisDoc(ImportGinisDocViewModel v,string oper)
        {
            RefreshState_ImportGinisDoc(v);
            if (oper == "postback")
            {
                return View(v);
            }
            

            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(v.SelectedSouborCombo))
                {
                    this.AddMessageTranslated("Chybí GINIS soubor v rámci dokumentu."); return View(v);                    
                }

                var gs = v.lisSouboryDokumentu.Where(p => p.IdSouboru == v.SelectedSouborCombo).First();
                var cG = new BL.bas.GinisSupport();
                var ginisfile2save = cG.StahnoutSouborZGinis(gs.IdDokumentu, gs.IdSouboru, gs.TypVazby, v.httpclient, Factory).Result;

                var rec = new BO.o27Attachment() { o27GUID = BO.BAS.GetGuid(),x29ID=101, o27DataPID=v.RecA01.pid,o13ID=v.SelectedO13ID };
                rec.o27GinisDocPID = gs.IdDokumentu;
                rec.o27GinisFilePID = gs.IdSouboru;
                rec.o27OriginalFileName = gs.JmenoOrigSouboru;
                
                rec.o27Label = v.o27Description;
                string strTempPath = Factory.App.TempFolder + "\\" + rec.o27GUID + "_" + ginisfile2save.JmenoOrigSouboru;
                System.IO.File.Copy(ginisfile2save.JmenoTempSouboru, strTempPath, true);
                rec.o27FileSize = Convert.ToInt32(BO.BASFILE.GetFileInfo(strTempPath).Length);
                rec.o27OriginalExtension = BO.BASFILE.GetFileInfo(strTempPath).Extension;
                rec.o27ContentType = GetContentTypeForFileExtension(strTempPath);

                string strDestFolderName = Factory.o27AttachmentBL.GetUploadFolder(v.SelectedO13ID);
                if (Factory.o27AttachmentBL.CopyOneTempFile2Upload(rec.o27GUID + "_" + ginisfile2save.JmenoOrigSouboru, strDestFolderName, rec.o27GUID + "_" + ginisfile2save.JmenoOrigSouboru))
                {                    
                    rec.o27ArchiveFileName = rec.o27GUID + "_" + ginisfile2save.JmenoOrigSouboru;
                    rec.o27ArchiveFolder = Factory.o27AttachmentBL.GetUploadFolder(v.SelectedO13ID);
                    Factory.o27AttachmentBL.Save(rec);

                    v.SetJavascript_CallOnLoad(v.RecA01.pid);
                    return View(v);
                }

            }


            
            return View(v);
        }

        private void RefreshState_ImportGinisDoc(ImportGinisDocViewModel v)
        {
            v.RecA01 = Factory.a01EventBL.Load(v.a01id);
            if (string.IsNullOrEmpty(v.InputSpis))
            {
                v.InputSpis = v.RecA01.a01CaseCode;
                if (v.RecA01.a01CaseCodePID != null)
                {
                    v.InputSpis = v.RecA01.a01CaseCodePID;
                }
            }

            

            var cG = new BL.bas.GinisSupport();
            v.GinisSpisUrl = cG.GetGinisURL(v.RecA01.a01CaseCodePID);

            if (!string.IsNullOrEmpty(v.InputSpis))
            {
                v.httpclient = _httpclientfactory.CreateClient();
                try
                {
                    v.lisDokument = cG.SeznamDokumentuVeSpisu(v.InputSpis, v.httpclient, Factory).Result;
                    if (v.lisDokument.Count() > 0 && v.InputDokument == null)
                    {
                        v.InputDokument = v.lisDokument.First().IdDokumentu;
                    }
                }catch(Exception e)
                {
                    this.AddMessageTranslated("GINIS Error/SeznamDokumentuVeSpisu: " + e.Message);
                    v.lisDokument = new List<BO.Ginis.GinisDocument>();
                }

                if (!string.IsNullOrEmpty(v.InputDokument))
                {
                    try
                    {
                        v.RecGinisDokument = cG.DetailDokumentu(v.InputDokument, v.httpclient, Factory).Result;
                        v.lisSouboryDokumentu = cG.SeznamSouboruDokumentu(v.RecGinisDokument.IdDokumentu, v.httpclient, Factory).Result;
                        if (v.lisSouboryDokumentu.Count() == 0)
                        {
                            this.AddMessageTranslated("Dokument neobsahuje soubory.");
                        }

                    }catch(Exception e)
                    {
                        this.AddMessageTranslated("GINIS Error/DetailDokumentu: " + e.Message);
                    }
                }

            }
            else
            {
                v.lisDokument = new List<BO.Ginis.GinisDocument>();
            }


            v.lisO13 = Factory.o13AttachmentTypeBL.GetList(new BO.myQueryO13() { x29id = 101,a08id=v.RecA01.a08ID });
            


        }



        public string GetContentTypeForFileExtension(string filePath)
        {
            const string DefaultContentType = "application/octet-stream";

            var provider = new FileExtensionContentTypeProvider();

            if (!provider.TryGetContentType(filePath, out string contentType))
            {
                contentType = DefaultContentType;
            }

            return contentType;
        }




        public IActionResult Index()
        {
            
            return View();
        }



    }
}
