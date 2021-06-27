using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ReservationSystemDraft_v1.Areas.Member.Models.Activity
{
    public class Request
    {
        public int Id { get; set; }
        [Required]
        public int Guests { get; set; }

        [Required, Display(Name = "Date and Time request")]
        public DateTime StartTime { get; set; }

        [Required]
        public int Duration { get; set; }

        [Required]
        public string Email { get; set; }
        [Required]
        public string Phone { get; set; }
        public string Notes { get; set; }

        public string RestaurantName { get; set; }

    }
}
