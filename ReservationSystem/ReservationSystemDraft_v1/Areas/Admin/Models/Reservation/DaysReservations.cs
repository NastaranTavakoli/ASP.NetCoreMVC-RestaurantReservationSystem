using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ReservationSystemDraft_v1.Data;

namespace ReservationSystemDraft_v1.Areas.Admin.Models.Reservation
{
    public class DaysReservations
    {
        public int SittingId { get; set; }
        public DateTime DateTime { get; set; }
        public List<Data.Sitting> Sittings { get; set; }
        public List<Data.Area> Areas { get; set; }
        public List<Data.Reservation> Reservations { get; set; }
        public string SearchString { get; set; }
    }
}
