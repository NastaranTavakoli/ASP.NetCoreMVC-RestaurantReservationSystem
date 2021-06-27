using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReservationSystemDraft_v1.Areas.Admin.Models.Report
{
    public class DailyReview
    {
        public DateTime SelectedDate { get; set; }
        public List<int> ReservationCountList { get; set; } = new List<int>();
        public List<Data.Sitting> Sittings { get; set; } = new List<Data.Sitting>();
        public List<DateTime> TimeSlots { get; set; } = new List<DateTime>();
        public List<int> TotalSeats { get; set; } = new List<int>();
        public List<int> SeatsCompleted { get; set; } = new List<int>();
        public List<int> SeatsCanceled { get; set; } = new List<int>();
        public List<int> CapacityList { get; set; } = new List<int>();
        public List<List<int>> DailySeatsBooked { get; set; } = new List<List<int>>();
    }
}



