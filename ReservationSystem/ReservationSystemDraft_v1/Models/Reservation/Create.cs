using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using ReservationSystemDraft_v1.Data;
using ReservationSystemDraft_v1.Services;

namespace ReservationSystemDraft_v1.Models.Reservation


{
    public class Create
    {
        public int Id { get; set; }
        public int RestaurantId { get; set; }
        public Data.Restaurant Restaurant { get; set; }

        [Required(ErrorMessage = "Please specify between 1 and 12 guests. If you require a larger booking, please contact the restaurant directly."), Display(Name = "Number of Guests"), Range(1, 12)]
        public int Guests { get; set; }

        [Required, DataType(DataType.Date), Display(Name = "Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime SelectedDate { get; set; }

        [Required, DataType(DataType.Time), Display(Name = "Start Time")]
        public DateTime StartTime { get; set; }

        public List<ReservationService.Availability> Availabilities { get; set; } = new List<ReservationService.Availability>();


        [Required]
        public int SittingId { get; set; }
    }
}
