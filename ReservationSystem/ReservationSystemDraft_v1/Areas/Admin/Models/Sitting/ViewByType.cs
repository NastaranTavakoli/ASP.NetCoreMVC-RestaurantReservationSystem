using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReservationSystemDraft_v1.Areas.Admin.Models.Sitting
{
    public class ViewByType
    {
        public int TemplateId { get; set; }
        public string Type { get; set; }
        public List<Data.Sitting> Sittings { get; set; }
        public int TotalSittings { get; set; }
        public int PageNumber { get; set; }

    }
}
