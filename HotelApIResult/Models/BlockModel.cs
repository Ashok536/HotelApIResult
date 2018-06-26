using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HotelApIResult.Controllers;

namespace HotelApIResult.Models
{
    public class BlockRequestModel
    {
        public string EndUserIp { get; set; }
        public string TokenId { get; set; }
        public string TraceId { get; set; }
        public int ResultIndex { get; set; }
        public int ClientReferenceNo { get; set; }
        public string HotelCode { get; set; }
        public string HotelName { get; set; }
        public string GuestNationality { get; set; }
        public int NoOfRooms { get; set; }
        public bool IsVoucherBooking { get; set; }
        public List<HotelRoomDetail> HotelRoomsDetails { get; set; }
    }

    public class BookRequestModel
    {
        public string EndUserIp { get; set; }
        public string TokenId { get; set; }
        public string TraceId { get; set; }
        public int ResultIndex { get; set; }
        public int ClientReferenceNo { get; set; }
        public string HotelCode { get; set; }
        public string HotelName { get; set; }
        public string GuestNationality { get; set; }
        public int NoOfRooms { get; set; }
        public bool IsVoucherBooking { get; set; }
        public List<HotelRoomDetail> HotelRoomsDetails { get; set; }
        public List<HotelPassenger> HotelPassenger { get; set; }
    }

    public class HotelPassenger
    {
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Phoneno { get; set; }
        public string Email { get; set; }
        public int PaxType { get; set; }
        public bool LeadPassenger { get; set; }
        public int Age { get; set; }
        public string PassportNo { get; set; }
        public DateTime PassportIssueDate { get; set; }
        public DateTime PassportExpDate { get; set; }
    }
}