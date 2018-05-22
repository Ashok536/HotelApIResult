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
using HotelApiServices;
using System.Configuration;
using System.Threading.Tasks;
using System.Web.Helpers;

namespace HotelApIResult.Controllers
{
    public class HomeController : Controller
    {
        public HotelBookingCountryCity hotelCity = new HotelBookingCountryCity();
        private string BaseUriUpToCityData = ConfigurationManager.AppSettings["BaseuriUpToCity"];
        private string BaseUriFormHotelSearch = ConfigurationManager.AppSettings["BaseuriFormHotelSearch"];
        private string ClientId= ConfigurationManager.AppSettings["ClientId"];
        private string Username = ConfigurationManager.AppSettings["UserName"];
        private string Password = ConfigurationManager.AppSettings["Password"];

        ApiLogics apiServices = new ApiLogics();
        SearchApI.Generate GsearchApI = new SearchApI.Generate();
        SearchApI searchApI = new SearchApI();
        static GetIp g = new GetIp();
        private string ip = g.GettingIP();

        public HotelAuthDbClass hadc = new HotelAuthDbClass();
        public LogicsforRooms logicsfor = new LogicsforRooms();

        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Index(SearchModel sm)
        {
            if (sm.CheckinDate != null & sm.CheckoutDate != null & sm.CityName != null & sm.Guests > 0 & sm.NoOfRooms > 0)
            {
                TempData["Search"] = sm;
                string url = string.Format("/Home/HotelResults?Search={0}",JsonConvert.SerializeObject(sm));
                return Redirect(url);
            }
            return View();
        }

        public ActionResult ToAuthenticaTE()
        {
            HotelAuth ha = new HotelAuth();
            string date =Convert.ToString(DateTime.Now.ToUniversalTime());

            string fer = GetAthu();
            JObject jObject=(JObject) JsonConvert.DeserializeObject(fer);
            JObject member = (JObject)jObject["Member"];
            ha = member.ToObject<HotelAuth>();
            ha.TokenId =(string) jObject["TokenId"];
            if(ha.LoginDetails==null)
            {
                ha.LoginDetails = date;
            }

            bool result = hadc.SaveAuth(ha);
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

        public ActionResult CancelationPolicy()
        {
            string matter = Request.QueryString["matter"].ToString();
            return PartialView();
        }

        public ActionResult HotelResults(string Search)
        {
            SearchModel sm = JsonConvert.DeserializeObject<SearchModel>(Search);
            SearchModel search = TempData["Search"] as SearchModel;
            if(sm.CityName==null)
            { return RedirectToAction("Index", "Home"); }

            var ges=hotelCity.DestinationCityTabs.Where(s => s.CityName == sm.CityName).FirstOrDefault();
            ViewBag.CityName = ges.CityName;

            var res = GetHotelSearch(sm,ges.CityId,ges.CountryCode).ToList();
            ViewBag.Count = res.Count;

            if(res==null)
            {
                ViewBag.Message = "No Results Found";
            }
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
            string str = JsonConvert.SerializeObject(at);
            string st = apiServices.GetApiAuthenticate(str,BaseUriUpToCityData);
            return st;
        }

        public string GetCountryList()
        {
            string stringData = null;
            string tid = hadc.GetTokenId();
            var response1 = string.Empty;
            CountryRequest cf = new CountryRequest();
            cf.ClientId = ClientId;
            cf.TokenId = tid;
            cf.EndUserIp = ip.Replace(" ", "");
            try
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(BaseUriUpToCityData);
                MediaTypeWithQualityHeaderValue contentType = new MediaTypeWithQualityHeaderValue("application/json");
                client.DefaultRequestHeaders.Accept.Add(contentType);
                var contentData = new StringContent(JsonConvert.SerializeObject(cf), System.Text.Encoding.UTF8, "application/json");
                HttpResponseMessage response = client.PostAsync("rest/CountryList", contentData).Result;
                stringData = response.Content.ReadAsStringAsync().Result;
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
                    CountryTab ct = new CountryTab();
                    foreach (var item in CountryCodes.Zip(CountryNames, (a, b) => new { A = a, B = b }))
                    {
                        var a = item.A;
                        var b = item.B;
                        ct.CountryCode = a;
                        ct.CountryName = b;
                        hotelCity.CountryTabs.Add(ct);
                        hotelCity.SaveChanges();
                    }
                }
            }
            catch(WebException ex)
            {
                ViewBag.Ex = ex;
            }
            return stringData;
        }

        public string GetDestinationList()
        {
            string str = null;
            var has = hotelCity.CountryTabs.Select(b => b.CountryCode).ToList();
            var response1 = string.Empty;
            DestinationRequest cf = new DestinationRequest();
            cf.ClientId = ClientId;
            cf.TokenId = hadc.GetTokenId();
            cf.EndUserIp = ip.Replace(" ", "");

            DestinationCityTab cd = new DestinationCityTab();
            try
            {
                if (has != null)
                {
                    foreach (string st in has)
                    {
                        cf.CountryCode = st;
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
                        if (de.DestinationCityList != "No City Found for the same combination.")
                        {
                            XDocument sda = XDocument.Parse(dsl);
                            if (Status == 1)
                            {
                                var Citycode = from service in sda.Descendants("City") select (string)service.Element("CityId");
                                var CityName = from service in sda.Descendants("City") select (string)service.Element("CityName");
                                var Countrycodes = from service in sda.Descendants("City") select (string)service.Element("CountryCode");
                                var ff = Countrycodes.ToList();
                                var fc = Citycode.ToList();
                                var fn = CityName.ToList();

                                foreach (var desn in fc.Zip(fn, Tuple.Create))
                                {
                                    cd.CityId = Convert.ToInt32(desn.Item1);
                                    cd.CityName = desn.Item2;
                                    cd.CountryCode = st;
                                    hotelCity.DestinationCityTabs.Add(cd);
                                    hotelCity.SaveChanges();
                                }
                            }
                        }
                    }
                }
            }
            catch(WebException ex)
            {
                ViewBag.csg = ex.Response.ToString();
            }
            return str;
        }

        public IEnumerable<HotelResult> GetHotelSearch(SearchModel se,int CityId,string CountryCode)
        {
            IEnumerable<HotelResult> htl = null;
            string date = apiServices.dateChange(se.CheckinDate);
            int dt = apiServices.getDays(se.CheckinDate, se.CheckoutDate);
            string ge = hadc.GetTokenId();

            Hotelsearch htlsr = new Hotelsearch();

            htlsr.RoomGuests = logicsfor.RoomDivideLogic(se);
            htlsr.CheckInDate = date;
            htlsr.NoOfNights = dt;
            htlsr.CountryCode = CountryCode;
            htlsr.CityId = CityId;
            htlsr.ResultCount = 0;
            htlsr.PreferredCurrency = "INR";
            htlsr.GuestNationality = "IN";
            htlsr.NoOfRooms = se.NoOfRooms;
            htlsr.PreferredHotel = "";
            htlsr.MaxRating = 5;
            htlsr.MinRating = 0;
            //htlsr.ReviewScore = 0.0;
            htlsr.isNearBySearchAllowed = false;
            htlsr.EndUserIp = ip.Replace(" ", "");
            htlsr.TokenId = ge;

            string sr = JsonConvert.SerializeObject(htlsr);
            string hotelresult =GsearchApI.GetHotelSearch(BaseUriFormHotelSearch,sr);

            JObject jObject = (JObject)JsonConvert.DeserializeObject(hotelresult);
            JObject json = (JObject)jObject["HotelSearchResult"];
            if (json == null)
            {
                Response.Write(hotelresult);
            }
            else
            {
                string Traceid = (string)json["TraceId"];
                Session["TId"] = Traceid;
                int ResponseStatus = (Int32)json["ResponseStatus"];
                if (ResponseStatus == 1)
                {
                    JArray hotellist = (JArray)json["HotelResults"];
                    if (hotellist != null)
                    {
                        htl = hotellist.ToObject<IEnumerable<HotelResult>>();
                    }
                    else
                    { htl = null; }
                }
            }
            return htl;
        }

        public HotelInfoResponse getHotelInfo()
        {
            string hotelinforesponse = null;
            HotelInfoRequest hir = new HotelInfoRequest();
            hir.EndUserIp = ip.Replace(" ", "");
            hir.HotelCode = Request.QueryString["HotelCode"];
            hir.ResultIndex = Convert.ToInt32(Request.QueryString["ResultIndex"]);
            string tid = hadc.GetTokenId();
            hir.TokenId = tid;
            hir.TraceId = Session["TId"].ToString();

            string sr = JsonConvert.SerializeObject(hir);
            HotelInfoResponse hf = new HotelInfoResponse();

            hotelinforesponse = searchApI.GetHotelInfo(BaseUriFormHotelSearch, sr);
            JObject jObject = (JObject)JsonConvert.DeserializeObject(hotelinforesponse);
            JObject json = (JObject)jObject["HotelInfoResult"];
            if (json == null)
            {
                Response.Write(hotelinforesponse);
                //hf = null;
            }
            else
            {
                int status = (Int32)json["ResponseStatus"];
                if (status == 1)
                {
                    hf = json["HotelDetails"].ToObject<HotelInfoResponse>();
                    hf.HotelRoomsDetails = GetHotelRooms() as List<HotelRoomsDetails>;
                }
            }
            return hf;
        }

        public IEnumerable<HotelRoomsDetails> GetHotelRooms()
        {
            IEnumerable<HotelRoomsDetails> hr = null;
            HotelInfoRequest hir = new HotelInfoRequest();
            hir.EndUserIp = ip.Replace(" ", "");
            hir.HotelCode = Request.QueryString["HotelCode"];
            hir.ResultIndex = Convert.ToInt32(Request.QueryString["ResultIndex"]);
            string tid = hadc.GetTokenId();
            hir.TokenId = tid;
            hir.TraceId = Session["TId"].ToString();
            try
            {
                HttpClient httpClient = new HttpClient();
                httpClient.BaseAddress = new Uri(BaseUriFormHotelSearch);
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var data = new StringContent(JsonConvert.SerializeObject(hir), Encoding.UTF8, "application/json");
                HttpResponseMessage httpResponse = httpClient.PostAsync("rest/GetHotelRoom", data).Result;
                string responseData = httpResponse.Content.ReadAsStringAsync().Result;
                JObject jObject = (JObject)JsonConvert.DeserializeObject(responseData);
                JObject json = (JObject)jObject["GetHotelRoomResult"];
                int Status = (Int32)json["ResponseStatus"];
                if (Status == 1)
                {
                    hr = json["HotelRoomsDetails"].ToObject<IEnumerable<HotelRoomsDetails>>();
                }
            }
            catch(WebException es)
            {
                ViewBag.Rsg = es.Response.ToString();
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
        public decimal OfferedPriceRoundedOff { get; set; }
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
        public List<string> HotelFacilities { get; set; }
        public List<Attractions> Attractions { get; set; }
    }
    public class Attractions
    {
        public string Key { get; set; }
        public string Value { get; set; }
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
        public string SmokingPreference { get; set; }
        public string CancellationPolicy { get; set; }
    }
    public  class DayRates
    {
        public decimal Amount { get; set; }
        public string Date { get; set; }
    }
}