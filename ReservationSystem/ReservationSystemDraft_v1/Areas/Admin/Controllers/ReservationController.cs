using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using ReservationSystemDraft_v1.Areas.Admin.Models.Reservation;
using ReservationSystemDraft_v1.Data;
using ReservationSystemDraft_v1.Services;
using Create = ReservationSystemDraft_v1.Areas.Admin.Models.Reservation.Create;

namespace ReservationSystemDraft_v1.Areas.Admin.Controllers
{
    public class ReservationController : AdminBaseController
    {

        private readonly ReservationService _reservationService;


        public ReservationController(ApplicationDbContext context, UserManager<IdentityUser> userManager, IMapper mapper, ReservationService reservationService) : base(context, userManager, mapper)
        {
            _reservationService = reservationService;
        }

        public async Task<IActionResult> Index(DateTime? date = null, int? sittingId = null, string searchString = null)
        {
            var iu = await _userManager.FindByNameAsync(User.Identity.Name);
            var admin = _context.People.SingleOrDefault(p => iu.Id == p.UserId);

            var m = new DaysReservations();
            m.DateTime = date ?? DateTime.Now;
            m.SearchString = searchString == null ? "" : searchString.ToLower();

            m.Sittings = await _context.Sittings
                .Include(s => s.SittingType)
                .Where(s => s.RestaurantId == admin.RestaurantId 
                    && s.Active 
                    && s.StartTime.Date == m.DateTime.Date)
                .OrderBy(s => s.StartTime)
                .ToListAsync();

            if (!m.Sittings.Any(s => s.EndTime > m.DateTime))
            {
                m.SittingId = 0;
            }
            else
            {
                m.SittingId = sittingId ?? m.Sittings.Where(s => s.EndTime > m.DateTime).OrderBy(s => s.StartTime).ToArray()[0].Id;
            }

            var reservations = await _context.Reservations
                .Include(r => r.Customer)
                .Include(r => r.Status)
                .Include(r => r.ReservationTables)
                    .ThenInclude(rt => rt.Table)
                .Where(r => r.SittingId == m.SittingId
                    && r.StartTime.Date == m.DateTime.Date
                    && r.StatusId != 3
                    && (string.IsNullOrEmpty(searchString) || r.Customer.FirstName.ToLower().Contains(m.SearchString) || r.Customer.LastName.ToLower().Contains(m.SearchString) || r.ConfirmationCode.Contains(m.SearchString)))
                .ToListAsync();

            m.Areas = await _context.Areas.Where(a => a.RestaurantId == admin.RestaurantId).ToListAsync();
            m.Reservations = reservations;
            return View(m);
        }

        public async Task<IActionResult> Search(DateTime? startDate = null, DateTime? endDate = null, int orderOptionId = 1, string searchString = null, int pageNumber = 1)
        {   
            //Date range for reservations to pull up for index
            var start = startDate ?? DateTime.Today;
            var end = endDate ?? DateTime.Today;
            //End date is inclusive, so make it 1 minute before midnight
            end = end.AddDays(1).AddMinutes(-1);

            //Details of logged in admin
            var iu = await _userManager.FindByNameAsync(User.Identity.Name);
            var admin = _context.People.SingleOrDefault(p => iu.Id == p.UserId);

            //Query to pull up reservations
            var query = _context.Reservations.Include(r => r.Customer).Include(r => r.Status).Where(r =>
                r.Sitting.RestaurantId == admin.RestaurantId && r.StartTime >= start && r.StartTime < end);

            //If user is searching
            if (searchString != null)
            {
                searchString = searchString.ToLower();
                query = query.Where(r => r.Customer.FirstName.ToLower().Contains(searchString) || r.Customer.LastName.ToLower().Contains(searchString) || r.Customer.Email.ToLower().Contains(searchString) || r.ConfirmationCode.Contains(searchString));
            }

            ////Take the first ten
            //var reservations = await query.Take(10).ToListAsync();

            //Execute query based on orderBy and page number
            var reservations = new List<Reservation>();
            switch (orderOptionId)
            {
                case 2:
                    reservations = await query.OrderByDescending(r => r.StartTime).Skip((pageNumber - 1) * 10).Take(10).ToListAsync();
                    break;
                case 3:
                    reservations = await query.OrderBy(r => r.Customer.FirstName).Skip((pageNumber - 1) * 10).Take(10).ToListAsync();
                    break;
                case 4:
                    reservations = await query.OrderByDescending(r => r.Customer.FirstName).Skip((pageNumber - 1) * 10).Take(10).ToListAsync();
                    break;
                case 5:
                    reservations = await query.OrderBy(r => r.StartTime).OrderByDescending(r => r.ActionRequired).Skip((pageNumber - 1) * 10).Take(10).ToListAsync();
                    break;
                default:
                    reservations = await query.OrderBy(r => r.StartTime).Skip((pageNumber - 1) * 10).Take(10).ToListAsync();
                    break;
            }

            //Create options for sorting
            var options = new Collection()
            {
                new {Id = 1, Description = "Soonest"},
                new {Id = 2, Description = "Latest"},
                new {Id = 3, Description = "Name Ascending"},
                new {Id = 4, Description = "Name Descending"},
                new {Id = 5, Description = "Require Action"},
            };
            //View model
            var m = new GetReservations()
            {
                Reservations = reservations,
                StartDate = start,
                EndDate = end,
                OrderOptions = new SelectList(options, "Id", "Description"),
                TotalPages = query.Count() / 10 + 1,
                PageNumber = pageNumber,
                SearchString = searchString,
                OrderOptionId = orderOptionId
            };
            return View(m);
        }        

        public async Task<IActionResult> Confirm(int id, string returnUrl = null)
        {
            returnUrl ??= "Admin/Reservation";
            bool success = await _reservationService.ConfirmReservation(id);
            if (!success) return BadRequest();
            return LocalRedirect(returnUrl);
        }

        public async Task<IActionResult> Seat(int id, string returnUrl = null)
        {
            returnUrl ??= "Admin/Reservation";
            bool success = await _reservationService.SeatReservation(id);
            if (!success) return BadRequest();
            return LocalRedirect(returnUrl);
        }

        public async Task<IActionResult> Complete(int id, string returnUrl = null)
        {
            returnUrl ??= "Admin/Reservation";
            bool success = await _reservationService.CompleteReservation(id);
            if (!success) return BadRequest();
            return LocalRedirect(returnUrl);
        }

        public async Task<IActionResult> Cancel(int id, string returnUrl = null)
        {
            returnUrl ??= "Admin/Reservation";
            bool success = await _reservationService.CancelReservation(id);
            if (!success) return BadRequest();
            return LocalRedirect(returnUrl);
        }

        public async Task<IActionResult> Pending(GetReservations m)
        {
            m.SearchString ??= "";
            var iu = await _userManager.FindByNameAsync(User.Identity.Name);
            var admin = _context.People.SingleOrDefault(p => iu.Id == p.UserId);
            var pendingReservations = await _context.Reservations.Include(r => r.Customer)
                .Where(r => r.Sitting.RestaurantId == admin.RestaurantId 
                    && r.StatusId == 1 
                    && (r.Customer.FirstName.Contains(m.SearchString) || r.Customer.LastName.Contains(m.SearchString) || r.ConfirmationCode.Contains(m.SearchString)))
                .OrderBy(r => r.StartTime)
                .ToListAsync();
            m.Reservations = pendingReservations;

            return View(m);
        }        

        public async Task<IActionResult> Details(int id, Edit m, string returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl ?? "/Admin/Reservation";
            var reservation = await _context.Reservations
                .Include(r => r.ReservationTables)
                    .ThenInclude(rt => rt.Table)
                .Include(r => r.Customer)
                .Include(r => r.Sitting)
                .SingleOrDefaultAsync(r => r.Id == id);

            if (m.Customer == null)
            {
                m = new Edit()
                {
                    Id = reservation.Id,
                    SittingId = reservation.SittingId,
                    OldSittingId = reservation.SittingId,
                    StartTime = reservation.StartTime,
                    OldStartTime = reservation.StartTime,
                    SittingTypeId = reservation.Sitting.SittingTypeId,
                    OldSittingTypeId = reservation.Sitting.SittingTypeId,
                    Date = reservation.StartTime,
                    Duration = reservation.Duration,
                    OldDuration = reservation.Duration,
                    Guests = reservation.Guests,
                    OldGuests = reservation.Guests,
                    Notes = reservation.Notes,
                    Customer = reservation.Customer,
                    StatusId = reservation.StatusId,
                };
            }
            return View(m);
        }

        public async Task<IActionResult> EditTime(Edit m, string returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl ?? "/Admin/Reservation";
            var iu = await _userManager.FindByNameAsync(User.Identity.Name);
            var restaurantId = _context.People.SingleOrDefault(p => iu.Id == p.UserId).RestaurantId;

            var durations = new Collection();
            for (int i = 90; i <= 240; i += 15)
            {
                durations.Add(new { Id = i, Description = new TimeSpan(i / 60, i % 60, 0).ToString(@"hh\:mm") });
            }
            var durationOptions = new SelectList(durations, "Id", "Description");

            m.DurationOptions = durationOptions;
            m.Availabilities = await _reservationService.GetActiveTimeSlots(m.Duration, m.Guests, m.Date, restaurantId.Value);
            return View(m);
        }


        [HttpPost]
        public async Task<IActionResult> DiscardUpdate(Edit m, string returnUrl = null)
        {
            var customer = _context.People.SingleOrDefault(p => p.Email == m.Customer.Email);
            if (customer == null)
            {
                customer = new Person()
                {
                    Email = m.Customer.Email,
                };
                await _context.People.AddAsync(customer);
            }
            customer.FirstName = m.Customer.FirstName;
            customer.LastName = m.Customer.LastName;
            customer.Phone = m.Customer.Phone;
            customer.Notes = m.Notes;

            var reservation = _context.Reservations.Include(r => r.Customer).SingleOrDefault(r => r.Id == m.Id);
            reservation.Customer = customer;
            reservation.StartTime = m.OldStartTime;
            reservation.Duration = m.OldDuration;
            reservation.Guests = m.OldGuests;
            reservation.SittingId = m.OldSittingId;
            reservation.Notes = m.Notes;
            reservation.ActionRequired = false;
            await _context.SaveChangesAsync();
            if (returnUrl != null)
            {
                return LocalRedirect(returnUrl);
            }
            return RedirectToAction(nameof(Index));
        }


        [HttpPost]
        public async Task<IActionResult> Update(Edit m, TimeSpan timeSlot = new TimeSpan(), string returnUrl = null)
        {
            var customer = _context.People.SingleOrDefault(p => p.Email == m.Customer.Email);
            if (customer == null)
            {
                customer = new Person()
                {
                    Email = m.Customer.Email,
                };
                await _context.People.AddAsync(customer);
            }
            customer.FirstName = m.Customer.FirstName;
            customer.LastName = m.Customer.LastName;
            customer.Phone = m.Customer.Phone;
            customer.Notes = m.Notes;

            var reservation = _context.Reservations.Include(r => r.Customer).SingleOrDefault(r => r.Id == m.Id);
            reservation.Customer = customer;
            reservation.StartTime = m.Date.Add(m.StartTime.TimeOfDay);
            reservation.Duration = m.Duration;
            reservation.Guests = m.Guests;
            reservation.SittingId = m.SittingId;
            reservation.Notes = m.Notes;
            reservation.StatusId = m.StatusId;
            reservation.ActionRequired = false;
            await _context.SaveChangesAsync();
            if (returnUrl != null)
            {
                return LocalRedirect(returnUrl);
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> SelectTime()
        {
            var iu = await _userManager.FindByNameAsync(User.Identity.Name);
            var restaurantId = _context.People.SingleOrDefault(p => iu.Id == p.UserId).RestaurantId;

            var restaurant = _context.Restaurants.Include(r => r.Sittings).ThenInclude(s => s.SittingType)
                .SingleOrDefault(r => r.Id == restaurantId);

            var durations = new Collection();
            for (int i = 90; i <= 240; i += 15)
            {
                durations.Add(new { Id = i, Description = new TimeSpan(i / 60, i % 60, 0).ToString(@"hh\:mm") });
            }
            var durationOptions = new SelectList(durations, "Id", "Description");

            var m = new Models.Reservation.Create()
            {
                DurationOptions = durationOptions,
                RestaurantName = restaurant.Name,
                RestaurantId = restaurantId.Value,
                Guests = 2,
                Availabilities = await _reservationService.GetActiveTimeSlots(90, 2, DateTime.Now, restaurantId.Value),
            };
            return View(m);
        }

        [HttpPost]
        public async Task<IActionResult> SelectTime(Create m)
        {
            var restaurant = _context.Restaurants.Include(r => r.Sittings).ThenInclude(s => s.SittingType)
                .SingleOrDefault(r => r.Id == m.RestaurantId);

            var durations = new Collection();
            for (int i = 90; i <= 240; i += 15)
            {
                durations.Add(new { Id = i, Description = new TimeSpan(i / 60, i % 60, 0).ToString(@"hh\:mm") });
            }
            var durationOptions = new SelectList(durations, "Id", "Description");

            m.DurationOptions = durationOptions;
            m.RestaurantName = restaurant.Name;
            m.Availabilities = await _reservationService.GetActiveTimeSlots(m.Duration, m.Guests, m.Date, m.RestaurantId);

            return View(m);
        }
        
        public async Task<IActionResult> ConfirmDetails(DateTime date, int restaurantId, int guests, int duration, int sittingId, TimeSpan timeSlot)
        {
            var reservationSources = await _context.ReservationSources.ToListAsync();
            reservationSources.RemoveAt(0);
            var sourceOptions = new SelectList(reservationSources, nameof(ReservationSource.Id),
                nameof(ReservationSource.Description));
            var m = new Create();
            m.Date = date;
            m.Guests = guests;
            m.Duration = duration;
            m.SittingId = sittingId;
            m.RestaurantId = restaurantId;
            m.SourceTypes = sourceOptions;
            m.StartTime = m.Date.Add(timeSlot);
            return View(m);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Create m)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var customer = _context.People.Include(c => c.Reservations).SingleOrDefault(p => p.Email == m.Email);
                    if (customer == null)
                    {
                        customer = new Person()
                        {
                            Email = m.Email,
                        };
                        await _context.People.AddAsync(customer);
                    }

                    customer.FirstName = m.FirstName;
                    customer.LastName = m.LastName;
                    customer.Phone = m.Phone;
                    customer.Notes = m.Notes;

                    var rnd = new Random();
                    await _context.SaveChangesAsync();
                    var reservation = new Reservation()
                    {
                        SittingId = m.SittingId,
                        CustomerId = customer.Id,
                        Guests = m.Guests,
                        StartTime = m.StartTime,
                        Duration = m.Duration,
                        Notes = m.Notes,
                        SourceId = m.SourceId,
                        StatusId = 1,
                        ConfirmationCode = $"{customer.Id}{m.SittingId}{(customer.Reservations != null ? customer.Reservations.Count : 0)}{rnd.Next(1000000, 9999999)}",
                    };

                    await _context.Reservations.AddAsync(reservation);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index), "Reservation", new { area = "Admin", date = reservation.StartTime.Date, sittingId = reservation.SittingId });
                }
                catch (Exception)
                {
                    ModelState.AddModelError("Error", "An error occurred when trying to make this reservation. Please try again.");
                }
            }
            return View("ConfirmDetails", m);
        }
    }
}
