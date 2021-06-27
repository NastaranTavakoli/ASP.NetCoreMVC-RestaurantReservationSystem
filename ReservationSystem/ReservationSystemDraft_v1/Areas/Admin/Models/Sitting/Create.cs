using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ReservationSystemDraft_v1.Areas.Admin.Models.Sitting
{
    public class Create
    {
        [Required]
        public int RestaurantId { get; set; }

        public SelectList SittingTypes { get; set; }

        [Display(Name = "Sitting Type")]
        public int? SittingTypeId { get; set; }

        [Display(Name ="Create New Sitting Type"), StringLength(100, MinimumLength = 2)]
        public string NewSittingType { get; set; }

        [Required, Display(Name = "Start Time"), DataType(DataType.Time)]
        public DateTime StartTime { get; set; }

        [Required, Display(Name = "End Time"), DataType(DataType.Time)]
        public DateTime EndTime { get; set; }

        [Required, Range(1, 10000)]
        public int Capacity { get; set; }

        [Required, Display(Name = "Closed for bookings")]
        public bool IsClosedForBooking { get; set; }

        [Required, Display(Name = "Reservation Time Increment", Description = "The time in minutes between reservation time slots")]
        [Range(10, 1440)]
        public int Increment { get; set; }

        [Required, Display(Name = "Default Reservation Duration"), Range(0, 1440)]
        public int DefaultReservationDuration { get; set; }

        [Display(Name = "One-off Sitting")]
        public bool OneOffSitting { get; set; }

        [Required, Display(Name = "Schedule On")]
        public bool[] DaysOfWeek { get; set; }

        [Required, Display(Name = "Schedule from"), DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Required, Display(Name = "Until"), DataType(DataType.Date)]
        public DateTime EndDate { get; set; }
        [Display(Name = "Name"), StringLength(100, MinimumLength = 2)]
        public string Name { get; set; }
    }
}
