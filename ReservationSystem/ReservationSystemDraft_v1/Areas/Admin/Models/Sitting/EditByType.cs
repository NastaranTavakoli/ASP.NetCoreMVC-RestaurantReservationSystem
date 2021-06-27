using ReservationSystemDraft_v1.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ReservationSystemDraft_v1.Areas.Admin.Models.Sitting
{
    public class EditByType
    {
        [Required]
        public int Id { get; set; }
        [StringLength(100, MinimumLength = 2)]
        public string Name { get; set; }
        [Required]
        public int RestaurantId { get; set; }
        [Required]
        public int SittingTypeId { get; set; }
        public SittingType SittingType { get; set; }
        [Required, DataType(DataType.Date), Display(Name = "Start Date")]
        public DateTime StartDate { get; set; }
        [Required, DataType(DataType.Date), Display(Name = "End Date")]
        public DateTime EndDate { get; set; }
        [Required, DataType(DataType.Time), Display(Name = "Start Time")]
        public DateTime StartTime { get; set; }
        [Required, DataType(DataType.Time), Display(Name = "End TIme")]
        public DateTime EndTime { get; set; }
        [Required, Range(0, 10000)]
        public int Capacity { get; set; }
        [Required, Range(10, 1440)]
        public int Increment { get; set; }
        [Required, Range(0, 1440), Display(Name = "Default Duration")]
        public int DefaultReservationDuration { get; set; }
        public bool Active { get; set; } = true;
    }
}
