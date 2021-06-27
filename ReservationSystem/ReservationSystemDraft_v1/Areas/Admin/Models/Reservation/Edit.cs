using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using ReservationSystemDraft_v1.Data;
using ReservationSystemDraft_v1.Services;

namespace ReservationSystemDraft_v1.Areas.Admin.Models.Reservation
{
    public class Edit
    {
        public int Id { get; set; }
        [Required]
        [Display(Name = "Sitting")]
        public int SittingId { get; set; }

        public int StatusId { get; set; }
        public int OldSittingId { get; set; }
        public int SittingTypeId { get; set; }
        public int OldSittingTypeId { get; set; }

        [Required]
        [Display(Name = "Sitting")]
        public SelectList SittingTypes { get; set; }

        public List<Data.Sitting> AvailableSittings { get; set; } = new List<Data.Sitting>();
        public List<ReservationService.Availability> Availabilities { get; set; } = new List<ReservationService.Availability>();

        [Required]
        [Display(Name = "Available time slots")]
        public SelectList AvailabilitiesOptions { get; set; }

        [Required]
        public int CustomerId { get; set; }
        public Person Customer { get; set; }
        [Required]
        [Display(Name = "Number of guests"), Range(0, 40)]
        public int Guests { get; set; }

        public int OldGuests { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }


        [Required]
        [DataType(DataType.Time), Display(Name = "Start Time")]
        public DateTime StartTime { get; set; }


        [Display(Name = "Start Time")]
        public DateTime OldStartTime { get; set; }

        public int OldDuration { get; set; }
        public int Duration { get; set; }
        [Required]
        [Display(Name = "Duration")]
        public SelectList DurationOptions { get; set; }

        [Display(Name = "Include unavailable times")]
        public bool ShowUnavailableTimeSlots { get; set; }

        public string Notes { get; set; }
    }
}
