using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
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

    }
}
