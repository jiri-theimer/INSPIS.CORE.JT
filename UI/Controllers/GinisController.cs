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

namespace UI.Controllers
{
    public class GinisController : BaseController
    {
        private readonly IHttpClientFactory _httpclientfactory;

        public GinisController(IHttpClientFactory hcf)
        {
            _httpclientfactory = hcf;
        }
        public IActionResult UploadGinisDocument()
        {
            return View();
        }





        public async Task<List<BO.Ginis.GinisDocumentType>> SeznamDokumentuVeSpisu(string spis)
        {
            var httpclient = _httpclientfactory.CreateClient();

            using (var request = new HttpRequestMessage(new HttpMethod("GET"), "https://tinspiscore.csicr.cz/pipe/api/seznamtypudokumentu?login=lamos"))
            {

                var response = await httpclient.SendAsync(request);

                var strJson = await response.Content.ReadAsStringAsync();
                var lis=JsonConvert.DeserializeObject<List<BO.Ginis.GinisDocumentType>>(strJson);

                
                return lis;
            }
        }

        public IActionResult Index()
        {
            var v = new UI.Models.a01AddAttachment() { };

            SeznamDokumentuVeSpisu("EE").Wait();

            return View(v);
        }



    }
}
