using ReservationSystemDraft_v1.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ReservationSystemDraft_v1.Areas.Admin.Models.Sitting
{
    public class Details
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public int SittingTypeId { get; set; }
        public SittingType SittingType { get; set; }
        public int SittingTemplateId { get; set; }
        public SittingTemplate SittingTemplate { get; set; }
        [Required]
        public int RestaurantId { get; set; }
        public Restaurant Restaurant { get; set; }
        [Required, DataType(DataType.DateTime), Display(Name = "Start")]
        public DateTime StartTime { get; set; }
        [Required, DataType(DataType.DateTime), Display(Name = "End")]
        public DateTime EndTime { get; set; }
        [Required, Range(0, 10000)]
        public int Capacity { get; set; }
        [Required, Display(Name = "Closed for bookings")]
        public bool IsClosedForBooking { get; set; }
        [Required, Range(10, 1440)]
        public int Increment { get; set; }
        public List<Data.Reservation> Reservations { get; set; } = new List<Data.Reservation>();
        public int TotalReservations { get; set; }
        public int PageNumber { get; set; }
        [Required, Range(0, 1440), Display(Name = "Default Duration")]
        public int DefaultReservationDuration { get; set; }
        public bool Active { get; set; } = true;
        private enum Statuses
        {
            Pending = 1,
            Confirmed = 2,
            Cancelled = 3,
            Seated = 4,
            Completed = 5
        }

        public string GetStatus(int statusId) => ((Statuses)statusId).ToString();

        public double AtCapacity { get; set; }
    }
}
