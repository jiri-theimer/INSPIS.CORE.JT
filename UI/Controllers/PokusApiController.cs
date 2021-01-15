using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;

using UI.Models;

namespace UI.Controllers
{
    public class PokusApiController : Controller
    {
        //public Task<string> Simple([FromServices] IHttpClientFactory hcfactory)
        //{
        //    var client = hcfactory.CreateClient();
        //    client.BaseAddress = "http://api.weatherapi.com/v1/current.json?key=fc4dba410de847eaa28202954211301&q=Prague";

        //    return client.GetAsync()
        //}
        private readonly IHttpClientFactory _httpclientfactory;

        public PokusApiController(IHttpClientFactory hcf)
        {
            _httpclientfactory = hcf;
        }

        public async Task<string> Simple1(string city)
        {
            var url = "http://api.weatherapi.com/v1/current.json?key=fc4dba410de847eaa28202954211301&q="+city;
            var httpclient = new HttpClient();
            var response = await httpclient.GetAsync(url);

            return await response.Content.ReadAsStringAsync();
        }
        
        public async Task<string> Simple2(string city)
        {
            var url = "http://api.weatherapi.com/v1/current.json?key=fc4dba410de847eaa28202954211301&q=" + city;
            using(var httpclient = new HttpClient())
            {
                var response = await httpclient.GetAsync(url);
                return await response.Content.ReadAsStringAsync();
            }
            
            

            
        }


        public async Task<string> Simple3(string city)
        {
            var url = "http://api.weatherapi.com/v1/current.json?key=fc4dba410de847eaa28202954211301&q=" + city;

            var httpclient = _httpclientfactory.CreateClient();
            

            var response = await httpclient.GetAsync(url);

           
            return await response.Content.ReadAsStringAsync();
        }


        public async Task<Pocasi> Simple4(string city)
        {
            var url = "http://api.weatherapi.com/v1/current.json?key=fc4dba410de847eaa28202954211301&q=" + city;

            var httpclient = _httpclientfactory.CreateClient();


            var response = await httpclient.GetAsync(url);


            var strJson = await response.Content.ReadAsStringAsync();
            
            return JsonConvert.DeserializeObject<Pocasi>(strJson);
        }


        public async Task<float> Teplota(string city)
        {
            var url = "http://api.weatherapi.com/v1/current.json?key=fc4dba410de847eaa28202954211301&q=" + city;

            var httpclient = _httpclientfactory.CreateClient();


            var response = await httpclient.GetAsync(url);


            var strJson = await response.Content.ReadAsStringAsync();

            var c = JsonConvert.DeserializeObject<Pocasi>(strJson);

            return c.current.temp_c;
        }

        public async Task<float> Vitr(string city)
        {
            var url = "http://api.weatherapi.com/v1/current.json?key=fc4dba410de847eaa28202954211301&q=" + city;

            var httpclient = _httpclientfactory.CreateClient();
            

            var response = await httpclient.GetAsync(url);


            var strJson = await response.Content.ReadAsStringAsync();

            var c = JsonConvert.DeserializeObject<Pocasi>(strJson);

            return c.current.wind_kph;
        }

        public async Task<string> Fidoo1()
        {
            using (var httpClient = new HttpClient())
            {
                using (var request = new HttpRequestMessage(new HttpMethod("POST"), "https://api.fidoo.com/v2/user/get-user"))
                {
                    request.Headers.TryAddWithoutValidation("Accept", "application/json");
                    request.Headers.TryAddWithoutValidation("X-Api-Key", "paklic1");

                    request.Content = new StringContent("{  \n   \"userId\": \"d046b326-3d6c-4dc3-852f-3999ac3f0632\"  \n }");

                    
                    request.Content.Headers.ContentType = new MediaTypeWithQualityHeaderValue("application/json");

                   
                    var response = await httpClient.SendAsync(request);

                    return await response.Content.ReadAsStringAsync();
                }
            }
        }

        
        public async Task<string> FiDoo2()
        {
            //indický youtube návod: https://www.youtube.com/watch?v=bAXZx0zOeCU&ab_channel=RahulNath
            //využito z: https://curl.olsh.me/
            //návod pro práci s HttpClient: https://gist.github.com/dfch/7b338046d5e63e3b3106
            //článek o používání: SYSTEM.NET.HTTP.JSON: https://www.stevejgordon.co.uk/sending-and-receiving-json-using-httpclient-with-system-net-http-json

            //článek k request (POST) v rámci httpclient: https://stackoverflow.com/questions/60420786/using-c-sharp-httpclient-how-to-post-a-string-httpcontent-to-asp-net-core-web-a
            //swagger pro FiDoo: https://api.fidoo.com/v2/#!/public45settings45api45v452/getCostCentersUsingPOST_1

            //první FIDOO api klíč: pak1jgnBUswEVDGTkdT86v21XKMT0UYZSK0rYAq8Ecb1baMI5JskcAIZC32sMbw6
            //jeho zamaskovaná hodnota: pak1jgnBUswEVDGTkdT

            var httpclient = _httpclientfactory.CreateClient();

            using (var request = new HttpRequestMessage(new HttpMethod("POST"), "https://api.fidoo.com/v2/user/get-user-by-email"))
            {
                request.Headers.TryAddWithoutValidation("Accept", "application/json");
                request.Headers.TryAddWithoutValidation("X-Api-Key", "pak1jgnBUswEVDGTkdT86v21XKMT0UYZSK0rYAq8Ecb1baMI5JskcAIZC32sMbw6");

                var s= JsonConvert.SerializeObject(new UI.Models.Fidoo.UserByEmail() { email = "jiri.theimer@marktime.cz" });
                request.Content = new StringContent(s);

                //request.Content = new StringContent("{  \n   \"email\": \"jiri.theimer@marktime.cz\"  \n }");

               
               
                request.Content.Headers.ContentType = new MediaTypeWithQualityHeaderValue("application/json");


                var response = await httpclient.SendAsync(request);

                return await response.Content.ReadAsStringAsync();
            }

        }

        public async Task<string> FiDoo3()
        {
            
            var httpclient = _httpclientfactory.CreateClient();

            using (var request = new HttpRequestMessage(new HttpMethod("POST"), "https://api.fidoo.com/v2/settings/get-cost-centers"))
            {
                request.Headers.TryAddWithoutValidation("Accept", "application/json");
                request.Headers.TryAddWithoutValidation("X-Api-Key", "pak1jgnBUswEVDGTkdT86v21XKMT0UYZSK0rYAq8Ecb1baMI5JskcAIZC32sMbw6");

                var s = JsonConvert.SerializeObject(new UI.Models.Fidoo.QueryRequestRoot());
                request.Content = new StringContent(s);

                request.Content.Headers.ContentType = new MediaTypeWithQualityHeaderValue("application/json");


                var response = await httpclient.SendAsync(request);

                return await response.Content.ReadAsStringAsync();
            }

        }

        public async Task<string> FiDoo4()
        {

            var httpclient = _httpclientfactory.CreateClient();

            using (var request = new HttpRequestMessage(new HttpMethod("POST"), "https://api.fidoo.com/v2/expense/get-expenses"))
            {
                request.Headers.TryAddWithoutValidation("Accept", "application/json");
                request.Headers.TryAddWithoutValidation("X-Api-Key", "pak1jgnBUswEVDGTkdT86v21XKMT0UYZSK0rYAq8Ecb1baMI5JskcAIZC32sMbw6");

                var d1 = new DateTime(2021, 1, 1).ToLocalTime();
                var d2 = d1.AddYears(1).ToLocalTime();
                var s = JsonConvert.SerializeObject(new UI.Models.Fidoo.QueryRequestExpenses() {lastModifyFrom=d1,from=d1,to=d2,limit=1000 });
                request.Content = new StringContent(s);

                request.Content.Headers.ContentType = new MediaTypeWithQualityHeaderValue("application/json");


                var response = await httpclient.SendAsync(request);

                return await response.Content.ReadAsStringAsync();
            }

        }

    }


}
