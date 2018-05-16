using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HotelApIResult.Models;
using HotelApIResult.Controllers;

namespace HotelApIResult
{
    public class HotelAuthDbClass
    {
        private HotelBookingAuth bookingAuth = new HotelBookingAuth();

        public bool SaveAuth(HotelAuth auth)
        {
            if(auth!=null)
            {
                bookingAuth.HotelAuths.Add(auth);
                bookingAuth.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }
        }

        public string GetTokenId()
        {
            string Tokenid = string.Empty;
            Tokenid = bookingAuth.HotelAuths.OrderByDescending(s=>s.Id).Select(s => s.TokenId).FirstOrDefault();
            return Tokenid;
        }
    }
    public class LogicsforRooms
    {
        public List<RoomGsts> RoomDivideLogic(SearchModel sa)
        {
            int NoOfAdults = sa.Guests, NoOfChild = sa.NoOFChilds;
            List<int> age = new List<int>();
            //age.Add(3);
            //age.Add(4);
            //age.Add(5);
            //age.Add(6);
            //age.Add(7);
            int h = 2, t = 2;

            int noofRooms = sa.NoOfRooms;
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