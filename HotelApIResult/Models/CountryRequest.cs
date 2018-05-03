using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HotelApIResult.Models
{
    public class CountryRequest
    {
        public string ClientId {get;set;}
        public string TokenId { get; set; }
        public string EndUserIp { get; set; }
    }
    public class CountryList
    {
        public int ID { get; set; }
        public string CountryName { get; set; }
        public string CountryCode { get; set; }
    }
    public class DestinationRequest
    {
        public string ClientId { get; set; }
        public string TokenId { get; set; }
        public string EndUserIp { get; set; }
        public string CountryCode { get; set; }
    }
    public class DestinationResponse
    {
        public string DestinationCityList { get; set; }
        public int Status { get; set; }
    }
}