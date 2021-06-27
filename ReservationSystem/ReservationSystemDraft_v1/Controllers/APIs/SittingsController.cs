using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReservationSystemDraft_v1.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReservationSystemDraft_v1.Controllers.APIs
{
    [Route("api/[controller]")]
    [ApiController]
    public class SittingsController : ControllerBase
    {

        private readonly ApplicationDbContext _context;

        public SittingsController(ApplicationDbContext context) 
        {
            _context = context;
        }
        [Route("available/{restaurantId}/{year}/{month}")]
        public IActionResult AvailableDates(int restaurantId, int year, int month)
        {
            var start = new DateTime(year, month, 1);
            var end = start.AddMonths(1);
            int numberOfDays = (int)(end - start).TotalDays;

            var sittings = _context.Sittings.Where(s => s.RestaurantId == restaurantId && s.StartTime.Date >= start && s.EndTime.Date < end && s.Active && !s.IsClosedForBooking).ToArray();
            int[] sittingCount = new int[numberOfDays];
            List<string> availableDays = new List<string>();

            for (int i = 0; i < numberOfDays; i++)
            {
                if (sittings.Any(s => s.StartTime.Day == start.AddDays(i).Day))
                {
                    availableDays.Add($"{i + 1}/{month}/{year}");
                }
            }
            return Ok(availableDays); 
        }
    }
}
