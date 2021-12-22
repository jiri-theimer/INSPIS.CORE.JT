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
                var httpclient = _httpclientfactory.CreateClient();
                try
                {
                    v.lisDokument = cG.SeznamDokumentuVeSpisu(v.InputSpis, httpclient, Factory).Result;
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
                        v.RecGinisDokument = cG.DetailDokumentu(v.InputDokument, httpclient, Factory).Result;
                        v.lisSouboryDokumentu = cG.SeznamSouboruDokumentu(v.RecGinisDokument.IdDokumentu, httpclient, Factory).Result;
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





        
        

        public IActionResult Index()
        {
            
            return View();
        }



    }
}
