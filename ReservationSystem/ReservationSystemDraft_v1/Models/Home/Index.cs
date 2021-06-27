using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ReservationSystemDraft_v1.Models.Home
{
    public class Index
    {
        public List<Data.Restaurant> AvailableRestaurants { get; set; } = new List<Data.Restaurant>();
        public List<Data.Restaurant> LatestAddedRestaurants { get; set; } = new List<Data.Restaurant>();

        [Display(Name = "Search by name or address")]
        public string SearchString { get; set; }

        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [DataType(DataType.Time)]
        public DateTime Time { get; set; }

        public int Guests { get; set; }

        public SelectList OrderOptions { get; set; }

        [Display(Name = "Order by")]
        public int? OrderOptionId { get; set; }
    }
}
