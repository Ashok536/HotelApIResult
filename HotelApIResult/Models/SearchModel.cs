using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HotelApIResult.Models
{
    public class SearchModel
    {
        [Required(ErrorMessage ="CityName is Required")]
        public string CityName { get; set; }
        [Required(ErrorMessage = "CheckInDate is Required")]
        public DateTime CheckinDate { get; set; }
        [Required(ErrorMessage = "CheckOutdate is Required")]
        public DateTime CheckoutDate { get; set; }
        [Required(ErrorMessage = "Rooms is Required")]
        [DisplayName("Rooms")]
        public int NoOfRooms { get; set; }
        [Required(ErrorMessage = "Adults is Required")]
        [DisplayName("Adults")]
        public int Guests { get; set; }
        [DisplayName("Childs")]
        public int NoOFChilds { get; set; }
    }
}