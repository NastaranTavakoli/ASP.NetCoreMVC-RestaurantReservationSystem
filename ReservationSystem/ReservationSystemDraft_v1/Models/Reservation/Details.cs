using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using ReservationSystemDraft_v1.Data;

namespace ReservationSystemDraft_v1.Models.Reservation
{
    public class Details
    {
        public Restaurant Restaurant { get; set; }
        public int RestaurantId { get; set; }
        public int Guests { get; set; }
        public DateTime StartTime { get; set; }
        public string SelectedDate { get; set; }
        public string selectedTime { get; set; }
        public int Duration { get; set; }
        public int SittingId { get; set; }
        public Sitting Sitting { get; set; }
        public Person Customer { get; set; }
        public string Notes { get; set; }
        [Required, Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Required, Display(Name = "Last Name")]
        public string LastName { get; set; }
        [Required, DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Required, DataType(DataType.PhoneNumber)]
        public string Phone { get; set; }
    }
}
