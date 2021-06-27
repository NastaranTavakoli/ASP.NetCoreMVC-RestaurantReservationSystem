using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ReservationSystemDraft_v1.Data
{
    public class Sitting
    {
        public int Id { get; set; }
        [Required]
        public int SittingTypeId { get; set; }
        public SittingType SittingType { get; set; }

        public int? SittingTemplateId { get; set; }
        public SittingTemplate SittingTemplate { get; set; }

        [Required]
        public int RestaurantId { get; set; }
        public Restaurant Restaurant { get; set; }
        public string Name { get; set; }
        [Required]
        public DateTime StartTime { get; set; }
        [Required]
        public DateTime EndTime { get; set; }

        public string Description => $"{SittingType.Description} {StartTime.ToShortTimeString()}-{EndTime.ToShortTimeString()}, Status: {(IsClosedForBooking ? "Closed" : "Open")}, Capacity: {Capacity}";

        public string ShortDescription => $"{SittingType.Description} {StartTime.ToShortTimeString()}-{EndTime.ToShortTimeString()}";

        [Required]
        public int Capacity { get; set; }
        [Required]
        public bool IsClosedForBooking { get; set; }
        public int Increment { get; set; }
        public List<Reservation> Reservations { get; set; } = new List<Reservation>();
        public int DefaultReservationDuration { get; set; }
        public bool Active { get; set; }
    }
}
