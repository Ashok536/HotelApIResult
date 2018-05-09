﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace HotelServices
{
    public class GetIp
    {
        public string GettingIP()
        {
            string uri = "http://checkip.dyndns.org/";
            string ip = String.Empty;

            using (var client = new HttpClient())
            {
                var result = client.GetAsync(uri).Result.Content.ReadAsStringAsync().Result;

                ip = result.Split(':')[1].Split('<')[0];
            }
            return ip.Replace(" ","");
        }
    }
}