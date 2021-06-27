using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ReservationSystemDraft_v1.Areas.Member.Models.Activity
{
    public class Action
    {
        public Request Request { get; set; }
        [Required]
        public string RequestType { get; set; }
    }
}
