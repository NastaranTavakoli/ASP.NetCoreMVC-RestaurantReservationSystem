using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ReservationSystemDraft_v1.Data
{
    public class Table
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public int AreaId { get; set; }
        public Area Area { get; set; }
        public List<ReservationTable> ReservationTables { get; set; } = new List<ReservationTable>();
        [Required]
        public int Capacity { get; set; }
        [NotMapped]
        public string NameAndCapacity => $"{Name} ({Capacity})";
    }
}
