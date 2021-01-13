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

            return Newtonsoft.Json.JsonConvert.DeserializeObject<Pocasi>(strJson);
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
            
            var httpclient = _httpclientfactory.CreateClient();

            using (var request = new HttpRequestMessage(new HttpMethod("POST"), "https://api.fidoo.com/v2/user/get-user"))
            {
                request.Headers.TryAddWithoutValidation("Accept", "application/json");
                request.Headers.TryAddWithoutValidation("X-Api-Key", "paklic1");

                request.Content = new StringContent("{  \n   \"userId\": \"d046b326-3d6c-4dc3-852f-3999ac3f0632\"  \n }");


                request.Content.Headers.ContentType = new MediaTypeWithQualityHeaderValue("application/json");


                var response = await httpclient.SendAsync(request);

                return await response.Content.ReadAsStringAsync();
            }

        }

    }


}
