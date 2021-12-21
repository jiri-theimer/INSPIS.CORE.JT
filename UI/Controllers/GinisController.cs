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
            v.InputSpis = v.RecA01.a01CaseCode;
            if (v.RecA01.a01CaseCodePID != null)
            {
                v.InputSpis = v.RecA01.a01CaseCodePID;
            }

            return View(v);
        }

        private void RefreshState_ImportGinisDoc(ImportGinisDocViewModel v)
        {
            v.RecA01 = Factory.a01EventBL.Load(v.a01id);

            v.lisDokument = new List<BO.Ginis.GinisDocument>();
            
            v.GinisSpisUrl = new BL.bas.GinisSupport().GetGinisURL(v.RecA01.a01CaseCodePID);

        }





        
        

        public IActionResult Index()
        {
            
            return View();
        }



    }
}
