using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ReservationSystemDraft_v1.Data
{
    public class ReservationStatus
    {
        public int Id { get; set; }
        [Required]
        public string Description { get; set; }
    }
}
