using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ReservationSystemDraft_v1.Areas.Admin.Models.Reservation
{
    public class GetReservations
    {
        public List<Data.Reservation> Reservations { get; set; } = new List<Data.Reservation>();

        [DataType(DataType.Date), Display(Name = "Period Start")]
        public DateTime StartDate { get; set; } 

        [DataType(DataType.Date), Display(Name = "Period End")]
        public DateTime EndDate { get; set; }

        [Display(Name = "Search by name, email or code")]
        public string SearchString { get; set; }

        public int PageNumber { get; set; }

        public int TotalPages { get; set; }

        public bool HasPreviousPage => PageNumber > 1;

        public bool HasNextPage => PageNumber < TotalPages;

        public SelectList OrderOptions { get; set; }

        [Display(Name = "Order by")]
        public int? OrderOptionId { get; set; }
    }
}
