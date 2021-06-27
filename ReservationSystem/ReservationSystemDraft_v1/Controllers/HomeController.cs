using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ReservationSystemDraft_v1.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using ReservationSystemDraft_v1.Areas.Admin.Models.Reservation;
using ReservationSystemDraft_v1.Data;
using ReservationSystemDraft_v1.Models.Home;
using ReservationSystemDraft_v1.Services;
using Index = ReservationSystemDraft_v1.Models.Home.Index;

namespace ReservationSystemDraft_v1.Controllers
{
    public class HomeController : Controller
    {

        private readonly ApplicationDbContext _context;
        private readonly ReservationService _reservationService;

        public HomeController(ApplicationDbContext context, ReservationService reservationService)
        {
            _context = context;
            _reservationService = reservationService;
        }

        public async Task<IActionResult> Index()
        {

            var defaultDate = DateTime.Now.Hour > 17 ? DateTime.Today.AddDays(1) : DateTime.Today;
            var defaultTime = DateTime.Now.Hour > 17
                ? DateTime.Today.AddDays(1).AddHours(19)
                : DateTime.Today.AddHours(19);

            var restaurantsWithSittings = await _context.Restaurants.Include(r => r.Photos).Include(r => r.Sittings).Where(r => r.Sittings.Any(s => s.StartTime.Date == defaultDate && s.EndTime.AddMinutes(-s.DefaultReservationDuration) > defaultTime && s.StartTime <= defaultTime && s.Active && !s.IsClosedForBooking)).Take(20).ToListAsync();

            var availableRestaurants = new List<Restaurant>();
            foreach (var r in restaurantsWithSittings)
            {
                var isAvailable = _reservationService
                    .GetActiveTimeSlots(90, 2, defaultDate, r.Id).Result
                    .Any(a => a.IsAvailable && a.StartTime == defaultTime);
                if (isAvailable)
                {
                    availableRestaurants.Add(r);
                }
            }

            var recentRestaurants = await _context.Restaurants.Take(4).Include(r => r.Photos).OrderByDescending(r => r.Id).ToListAsync();

            var options = new Collection()
            {
                new {Id = 1, Description = "Latest"},
                new {Id = 2, Description = "Oldest"},
                new {Id = 3, Description = "Name Ascending"},
                new {Id = 4, Description = "Name Descending"},
            };

            var m = new Index()
            {
                AvailableRestaurants = availableRestaurants,
                OrderOptions = new SelectList(options, "Id", "Description"),
                Date = defaultDate,
                Time = defaultTime,
                Guests = 2,
                LatestAddedRestaurants = recentRestaurants
            };
            return View(m);
        }


        [HttpPost]
        public async Task<IActionResult> Index(Index m)
        {

            m.SearchString ??= "";
            m.OrderOptionId ??= 1;

            var restaurantsWithSittings = await _context.Restaurants.Include(r => r.Photos).Include(r => r.Sittings).Where(r => r.Sittings.Any(s => s.StartTime.Date == m.Date && s.EndTime.AddMinutes(-s.DefaultReservationDuration) > m.Date.Add(m.Time.TimeOfDay) && s.StartTime <= m.Date.Add(m.Time.TimeOfDay) && s.Active && !s.IsClosedForBooking) && (r.Name.Contains(m.SearchString) || r.Address.Contains(m.SearchString))).ToListAsync();

            var availableRestaurants = new List<Restaurant>();
            foreach (var r in restaurantsWithSittings)
            {
                var isAvailable = _reservationService.GetActiveTimeSlots(90, 2, m.Date, r.Id).Result
                    .Any(a => a.IsAvailable && a.StartTime == m.Date.Add(m.Time.TimeOfDay));
                if (isAvailable)
                {
                    availableRestaurants.Add(r);
                }
            }

            switch (m.OrderOptionId)
            {
                case 2:
                    m.AvailableRestaurants = availableRestaurants.OrderBy(r => r.Id).Take(20).ToList();
                    break;
                case 3:
                    m.AvailableRestaurants = availableRestaurants.OrderBy(r => r.Name).Take(20).ToList();
                    break;
                case 4:
                    m.AvailableRestaurants = availableRestaurants.OrderByDescending(r => r.Name).Take(20).ToList();
                    break;
                default:
                    m.AvailableRestaurants = availableRestaurants.OrderByDescending(r => r.Id).Take(20).ToList();
                    break;
            }

            var recentRestaurants = await _context.Restaurants.Take(4).Include(r => r.Photos).OrderByDescending(r => r.Id).ToListAsync();
            var options = new Collection()
            {
                new {Id = 1, Description = "Latest"},
                new {Id = 2, Description = "Oldest"},
                new {Id = 3, Description = "Name Ascending"},
                new {Id = 4, Description = "Name Descending"},
            };
            m.OrderOptions = new SelectList(options, "Id", "Description");
            m.LatestAddedRestaurants = recentRestaurants;
            return View(m);
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        public IActionResult Business()
        {
            return View();
        }


        public IActionResult RedirectUser()
        {
            if (User.IsInRole("Member"))
            {
                return RedirectToAction("Index", "Home");
            }
            if (User.IsInRole("Staff"))
            {
                return RedirectToAction("Index", "Reservation", new { area = "Admin" });
            }
            return RedirectToAction("Index", "Home");
        }

    }
}
