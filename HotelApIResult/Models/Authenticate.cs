using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HotelApIResult.Models
{
    public class Authenticate
    {
        public string ClientId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string EndUserIp { get; set; }
    }
}