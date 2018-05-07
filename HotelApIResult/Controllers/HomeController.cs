using HotelApIResult.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using HotelServices;
using System.Configuration;

namespace HotelApIResult.Controllers
{
    public class HomeController : Controller
    {
        private string BaseUriUpToCityData = ConfigurationManager.AppSettings["BaseuriUpToCity"];
        private string BaseUriFormHotelSearch = ConfigurationManager.AppSettings["BaseuriFormHotelSearch"];
        private string ClientId= ConfigurationManager.AppSettings["ClientId"];
        private string Username = ConfigurationManager.AppSettings["UserName"];
        private string Password = ConfigurationManager.AppSettings["Password"];
        static GetIp g = new GetIp();
        private string ip = g.GettingIP();

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

        public ActionResult Contact(string RoomIndexs)
        {
            if (RoomIndexs == null)
            {
                return RedirectToAction("Error");
            }
            HotelRoomsDetails hd = JsonConvert.DeserializeObject<HotelRoomsDetails>(RoomIndexs);
            //int i =Convert.ToInt32(Request.QueryString["RoomIndexs"]);
           
            return View(hd);
        }

        public ActionResult Error()
        { return View(); }

        public ActionResult HotelResults()
        {
            var res = GetHotelSearch();
            return View(res);
        }

        public ActionResult HotelInfos()
        {
            var fer = getHotelInfo();
            ViewBag.Msg = GetHotelRooms();
            return View(fer);
        }

        public string GetAthu()
        {
            Authenticate at = new Authenticate();
            at.ClientId = ClientId;
            at.UserName = Username;
            at.Password = Password;
            at.EndUserIp = ip.Replace(" ", "");
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
            catch(WebException webEx)
            {
                return webEx.Response.ToString();
            }
        }

        public string GetCountryList()
        {
            var response1 = string.Empty;
            CountryRequest cf = new CountryRequest();
            cf.ClientId = ClientId;
            cf.TokenId = "bd199c2b-ad01-4b9a-bcb3-9a5d8bd0795a";
            cf.EndUserIp = ip.Replace(" ", "");
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(BaseUriUpToCityData);
            MediaTypeWithQualityHeaderValue contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            var contentData = new StringContent(JsonConvert.SerializeObject(cf), System.Text.Encoding.UTF8, "application/json");
            HttpResponseMessage response = client.PostAsync("rest/CountryList", contentData).Result;
            string stringData = response.Content.ReadAsStringAsync().Result;
            ViewBag.Message = stringData;
            string str = stringData;
            var obj = JsonConvert.DeserializeObject<countrys>(str);
            Console.WriteLine(obj.Countrylist);
            int status = obj.Status;

            if (status == 1)
            {
                string countryListData = obj.Countrylist;
                XDocument doc = XDocument.Parse(countryListData);
          
                var CountryCodes = from service in doc.Descendants("Country")
                                   select (string)service.Element("Code");
                var CountryNames = from service in doc.Descendants("Country")
                                   select (string)service.Element("Name");
                int i = 0;
                CountryList eds = new CountryList();
                foreach (var item in CountryCodes.Zip(CountryNames, (a, b) => new { A = a, B = b }))
                {
                    var a = item.A;
                    var b = item.B;
                    eds.CountryCode = a;
                    eds.CountryName = b;
                    //dsj.CountryLists.Add(eds);
                    //dsj.SaveChanges();
                    i++;
                }
            }

            return stringData;
        }

        public string GetDestinationList()
        {
            //var has = dsj.CountryLists.Select(b => b.CountryCode).ToList();
            var response1 = string.Empty;
            DestinationRequest cf = new DestinationRequest();
            // CountryList cc = new CountryList();
            //DestinationTab cd = new DestinationTab();
            string str = null;
            cf.ClientId = ClientId;
            cf.TokenId = "bd199c2b-ad01-4b9a-bcb3-9a5d8bd0795a";
            cf.EndUserIp = ip.Replace(" ", "");
            //foreach (string st in has)
            //{
                cf.CountryCode = "IN";
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(BaseUriUpToCityData);
                MediaTypeWithQualityHeaderValue contentType = new MediaTypeWithQualityHeaderValue("application/json");
                client.DefaultRequestHeaders.Accept.Add(contentType);
                var contentData = new StringContent(JsonConvert.SerializeObject(cf), System.Text.Encoding.UTF8, "application/json");
                HttpResponseMessage response = client.PostAsync("rest/DestinationCityList", contentData).Result;
                string stringData = response.Content.ReadAsStringAsync().Result;
                str = stringData;
                var de = JsonConvert.DeserializeObject<DestinationResponse>(str);
                int Status = de.Status;
                string dsl = de.DestinationCityList;
                //if (de.DestinationCityList != "No City Found for the same combination.")
                //{
                    XDocument sda = XDocument.Parse(dsl);
                    if (Status == 1)
                    {
                        var Citycode = from service in sda.Descendants("City") select (string)service.Element("CityId");
                        var CityName = from service in sda.Descendants("City") select (string)service.Element("CityName");
                        var Countrycodes = from service in sda.Descendants("City") select (string)service.Element("CountryCode");
                        var ff = Countrycodes.ToList();
                        var fc = Citycode.ToList();
                        var fn = CityName.ToList();

                //foreach (var desn in fc.Zip(fn, Tuple.Create))
                //{
                //    cd.CityId = desn.Item1;
                //    cd.CityName = desn.Item2;
                //    cd.CountryId = st;
                //    dd.DestinationTabs.Add(cd);
                //    dd.SaveChanges();
                //}
                //}
            }
            //}
            return str;
        }

        public IEnumerable<HotelResult> GetHotelSearch()
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
            for (int j = 1; j <= noofRooms; j++)
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

            htlsr.RoomGuests = new List<RoomGsts>() {
                new RoomGsts{NoOfAdults=2,NoOfChild=2,ChildAge=new List<int>(){4,5} }
            };
            string date =Convert.ToString(DateTime.Now.ToString("dd/MM/yyyy"));
            htlsr.CheckInDate = "08/05/2018";
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
            
            string fe = ip.Replace(" ", "");
            htlsr.EndUserIp = ip.Replace(" ", "");
            htlsr.TokenId = "1fa65d75-9d59-45b5-b555-59fc5f76c010";
            string sr = JsonConvert.SerializeObject(htlsr);

            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(BaseUriFormHotelSearch);
            //MediaTypeWithQualityHeaderValue contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var contentData = new StringContent(JsonConvert.SerializeObject(htlsr), Encoding.UTF8, "application/json");
            HttpResponseMessage responseMessage = client.PostAsync("rest/GetHotelResult", contentData).Result;
            string Data = responseMessage.Content.ReadAsStringAsync().Result;

            var result = JsonConvert.DeserializeObject<HotelResult>(Data);
            JObject jObject = (JObject)JsonConvert.DeserializeObject(Data);
            JObject json = (JObject)jObject["HotelSearchResult"];
            string Traceid = (string)json["TraceId"];
            Session["TId"] = Traceid;
            string ResponseStatus = json["ResponseStatus"].ToString();
            JArray hotellist = (JArray)json["HotelResults"];
            var Htlist = hotellist.ToList();
            IEnumerable<HotelResult> htl = hotellist.ToObject<IEnumerable<HotelResult>>();

            return htl;
        }

        public HotelInfoResponse getHotelInfo()
        {
            HotelInfoRequest hir= new HotelInfoRequest();
            hir.EndUserIp = ip.Replace(" ", "");
            hir.HotelCode =Request.QueryString["HotelCode"];
            hir.ResultIndex =Convert.ToInt32(Request.QueryString["ResultIndex"]);
            hir.TokenId = "1fa65d75-9d59-45b5-b555-59fc5f76c010";
            hir.TraceId = Session["TId"].ToString();

            string sr = JsonConvert.SerializeObject(hir);
            HotelInfoResponse hf=new HotelInfoResponse();
            try
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(BaseUriFormHotelSearch);
                //MediaTypeWithQualityHeaderValue contentType = new MediaTypeWithQualityHeaderValue("application/json");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var contentData = new StringContent(JsonConvert.SerializeObject(hir), Encoding.UTF8, "application/json");
                HttpResponseMessage responseMessage = client.PostAsync("rest/GetHotelInfo", contentData).Result;
                string Data = responseMessage.Content.ReadAsStringAsync().Result;
                JObject jObject = (JObject)JsonConvert.DeserializeObject(Data);
                JObject json = (JObject)jObject["HotelInfoResult"];
                int status = (Int32)json["ResponseStatus"];
                if (status == 1)
                {
                    hf = json["HotelDetails"].ToObject<HotelInfoResponse>();
                    hf.HotelRoomsDetails = GetHotelRooms() as List<HotelRoomsDetails>;
                }
            }
            catch(WebException webEx)
            {
                Response.Write(webEx.Response);
            }
            return hf;
        }

        public IEnumerable<HotelRoomsDetails> GetHotelRooms()
        {
            //string response = string.Empty;
            HotelInfoRequest hir = new HotelInfoRequest();
            hir.EndUserIp = ip.Replace(" ", "");
            hir.HotelCode = Request.QueryString["HotelCode"];
            hir.ResultIndex = Convert.ToInt32(Request.QueryString["ResultIndex"]);
            hir.TokenId = "1fa65d75-9d59-45b5-b555-59fc5f76c010";
            hir.TraceId = Session["TId"].ToString();

            HttpClient httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(BaseUriFormHotelSearch);
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var data = new StringContent(JsonConvert.SerializeObject(hir), Encoding.UTF8, "application/json");
            HttpResponseMessage httpResponse = httpClient.PostAsync("rest/GetHotelRoom", data).Result;
            string responseData = httpResponse.Content.ReadAsStringAsync().Result;
            JObject jObject = (JObject)JsonConvert.DeserializeObject(responseData);
            JObject json = (JObject)jObject["GetHotelRoomResult"];
            int Status = (Int32)json["ResponseStatus"];
            IEnumerable<HotelRoomsDetails> hr = null;
            if (Status == 1)
            {
                hr = json["HotelRoomsDetails"].ToObject<IEnumerable<HotelRoomsDetails>>();
            }
            return hr;
        }

    }

    public class countrys
    {
        public int Status { get; set; }
        public string Countrylist { get; set; }
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
        public int ResultIndex { get;set;}
        public string HotelCode { get; set; }
        public string HotelName { get; set; }
        public int StarRating { get; set; }
        public string HotelCategory { get; set; }
        public string HotelDescription { get; set; }
        public string HotelPromotion { get; set; }
        public string HotelPolicy { get; set; }
        public Price Price { get; set; }
        public string HotelPicture { get; set; }
        public string HotelAddress { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
    }

    public class Price
    {
        public string CurrencyCode { get; set; }
        public decimal RoomPrice { get; set; }
        public decimal Discount { get; set; }
        public decimal ServiceTax { get; set; }
    }

    public class HotelInfoRequest
    {
        public int ResultIndex { get; set; }
        public string HotelCode { get; set; }
        public string EndUserIp { get; set; }
        public string TokenId { get; set; }
        public string TraceId { get; set; }
    }
    public class HotelInfoResponse
    {
        public string HotelCode { get; set; }
        public string HotelName { get; set; }
        public int StarRating { get; set; }
        public string Description { get; set; }
        public string HotelPolicy { get; set; }
        public string HotelPicture { get; set; }
        public List<string> Images { get; set; }
        public string Address { get; set; }
        public string CountryName { get; set; }
        public string HotelContactNo { get; set; }
        public string FaxNumber { get; set; }
        public string PinCode { get; set; }
        public string Email { get; set; }
        public List<HotelRoomsDetails> HotelRoomsDetails { get; set; }
        public RoomCombinations RoomCombinations { get; set; }
    }
    public class RoomCombinations
    {
        public string InfoSource { get; set; }
        public List<RoomCombination> RoomCombination { get; set; }
    }
    public class RoomCombination
    {
        public List<int> RoomIndex { get; set; }
    }
    public class HotelRoomsDetails
    {
        public int RoomIndex { get; set; }
        public string RatePlanCode { get; set; }
        public string RatePlanName { get; set; }
        public string RoomTypeName { get; set; }
        public string InfoSource { get; set; }
        public string SequenceNo { get; set; }
        public string RoomPromotion { get; set; }
        public List<string> Amenities { get; set; }
        public List<DayRates> DayRates { get; set; }
        public Price Price { get; set; }
    }
    public  class DayRates
    {
        public decimal Amount { get; set; }
        public string Date { get; set; }
    }
}