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

        public int getDays(DateTime Checkindate, DateTime Checkoutdate)
        {
            TimeSpan se = Checkoutdate - Checkindate;
            return se.Days;
        }

        public string dateChange(DateTime Checkindate)
        {
            string date= Convert.ToString(Checkindate.ToString("dd/MM/yyyy"));
            date=date.Replace("-", "/");
            return date;
        }

        public List<RoomGsts> LogicEachRoom2()
        {
            int NoOfAdults = 5, NoOfChild = 6;
            List<int> age = new List<int>();
            age.Add(3);
            age.Add(5);
            age.Add(7);
            age.Add(4);
            age.Add(8);
            int p = 0;

            List<RoomGsts> guests = new List<RoomGsts>();
            int noofRooms = NoOfAdults / 2 + NoOfAdults % 2;
            for (int j = 1; j <= noofRooms; j++)
            {
                RoomGsts room = new RoomGsts();
                if (j % 2 != 0 & NoOfAdults == j)
                {
                    room.NoOfAdults = 1; List<int> ig = new List<int>();
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
            return guests;
        }

        public class RoomGsts
        {
            public int NoOfAdults { get; set; }
            public int NoOfChild { get; set; }
            public List<int> ChildAge { get; set; }
        }

        public List<RoomGsts> RoomDivideLogic(/*SearchModel sa*/)
        {
            int NoOfAdults = 2, NoOfChild = 2;
            List<int> age = new List<int>();
            //age.Add(3);
            //age.Add(4);
            //age.Add(5);
            //age.Add(6);
            //age.Add(7);
            int h = 2, t = 2;

            int noofRooms = 1;
            List<RoomGsts> guests = new List<RoomGsts>();
            for (int j = 1; j <= noofRooms; j++)
            {
                RoomGsts room = new RoomGsts();

                if (noofRooms % 2 == 0)
                {
                    List<int> ig = new List<int>();
                    ig.Clear();
                    int k = NoOfAdults / 2;
                    if (j == noofRooms)
                    {
                        room.NoOfAdults = NoOfAdults - k;
                        //if (h >= 1)
                        //{
                        //    for (int p = h - 1; p <= h; p++)
                        //    {
                        //        if (p < age.Count)
                        //        {
                        //            ig.Add(age[p]);
                        //            room.NoOfChild = ig.Count;
                        //        }
                        //        room.ChildAge = ig;
                        //    }
                        //}
                    }
                    else
                    {
                        room.NoOfAdults = k;
                        //if (h >= 1)
                        //{
                        //    for (int p = h - 2; p < h; p++)
                        //    {
                        //        if (p < age.Count)
                        //        {
                        //            ig.Add(age[p]);
                        //            room.NoOfChild = ig.Count;
                        //        }
                        //        room.ChildAge = ig;
                        //    }
                        //}
                    }
                    guests.Add(room);
                    h++;
                }
                else
                {
                    List<int> ig = new List<int>();
                    int f = NoOfAdults / noofRooms;
                    if (j == noofRooms)
                    {
                        for (int k = 0; k < guests.Count; k++)
                        {
                            NoOfAdults = NoOfAdults - guests[k].NoOfAdults;
                        }
                        room.NoOfAdults = NoOfAdults;
                        //if (t >= 1)
                        //{
                        //    for (int p = t - 2; p <= t; p++)
                        //    {
                        //        if (p < age.Count)
                        //        {
                        //            ig.Add(age[p]);
                        //            room.NoOfChild = ig.Count;
                        //        }
                        //        room.ChildAge = ig;
                        //    }
                        //}
                    }
                    else
                    {
                        room.NoOfAdults = f;
                        //if (t >= 1)
                        //{
                        //    for (int p = t - 2; p < t; p++)
                        //    {
                        //        if (p < age.Count)
                        //        {
                        //            ig.Add(age[p]);
                        //            room.NoOfChild = ig.Count;
                        //        }
                        //        room.ChildAge = ig;
                        //    }
                        //}
                    }
                    guests.Add(room);
                    t += 2;
                }
            }
            return guests;
        }
    }
}
