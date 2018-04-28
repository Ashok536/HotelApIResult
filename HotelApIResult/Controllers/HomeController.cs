using HotelApIResult.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace HotelApIResult.Controllers
{
    public class HomeController : Controller
    {
        string BaseUri = "http://api.tektravels.com/SharedServices/SharedData.svc/";
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Index(bool result = true)
        {
            ViewBag.Msg = GetHotelSearch();
            return View();
        }
        
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";
            return View();
        }

        public string GetAthu()
        {
            Authenticate at = new Authenticate();
            at.ClientId = "ApiIntegrationNew";
            at.UserName = "Pay2cart";
            at.Password = "Pay2cart@123";
            at.EndUserIp = "192.168.11.120";
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(BaseUri);
            //MediaTypeWithQualityHeaderValue contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var contentData = new StringContent(JsonConvert.SerializeObject(at), Encoding.UTF8, "application/json");
            HttpResponseMessage responseMessage = client.PostAsync("rest/Authenticate", contentData).Result;
            string Data = responseMessage.Content.ReadAsStringAsync().Result;

            return Data;
        }

        public string GetHotelSearch()
        {
            Hotelsearch htlsr = new Hotelsearch();

            int NoOfAdults = 5,NoOfChild =6;
            List<int> age = new List<int>();
            age.Add(3);
            age.Add(5);
            age.Add(7);
            age.Add(4);
            age.Add(8);
            int p=0;

            List<RoomGsts> guests = new List<RoomGsts>();
            
            int noofRooms = NoOfAdults / 2+NoOfAdults%2;
            for (int j = 1; j <= NoOfAdults; j++)
            {
                RoomGsts room = new RoomGsts();
                if(j%2!=0 & NoOfAdults==j)
                { room.NoOfAdults = 1; List<int> ig = new List<int>();
                    ig.Clear();
                    //room.NoOfAdults = 2;
                    int i = guests.Count;
                    
                    for (p = j - 1; p < age.Count; p++)
                    {
                        if (p < age.Count)
                        {
                            ig.Add(age[p]);
                            room.NoOfChild = ig.Count;
                        }
                        room.ChildAge = ig;
                    }
                    guests.Add(room);
                }
                else if (j % 2 == 0)
                {
                    List<int> ig = new List<int>();
                    ig.Clear();
                    room.NoOfAdults = 2;
                    for (p = j - 2; p < j; p++)
                    {
                        if (p < age.Count)
                        {
                            ig.Add(age[p]);
                            room.NoOfChild = ig.Count;
                        }
                        room.ChildAge = ig;
                    }
                    guests.Add(room);
                }               
            }
            htlsr.RoomGuests = guests;
            string date =Convert.ToString(DateTime.Now.ToString("dd/MM/yyyy"));
            htlsr.CheckInDate = "28/04/2018";
            htlsr.NoOfNights = 1;
            htlsr.CountryCode = "NL";
            htlsr.CityId = 14621;
            htlsr.ResultCount = 0;
            htlsr.PreferredCurrency = "INR";
            htlsr.GuestNationality = "IN";
            htlsr.NoOfRooms = 1;            
            htlsr.PreferredHotel = "";
            htlsr.MaxRating = 5;
            htlsr.MinRating = 0;
            //htlsr.ReviewScore = 0.0;
            htlsr.isNearBySearchAllowed = false;
            htlsr.EndUserIp = "192.168.11.120";
            htlsr.TokenId = "1fa65d75-9d59-45b5-b555-59fc5f76c010";
            string sr = JsonConvert.SerializeObject(htlsr);

            //HttpClient client = new HttpClient();
            //client.BaseAddress = new Uri("http://api.tektravels.com/BookingEngineService_Hotel/hotelservice.svc/");
            ////MediaTypeWithQualityHeaderValue contentType = new MediaTypeWithQualityHeaderValue("application/json");
            //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //var contentData = new StringContent(JsonConvert.SerializeObject(htlsr), Encoding.UTF8, "application/json");
            //HttpResponseMessage responseMessage = client.PostAsync("rest/GetHotelResult",contentData).Result;
            //string Data = responseMessage.Content.ReadAsStringAsync().Result;

            //var result = JsonConvert.DeserializeObject<HotelResult>(Data);
            //JObject jObject =(JObject) JsonConvert.DeserializeObject(Data);
            //JObject json = (JObject)jObject["HotelSearchResult"];
            //string ResponseStatus= json["ResponseStatus"].ToString();
            //JArray hotellist =(JArray)json["HotelResults"];
            //var Htlist = hotellist.ToList();
            //IEnumerable<HotelResult> htl =hotellist.ToObject<IEnumerable<HotelResult>>();

            return sr;
        }
    }
    public class Hotelsearch
    {
        public string CheckInDate { get; set; }
        public int NoOfNights { get; set; }
        public string CountryCode { get; set; }
        public int CityId { get; set; }
        public int ResultCount { get; set; }
        public string PreferredCurrency { get; set; }
        public string GuestNationality { get; set; }
        public int NoOfRooms { get; set; }
        public List<RoomGsts> RoomGuests { get; set; }
        public string PreferredHotel { get; set; }
        public int MaxRating { get; set; }
        public int MinRating { get; set; }
        public decimal ReviewScore { get; set; }
        public bool isNearBySearchAllowed { get; set; }
        public string EndUserIp { get; set; }
        public string TokenId { get; set; }
    }

    public class RoomGsts
    {
        public int NoOfAdults { get; set; }
        public int NoOfChild { get; set; }
        public List<int> ChildAge { get; set; }
    }

    public class HotelResult
    {
        public string HotelCode { get; set; }
    }

}