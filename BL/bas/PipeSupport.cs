using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;

namespace BL.bas
{
    public class PipeSupport
    {
        private readonly HttpClient _httpclient;
        private readonly BL.Factory _f;

        public PipeSupport(HttpClient httpclient,BL.Factory f)
        {
            _httpclient = httpclient;
            _f = f;
        }
        private string getApiKey()
        {
            var rec = new BO.p85Tempbox() { p85GUID = BO.BAS.GetGuid(), p85Prefix = "apikey",p85FreeText01="PIPE" };
            if (_f.p85TempboxBL.Save(rec) > 1)
            {
                return rec.p85GUID;
            }
            else
            {
                throw new Exception("Chyba při generování APIKEY");
            }
            
        }
        public async Task<bool> ValidateUser(string login,string password)        //volání PIPE api služby
        {

            using (var request = new HttpRequestMessage(new HttpMethod("GET"), _f.App.PipeBaseUrl + "/api/_ValidateUser?apikey="+getApiKey()+"&login=" + login + "&password=" + password))
            {
                HttpResponseMessage response = await _httpclient.SendAsync(request);
                string strJson = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<bool>(strJson);
               
            }


        }

        public async Task<string> RecoveryPassword(string login, string explicitpassword = null)        //volání PIPE api služby
        {
            string url = _f.App.PipeBaseUrl + "/api/_RecoveryPassword?apikey="+getApiKey()+"&login=" + login;
            if (!string.IsNullOrEmpty(explicitpassword))
            {
                url += "&explicitpassword=" + explicitpassword;
            }
            using (var request = new HttpRequestMessage(new HttpMethod("GET"),url))
            {
                HttpResponseMessage response = await _httpclient.SendAsync(request);
                string strJson = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<string>(strJson);

            }


        }

        public async Task<bool> ChangeLogin(string userid, string newlogin)        //volání PIPE api služby
        {

            using (var request = new HttpRequestMessage(new HttpMethod("GET"), _f.App.PipeBaseUrl + "/api/_ChangeLogin?apikey="+getApiKey()+"&userid=" + userid + "&newlogin=" + newlogin))
            {
                HttpResponseMessage response = await _httpclient.SendAsync(request);
                string strJson = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<bool>(strJson);

            }


        }

        
    }
}
