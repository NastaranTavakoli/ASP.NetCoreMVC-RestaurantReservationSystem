using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ReservationSystemDraft_v1.Data
{
    public class Area
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public int RestaurantId { get; set; }
        public Restaurant Restaurant { get; set; }
        public List<Table> Tables { get; set; } = new List<Table>();
    }
}
