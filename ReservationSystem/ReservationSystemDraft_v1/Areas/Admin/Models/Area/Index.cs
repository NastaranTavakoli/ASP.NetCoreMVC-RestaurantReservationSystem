using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using ReservationSystemDraft_v1.Data;

namespace ReservationSystemDraft_v1.Areas.Admin.Models.Area
{
    public class Index
    {
        public List<Data.Area> Areas { get; set; } = new List<Data.Area>();
        [Required]
        public string Name { get; set; }
        [Required, Range(1, 20)]
        public int Capacity { get; set; }

        [Required, Display(Name = "Area")]
        public int? AreaId { get; set; }

        [NotMapped]
        public SelectList AreaOptions { get; set; }
    }
}
