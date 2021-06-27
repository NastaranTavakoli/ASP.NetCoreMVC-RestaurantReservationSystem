using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ReservationSystemDraft_v1.Helpers
{
    public class RestaurantQueryObject
    {

        public string SearchValue { get; set; }

        public string Date { get; set; }

        public string Time { get; set; }

        public int? Guests { get; set; }

        public int? PageNumber { get; set; }

    }
}
