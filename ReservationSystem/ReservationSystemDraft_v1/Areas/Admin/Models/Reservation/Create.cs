using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using ReservationSystemDraft_v1.Data;
using ReservationSystemDraft_v1.Services;

namespace ReservationSystemDraft_v1.Areas.Admin.Models.Reservation
{
    public class Create
    {
        public int Id { get; set; }
        [Required]
        public int SittingId { get; set; }

        public string RestaurantName { get; set; }

        public int RestaurantId { get; set; }

        public int SittingTypeId { get; set; }

        [Display(Name = "Sitting")]
        public SelectList SittingTypes { get; set; }


        [Display(Name = "Available Sittings")]
        public SelectList AvailableSittings { get; set; }

        public List<ReservationService.Availability> Availabilities { get; set; } = new List<ReservationService.Availability>();

        [Display(Name = "Available time slots")]
        public SelectList AvailabilitiesOptions { get; set; }

        public int CustomerId { get; set; }


        [Required]
        public string Email { get; set; }
        [Required, Display( Name = "First Name")]
        public string FirstName { get; set; }
        [Required, Display(Name = "Last Name")]
        public string LastName { get; set; }
        [Required]
        public string Phone { get; set; }

        [Required]
        [Display(Name = "Number of guests"), Range(1, 40)]
        public int Guests { get; set; }

        [Required]
        [DataType(DataType.Date), Display(Name = "Date")]
        public DateTime Date { get; set; } = DateTime.Today;

        [Required]
        [DataType(DataType.Time), Display(Name = "Start Time")]
        public DateTime StartTime { get; set; }

        [Display(Name = "Enable unavailable times")]
        public bool ShowUnavailableTimeSlots { get; set; }
        [Required]
        public int Duration { get; set; }
        [Display(Name = "Duration")]
        public SelectList DurationOptions { get; set; }

        [Required, Display(Name = "Source of Reservation")]
        public int SourceId { get; set; }

        [Display(Name = "Source of Reservation")]
        public SelectList SourceTypes { get; set; }

        public string Notes { get; set; }

    }
}
