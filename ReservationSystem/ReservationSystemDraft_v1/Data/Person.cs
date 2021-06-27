using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ReservationSystemDraft_v1.Data
{
    public class Person
    {
        public int Id { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        public string FullName => $"{FirstName} {LastName}";

        [Required]
        public string Email { get; set; }
        [Required]
        public string Phone { get; set; }
        public string UserId { get; set; }
        public string Notes { get; set; }
        public int? RestaurantId { get; set; }
        public Restaurant Restaurant { get; set; }
        public List<Reservation> Reservations { get; set; } = new List<Reservation>();
    }
}
