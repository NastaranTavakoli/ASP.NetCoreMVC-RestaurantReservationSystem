using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReservationSystemDraft_v1.Models.Reservation
{
    public class CreateTimeSlots
    {
        public List<DateTime> TimeSlots { get; set; } = new List<DateTime>();
        public string SittingType { get; set; }
    }
}
