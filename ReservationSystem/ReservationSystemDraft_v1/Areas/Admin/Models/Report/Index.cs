using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using ReservationSystemDraft_v1.Data;

namespace ReservationSystemDraft_v1.Areas.Admin.Models.Report
{
    public class Index
    {
        [Display(Name = "Sitting Type")]
        public int SittingTypeId { get; set; }
        public SelectList SittingTypes { get; set; }
        public List<List<int>> WeeklySeatsBooked { get; set; } = new List<List<int>>();
        public List<int> ReservationCountListByState { get; set; } = new List<int>();
        public List<int> ReservationCountListBySource { get; set; } = new List<int>();
        public DateTime WeekStart { get; set; }
        public DateTime WeekEnd { get; set; }
        public List<Data.Sitting> Sittings { get; set; } = new List<Data.Sitting>();
    }
}
