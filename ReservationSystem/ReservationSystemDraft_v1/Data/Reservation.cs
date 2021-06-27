using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ReservationSystemDraft_v1.Data
{
    public class Reservation
    {
        public int Id { get; set; }
        [Required]
        public int SittingId { get; set; }

        [Required]
        public string ConfirmationCode { get; set; }
        public Sitting Sitting { get; set; }
        [Required]
        public int CustomerId { get; set; }
        public Person Customer { get; set; }
        [Required]
        public int Guests { get; set; }
        [Required]
        public DateTime StartTime { get; set; }
        [Required]
        public int Duration { get; set; }
        public DateTime EndTime => StartTime.AddMinutes(Duration);
        public ReservationSource Source { get; set; }
        [Required]
        public int SourceId { get; set; }
        public ReservationStatus Status { get; set; }
        [Required]
        public int StatusId { get; set; }
        public List<ReservationTable> ReservationTables { get; set; } = new List<ReservationTable>();
        public string Notes { get; set; }
        public bool ActionRequired { get; set; }

    }



}
