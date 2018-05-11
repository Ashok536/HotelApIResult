using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace HotelApiServices
{
    public class ApiLogics
    {
        public string GetApiAuthenticate(string at,string BaseUriUpToCityData)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(BaseUriUpToCityData);
            try
            {
                //MediaTypeWithQualityHeaderValue contentType = new MediaTypeWithQualityHeaderValue("application/json");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var contentData = new StringContent(at, Encoding.UTF8, "application/json");
                HttpResponseMessage responseMessage = client.PostAsync("rest/Authenticate", contentData).Result;
                string Data = responseMessage.Content.ReadAsStringAsync().Result;

                return Data;
            }
            catch (WebException webEx)
            {
                return webEx.Response.ToString();
            }
        }

        public class Generate
        {
            public string feed()
            {
                return string.Empty;
            }
        }
    }
}
