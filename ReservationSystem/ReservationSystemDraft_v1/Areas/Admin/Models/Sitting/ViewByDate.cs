using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ReservationSystemDraft_v1.Areas.Admin.Models.Sitting
{
    public class ViewByDate
    {
        [DataType(DataType.Time)]
        public DateTime Date { get; set; }
        public List<Data.Sitting> Sittings { get; set; }
    }
}
