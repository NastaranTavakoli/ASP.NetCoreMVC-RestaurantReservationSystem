using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ReservationSystemDraft_v1.Areas.Admin.Models.Table
{
    public class Create
    {
        [Required]
        public string Name { get; set; }
        [Required, Range(1, 100)]
        public int Capacity { get; set; }

        [Required, Display(Name="Area")]
        public int AreaId { get; set; }
        public string AreaName { get; set; }

        [NotMapped]
        public SelectList AreaOptions { get; set; }
    }
}
