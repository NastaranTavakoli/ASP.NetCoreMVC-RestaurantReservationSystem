using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ReservationSystemDraft_v1.Data
{
    public class Restaurant
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Phone { get; set; }
        [Required]
        public string Address { get; set; }
        [Required]
        public string Email { get; set; }
        public List<Sitting> Sittings { get; set; } = new List<Sitting>();
        public List<SittingType> SittingTypes { get; set; } = new List<SittingType>();
        public List<SittingTemplate> SittingTemplates { get; set; } = new List<SittingTemplate>();
        public List<Area> Areas { get; set; } = new List<Area>();
        public List<Person> People { get; set; } = new List<Person>();
        public List<Photo> Photos { get; set; }
    }
}
