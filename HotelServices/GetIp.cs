﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace HotelServices
{
    public class GetIp
    {
        private string BaseUriUpToCityData = ConfigurationManager.AppSettings["BaseuriUpToCity"];
        public string GettingIP()
        {
            string uri = "http://checkip.dyndns.org/";
            string ip = String.Empty;

            using (var client = new HttpClient())
            {
                var result = client.GetAsync(uri).Result.Content.ReadAsStringAsync().Result;

                ip = result.Split(':')[1].Split('<')[0];
            }
            return ip.Replace(" ", "");
        }

        public string GetApiAuthenticate(string at)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(BaseUriUpToCityData);
            try
            {
                //MediaTypeWithQualityHeaderValue contentType = new MediaTypeWithQualityHeaderValue("application/json");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var contentData = new StringContent(JsonConvert.SerializeObject(at), Encoding.UTF8, "application/json");
                HttpResponseMessage responseMessage = client.PostAsync("rest/Authenticate", contentData).Result;
                string Data = responseMessage.Content.ReadAsStringAsync().Result;

                return Data;
            }
            catch (WebException webEx)
            {
                return webEx.Response.ToString();
            }
        }
    }
}
