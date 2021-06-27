using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ReservationSystemDraft_v1.Data
{
    public class ReservationTable
    {
        public int Id { get; set; }
        [Required]
        public int TableId { get; set; }
        public Table Table { get; set; }
        [Required]
        public int ReservationId { get; set; }
        public Reservation Reservation { get; set; }
    }
}
