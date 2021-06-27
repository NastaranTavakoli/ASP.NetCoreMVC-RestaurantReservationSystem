using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ReservationSystemDraft_v1.Data;
using ReservationSystemDraft_v1.Services;

namespace ReservationSystemDraft_v1.Controllers
{
    public class ReservationController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IMapper _mapper;
        private readonly ReservationService _reservationService;

        public ReservationController(ApplicationDbContext context, UserManager<IdentityUser> userManager, IMapper mapper, ReservationService reservationService)
        {
            _context = context;
            _userManager = userManager;
            _mapper = mapper;
            _reservationService = reservationService;
        }

        public async Task<IActionResult> Index()
        {
            var restaurants = await _context.Restaurants.Take(20).Include(r => r.Photos).OrderByDescending(r => r.Id).ToListAsync();
            return View(restaurants);
        }

        public async Task<IActionResult> SelectDateTime(int? restaurantId, string selectedDate, int guests = 2)
        {
            var date = selectedDate == null ? DateTime.Today : DateTime.Parse(selectedDate);
            var restaurant = await _context.Restaurants.Include(r => r.Sittings).ThenInclude(s => s.SittingType).Include( r => r.Photos).FirstOrDefaultAsync(r => r.Id == restaurantId);
            if (restaurant == null) return NotFound();
            var m = new Models.Reservation.Create
            {
                RestaurantId = restaurant.Id,
                Restaurant = restaurant,
                SelectedDate = date,
                Guests = guests
            };
            //default duration of one reservation is set to 90 minutes
            var timeslots = await _reservationService.GetActiveTimeSlots(90, m.Guests, m.SelectedDate, restaurant.Id);
            m.Availabilities = timeslots.Where(t => t.IsAvailable).ToList();
            return View(m);
        }

        [HttpPost]
        public async Task<IActionResult> SelectDateTime(Models.Reservation.Create m)
        {
            //default duration of one reservation is set to 90 minutes
            var timeslots = await _reservationService.GetActiveTimeSlots(90, m.Guests, m.SelectedDate, m.RestaurantId);
            m.Availabilities = timeslots.Where(t => t.IsAvailable).ToList();
            m.Restaurant = await _context.Restaurants.Include(r => r.Photos)
                .FirstOrDefaultAsync(r => r.Id == m.RestaurantId);
            return View(m);
        }

        public async Task<IActionResult> Details(int? restaurantId, string? dateTime, int? guest)
        {
            var convertedDateTime = DateTime.Parse(dateTime);

            //Find a sitting by selected date and time
            var selectedSitting = await _context.Sittings.Where(s => s.RestaurantId == restaurantId).FirstOrDefaultAsync(s => s.StartTime <= convertedDateTime && s.EndTime > convertedDateTime);

            var m = new Models.Reservation.Details
            {
                RestaurantId = selectedSitting.RestaurantId,
                Guests = (int)guest,
                StartTime = convertedDateTime,
                Duration = selectedSitting.DefaultReservationDuration,
                SittingId = selectedSitting.Id
            };

            if (User.IsInRole("Member"))
            {
                var iu = await _userManager.FindByNameAsync(User.Identity.Name);
                var member = _context.People.SingleOrDefault(p => iu.Id == p.UserId);
                //update the properties values of Details object without creating a new one
                //_mapper.Map<Person, Models.Reservation.Details>(member, m);
                m.Customer = member;
                m.FirstName = member.FirstName;
                m.LastName = member.LastName;
                m.Phone = member.Phone;
                m.Email = member.Email;
            }
            m.Restaurant = await _context.Restaurants.FirstOrDefaultAsync(r => r.Id == m.RestaurantId);
            return View(m);
        }

        [HttpPost]
        public async Task<IActionResult> Confirm(Models.Reservation.Details m)
        {
            if (ModelState.IsValid)
            {
                var sitting = await _context.Sittings.FindAsync(m.SittingId);
                var existingPerson = _context.People.FirstOrDefault(p => p.Email == m.Email);

                Person temp = null;
                try
                {
                    if (existingPerson == null)
                    {
                        var newPerson = _mapper.Map<Person>(m);
                        await _context.People.AddAsync(newPerson);
                        await _context.SaveChangesAsync();
                        temp = newPerson;
                    }
                    else
                    {
                        _mapper.Map(m, existingPerson);
                        await _context.SaveChangesAsync();
                        temp = existingPerson;
                    }

                    var r = _mapper.Map<Reservation>(m);
                    r.SourceId = 1;
                    r.StatusId = 1;
                    r.Customer = temp;
                    r.Duration = sitting.DefaultReservationDuration;

                    var rnd = new Random();
                    r.ConfirmationCode = $"{r.CustomerId}{m.SittingId}{r.Customer.Reservations.Count}{rnd.Next(1000000, 9999999)}";

                    await _context.Reservations.AddAsync(r);
                    await _context.SaveChangesAsync();

                    var createdReservationId = r.Id;
                    return RedirectToAction(nameof(Confirmed), new { id = createdReservationId });
                }
                catch (Exception ex)
                {
                    //if creating a Reservation fails, delete the new person from Person table.
                    if (existingPerson == null && temp != null)
                    {
                        _context.People.Remove(temp);
                        await _context.SaveChangesAsync();
                    }
                    ModelState.AddModelError("Exception", ex.Message);
                }
            }
            m.Restaurant = await _context.Restaurants.FirstOrDefaultAsync(r => r.Id == m.RestaurantId);
            return View(nameof(Details), m);
        }

        public async Task<IActionResult> Confirmed(int? id)
        {
            var reservation = await _context.Reservations
                .Include(r => r.Customer)
                .Include(r => r.Sitting)
                    .ThenInclude(s => s.Restaurant)
                .Include(r => r.Sitting)
                    .ThenInclude(s => s.SittingType)
                .FirstOrDefaultAsync(r => r.Id == id);
            if (reservation == null) return NotFound();
            return View(reservation);
        }


    }
}



