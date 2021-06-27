using System;

namespace ReservationSystemDraft_v1.Helpers
{
    public class AvailabilityQueryObject
    {
        public int Duration { get; set; }

        public int Guests { get; set; }

        public string SelectedDate { get; set; }
    }
}