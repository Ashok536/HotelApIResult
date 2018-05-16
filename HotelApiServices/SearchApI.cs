using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace HotelApiServices
{
    public class SearchApI
    {
        public class Generate
        {
            public string GetHotelSearch(string BaseUriFormHotelSearch,string htlsr)
            {
                //IEnumerable<HotelResult> htl = null;
                string Data = null;
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(BaseUriFormHotelSearch);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                try
                {
                    var contentData = new StringContent(htlsr, Encoding.UTF8, "application/json");
                    HttpResponseMessage responseMessage = client.PostAsync("rest/GetHotelResult", contentData).Result;
                    Data = responseMessage.Content.ReadAsStringAsync().Result;

                    var result = JsonConvert.DeserializeObject<HotelResult>(Data);
                }
                catch(AggregateException ae)
                {
                    return ae.ToString();
                }
                catch(SocketException se)
                {
                    Console.WriteLine(se);
                }
                catch (WebException ex)
                {
                    Console.WriteLine(ex);
                }
                return Data;
            }

            public string GetTraceid(string tid)
            {
                HttpContext.Current.Session["TId"] = tid;
                //string ft = HttpContext.Current.Session["mySessionVariable"].ToString();
                return HttpContext.Current.Session["TId"].ToString();
            }
        }

        public string GetHotelInfo(string BaseUriFormHotelSearch,string hir)
        {
            string Data = null;
            try
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(BaseUriFormHotelSearch);
                //MediaTypeWithQualityHeaderValue contentType = new MediaTypeWithQualityHeaderValue("application/json");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var contentData = new StringContent(hir, Encoding.UTF8, "application/json");
                HttpResponseMessage responseMessage = client.PostAsync("rest/GetHotelInfo", contentData).Result;
                Data = responseMessage.Content.ReadAsStringAsync().Result;
            }
            catch (AggregateException ae)
            {
                return ae.ToString();
            }
            catch (WebException ex)
            {
                Console.WriteLine(ex);
            }
            return Data;
        }

        public class HotelResult
        {
            public int ResultIndex { get; set; }
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
    }
}
