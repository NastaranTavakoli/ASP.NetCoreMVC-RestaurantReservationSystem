using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ReservationSystemDraft_v1.Data;

namespace ReservationSystemDraft_v1.Services
{
    public class ReservationService
    {
        private readonly ApplicationDbContext _context;

        public ReservationService(ApplicationDbContext context)
        {
            _context = context;
        }

        public class Availability
        {
            public Data.Sitting Sitting { get; set; }
            public DateTime StartTime { get; set; }
            public bool IsAvailable { get; set; }

        }

        public async Task<bool> ConfirmReservation(int id)
        {
            try
            {
                var reservation = await _context.Reservations.FindAsync(id);
                if (reservation == null) return false;
                // Can only confirm pending reservation
                if (reservation.StatusId != 1) return false;
                reservation.StatusId = 2;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> SeatReservation(int id)
        {
            try
            {
                var reservation = await _context.Reservations.FindAsync(id);
                if (reservation == null) return false;
                // Can only seat confirmed reservation
                if (reservation.StatusId != 2) return false;
                reservation.StatusId = 4;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> CompleteReservation(int id)
        {
            try
            {
                var reservation = await _context.Reservations.FindAsync(id);
                if (reservation == null) return false;
                // Can only complete seated reservation
                if (reservation.StatusId != 4) return false;
                reservation.StatusId = 5;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> CancelReservation(int id)
        {
            try
            {
                var reservation = await _context.Reservations.FindAsync(id);
                if (reservation == null) return false;                
                reservation.StatusId = 3;
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }


        public async Task<List<Availability>> GetActiveTimeSlots(int duration, int guests, DateTime selectedDate, int restaurantId)
        {
            var availableSittings = await _context.Sittings.Where(s =>
                s.RestaurantId == restaurantId && s.StartTime.Date == selectedDate.Date &&
                (s.EndTime.AddMinutes(-duration)) > DateTime.Now && s.Active).Include(s => s.SittingType).Include(s => s.Reservations).ToListAsync();

            var availabilities = new List<Availability>();

            foreach (var sitting in availableSittings)
            {                
                var timeSlots = new List<DateTime>();
                for (var i = sitting.StartTime; i < sitting.EndTime; i = i.AddMinutes(sitting.Increment))
                {
                    timeSlots.Add(i);
                }
                if (sitting.IsClosedForBooking)
                {
                    availabilities.AddRange(timeSlots.Select(ts => new Availability { Sitting = sitting, StartTime = ts }));
                    continue;
                }
                var availableSlots = new List<DateTime>();
                foreach (var timeSlot in timeSlots)
                {
                    var occupancy = 0;
                    foreach (var r in sitting.Reservations)
                    {
                        if (r.StartTime <= timeSlot && r.EndTime > timeSlot)
                        {
                            occupancy += r.Guests;
                        }
                    }
                    if (sitting.Capacity - occupancy >= guests && timeSlot > DateTime.Now)
                    {
                        availableSlots.Add(timeSlot);
                    }
                    else
                    {
                        availabilities.Add(new Availability() { Sitting = sitting, StartTime = timeSlot });
                    }
                }

                var requiredConsecutiveSlots = duration / sitting.Increment;

                bool isConsecutive = false;

                foreach (var timeSlot in availableSlots)
                {
                    for (int i = 0; i < requiredConsecutiveSlots; i++)
                    {
                        isConsecutive = false;
                        if (availableSlots.Contains(timeSlot.AddMinutes(sitting.Increment * i)))
                        {
                            isConsecutive = true;
                        }
                    }
                    if (isConsecutive)
                    {
                        availabilities.Add(new Availability() { Sitting = sitting, StartTime = timeSlot, IsAvailable = true });
                    }
                }
            }

            var sortedAvailabilities = availabilities.OrderBy(a => a.StartTime).ToList();

            return sortedAvailabilities;
        }

    }
}
