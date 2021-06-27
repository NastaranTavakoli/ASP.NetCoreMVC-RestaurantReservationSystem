using ReservationSystemDraft_v1.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ReservationSystemDraft_v1.Areas.Admin.Models.Sitting
{
    public class Index
    {
        public List<SittingType> SittingTypes { get; set; }
        public List<SittingTemplate> SittingTemplates { get; set; }
        public List<Data.Sitting> Sittings { get; set; }
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }
    }
}
