using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HotelApIResult.Models;

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
}