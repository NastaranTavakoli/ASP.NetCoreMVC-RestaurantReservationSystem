using ReservationSystemDraft_v1.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReservationSystemDraft_v1.Areas.Admin.Models.Account
{
    public class Index
    {
        public List<Person> Staff { get; set; } = new List<Person>();
        public List<Person> Managers { get; set; } = new List<Person>();
    }
}
