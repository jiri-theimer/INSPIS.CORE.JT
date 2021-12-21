using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;

namespace BL.bas
{
    public class GinisSupport
    {
        public bool IsExpressionPID(string strExpression)
        {
            if (strExpression.IndexOf("-") > 0 | strExpression.IndexOf("/") > 0)
                return false;
            else
                return true;
        }

        public string GetGinisURL(string pid)
        {
            if (string.IsNullOrEmpty(pid)) return null;

            return $"http://wmx06.csi.local/gordic/ginis/app/usu05/?c=OpenDetail&ixx1={pid}";


        }

        public async Task<String> GetPidSpisuFromZnacka(string spis, HttpClient httpclient,BL.Factory f)        //volání spisové služby GINIS
        {
            
            using (var request = new HttpRequestMessage(new HttpMethod("GET"), f.App.PipeBaseUrl + "/api/GetPidSpisuFromZnacka?login=" + f.CurrentUser.j03Login + "&znacka=" + spis))
            {
                var response = await httpclient.SendAsync(request);
                var strJson = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<String>(strJson);

            }


        }


        public async Task<List<BO.Ginis.GinisDocumentType>> SeznamDokumentuVeSpisu(string spis, HttpClient httpclient, BL.Factory f)
        {            
            using (var request = new HttpRequestMessage(new HttpMethod("GET"), f.App.PipeBaseUrl + "/api/seznamdokumentuspisu?login="+f.CurrentUser.j03Login+"&pid_spis="+spis))
            {
                var response = await httpclient.SendAsync(request);

                var strJson = await response.Content.ReadAsStringAsync();
                var lis = JsonConvert.DeserializeObject<List<BO.Ginis.GinisDocumentType>>(strJson);


                return lis;
            }
        }


        public async Task<List<BO.Ginis.GinisDocumentType>> SeznamTypuDokumentu(HttpClient httpclient, BL.Factory f)
        {
            
            using (var request = new HttpRequestMessage(new HttpMethod("GET"), f.App.PipeBaseUrl + "/api/seznamtypudokumentu?login="+f.CurrentUser.j03Login))
            {

                var response = await httpclient.SendAsync(request);
                var strJson = await response.Content.ReadAsStringAsync();
                var lis = JsonConvert.DeserializeObject<List<BO.Ginis.GinisDocumentType>>(strJson);
                return lis;
            }
        }
    }
}
