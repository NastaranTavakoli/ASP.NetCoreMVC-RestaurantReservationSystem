using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ReservationSystemDraft_v1.Data
{
    public class SittingTemplate
    {
        public int Id { get; set; }
        public string Name { get; set; }
        [Required]
        public int RestaurantId { get; set; }
        public Restaurant Restaurant { get; set; }
        public int SittingTypeId { get; set; }
        public SittingType SittingType { get; set; }
        [Required, DataType(DataType.Date)]
        public DateTime StartDate { get; set; }
        [Required, DataType(DataType.Date)]
        public DateTime EndDate { get; set; }
        [Required, DataType(DataType.Time)]
        public DateTime StartTime { get; set; }
        [Required, DataType(DataType.Time)]
        public DateTime EndTime { get; set; }
        [Required]
        public int Capacity { get; set; }
        [Required]
        public int Increment { get; set; }
        [Required]
        public int DefaultReservationDuration { get; set; }
        public List<Sitting> Sittings { get; set; }
        public bool Active { get; set; }
    }
}
