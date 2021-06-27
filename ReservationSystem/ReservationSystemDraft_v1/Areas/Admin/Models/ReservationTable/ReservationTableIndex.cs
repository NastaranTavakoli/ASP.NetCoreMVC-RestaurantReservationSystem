using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReservationSystemDraft_v1.Areas.Admin.Models.ReservationTable
{
    public class ReservationTableIndex
    {
        public DateTime Date { get; set; }
        public IEnumerable<Data.Sitting> Sittings { get; set; }
        public int SittingId { get; set; }
        public IEnumerable<Data.Reservation> Reservations { get; set; }
        public int ReservationId { get; set; }
        public SelectList Tables { get; set; }
        public List<Data.ReservationTable> ClashingTables { get; set; }
    }
}
