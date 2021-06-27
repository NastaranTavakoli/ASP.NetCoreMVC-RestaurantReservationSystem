using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ReservationSystemDraft_v1.Areas.Admin.Models.Area
{
    public class Details
    {
        public int Id { get; set; }
        [Required, Display(Name = "Area Name")]
        public string Name { get; set; }
        [Required]
        public int RestaurantId { get; set; }

        public List<Data.Table> Tables { get; set; } = new List<Data.Table>();
    }
}
