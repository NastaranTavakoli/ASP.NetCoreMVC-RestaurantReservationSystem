using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ReservationSystemDraft_v1.Data;
using ReservationSystemDraft_v1.Helpers;
using ReservationSystemDraft_v1.Services;

namespace ReservationSystemDraft_v1.Controllers.APIs
{
    [Route("api/[controller]")]
    [ApiController]
    public class RestaurantsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ReservationService _reservationService;

        public RestaurantsController(ApplicationDbContext context, ReservationService reservationService)
        {
            _context = context;
            _reservationService = reservationService;
        }


        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] RestaurantQueryObject m)
        {
            m.PageNumber ??= 1;
            m.SearchValue ??= "";
            var selectedDate = (m.Date == null) ? DateTime.Today.AddDays(1) : DateTime.Parse(m.Date);
            var selectedTime = (m.Time == null) ? DateTime.Today.AddDays(1).AddHours(9) : selectedDate.Add(TimeSpan.Parse(m.Time));
            m.Guests ??= 2;

            var restaurantsWithSittings = await _context.Restaurants.Include(r => r.Photos).Include(r => r.Sittings).Where(r => r.Sittings.Any(s => s.StartTime.Date == selectedDate && s.EndTime.AddMinutes(-s.DefaultReservationDuration) > selectedTime && s.StartTime <= selectedTime && s.Active && !s.IsClosedForBooking) && (r.Name.Contains(m.SearchValue) || r.Address.Contains(m.SearchValue))).ToListAsync();

            var availableRestaurants = new List<Restaurant>();
            foreach (var r in restaurantsWithSittings)
            {
                var isAvailable = _reservationService.GetActiveTimeSlots(90, m.Guests.Value, selectedDate, r.Id).Result
                    .Any(a => a.IsAvailable && a.StartTime == selectedTime);
                if (isAvailable)
                {
                    availableRestaurants.Add(r);
                }
            }

            var availableRestaurantsToShow = availableRestaurants.Skip((m.PageNumber.Value - 1) * 10).Take(10).Select(r => new { r.Address, r.Email, r.Phone, r.Name, r.Id, Photos = r.Photos.Select(p => p.FileName).ToList() }).ToList();

            var recentRestaurants = await _context.Restaurants.Take(10).Include(r => r.Photos).OrderByDescending(r => r.Id).Select(r => new
            {
                r.Address,
                r.Email,
                r.Phone,
                r.Name,
                r.Id,
                Photos = r.Photos.Select(p => p.FileName).ToList()
            }).ToListAsync();

            var result = new { availableRestaurantsToShow, recentRestaurants };
            return Ok(result);
        }



        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var restaurant = await _context.Restaurants.Include(r => r.Photos).FirstOrDefaultAsync(r => r.Id == id);
            if (restaurant == null)
            {
                return NotFound();
            }
            var result = new
            {
                restaurant.Id, restaurant.Email, restaurant.Address, restaurant.Name, restaurant.Phone,
                Photos = restaurant.Photos.Select(p => new {p.FileName})
            };
            return Ok(result);
        }


    }
}
