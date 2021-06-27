using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ReservationSystemDraft_v1.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReservationSystemDraft_v1.Areas.Admin.Controllers
{
    [Authorize(Roles = "Manager")]
    public class SittingController : AdminBaseController
    {
        public SittingController(ApplicationDbContext context, UserManager<IdentityUser> userManager, IMapper mapper) : base(context, userManager, mapper)
        {
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var userProfile = await _context.People
                .FirstOrDefaultAsync(p => p.UserId == user.Id);
            if (user == null) return RedirectToPage("Login");
            var restaurant = await _context.Restaurants
                .Include(r => r.SittingTemplates.Where(st => st.Active && st.EndDate > DateTime.Today))
                .Include(r => r.Sittings.Where(s => s.Active))
                .FirstOrDefaultAsync(r => r.Id == userProfile.RestaurantId);
            if (restaurant == null) return NotFound();            

            var sittingTypes = await _context.SittingTypes
                .Where(st => st.RestaurantId == restaurant.Id)
                .ToListAsync();
            ViewBag.Restaurant = restaurant.Name;
            var m = new Models.Sitting.Index
            {
                SittingTypes = sittingTypes,
                SittingTemplates = restaurant.SittingTemplates,
                Sittings = restaurant.Sittings,
                Date = DateTime.Today
            };
            return View(m);
        }

        public async Task<IActionResult> NewSitting()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var userProfile = await _context.People
                .FirstOrDefaultAsync(p => p.UserId == user.Id);
            var restaurant = await _context.Restaurants.FindAsync(userProfile.RestaurantId);
            var sittingTypes = await _context.SittingTypes
                .Where(st => st.RestaurantId == restaurant.Id)
                .ToListAsync();
            var options = new SelectList(sittingTypes, nameof(SittingType.Id), nameof(SittingType.Description));
            var now = DateTime.Now;
            var m = new Models.Sitting.Create
            {
                RestaurantId = restaurant.Id,
                SittingTypes = options,
                StartTime = new DateTime(now.Year, now.Month, now.Day, 6, 0, 0),
                EndTime = new DateTime(now.Year, now.Month, now.Day, 12, 0, 0),
                IsClosedForBooking = false,
                Increment = 15,
                DefaultReservationDuration = 90,
                OneOffSitting = false,
                DaysOfWeek = new bool[] { true, true, true, true, true, true, true },
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddDays(90)
            };
            return View(m);
        }

        [HttpPost]
        public async Task<IActionResult> NewSitting(Models.Sitting.Create m)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (m.StartDate < DateTime.Today) throw new Exception("Scheduled start date cannot be in the past.");

                    if (m.SittingTypeId == null && string.IsNullOrEmpty(m.NewSittingType))
                    {
                        throw new Exception("You must specify a sitting type.");
                    }

                    SittingType sittingType;

                    if (!string.IsNullOrEmpty(m.NewSittingType))
                    {
                        sittingType = new SittingType
                        {
                            Description = m.NewSittingType,
                            RestaurantId = m.RestaurantId
                        };
                        _context.SittingTypes.Add(sittingType);     
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        sittingType = await _context.SittingTypes.FindAsync(m.SittingTypeId);
                    }

                    var sittings = new List<Sitting>();

                    if (m.OneOffSitting)
                    {
                        var startTime = m.StartDate.Date.Add(m.StartTime.TimeOfDay);
                        var endTime = m.StartDate.Date.Add(m.EndTime.TimeOfDay);
                        var name = m.Name ?? $"{sittingType.Description} {startTime.ToLongDateString()}";
                        if (m.EndTime.TimeOfDay < m.StartTime.TimeOfDay) endTime = endTime.AddDays(1);
                        var sitting = new Sitting
                        {
                            RestaurantId = m.RestaurantId,
                            SittingTypeId = sittingType.Id,
                            SittingTemplateId = null,
                            Name = name,
                            StartTime = startTime.ToUniversalTime(),
                            EndTime = endTime,
                            Increment = m.Increment,
                            DefaultReservationDuration = m.DefaultReservationDuration,
                            Capacity = m.Capacity,
                            IsClosedForBooking = m.IsClosedForBooking,
                            Active = true
                        };
                        _context.Sittings.Add(sitting);
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        if (m.StartDate > m.EndDate) throw new Exception("Scheduled end date must be after start date.");

                        var name = m.Name ?? $"Custom {sittingType.Description} Schedule";
                        var template = new SittingTemplate
                        {
                            Name = name,
                            RestaurantId = m.RestaurantId,
                            SittingTypeId = sittingType.Id,
                            StartDate = m.StartDate,
                            EndDate = m.EndDate,
                            StartTime = m.StartTime,
                            EndTime = m.EndTime,
                            Capacity = m.Capacity,
                            Increment = m.Increment,
                            DefaultReservationDuration = m.DefaultReservationDuration,
                            Active = true
                        };
                        _context.SittingTemplates.Add(template);
                        await _context.SaveChangesAsync();

                        // Start from provided start date
                        var date = m.StartDate;
                        // Create sitting until provided end date
                        while (date <= m.EndDate)
                        {
                            int day = (int)date.DayOfWeek;
                            bool sittingOnThisDay = m.DaysOfWeek[day];
                            //Only create sitting for days of week indicated
                            if (sittingOnThisDay)
                            {
                                var startTime = date.Date.Add(m.StartTime.TimeOfDay);
                                var endTime = date.Date.Add(m.EndTime.TimeOfDay);
                                if (m.EndTime.TimeOfDay < m.StartTime.TimeOfDay) endTime = endTime.AddDays(1);
                                var sitting = new Sitting
                                {
                                    RestaurantId = m.RestaurantId,
                                    SittingTypeId = sittingType.Id,
                                    SittingTemplateId = template.Id,
                                    Name = $"{name} {startTime.ToLongDateString()}",
                                    StartTime = startTime,
                                    EndTime = endTime,
                                    Increment = m.Increment,
                                    DefaultReservationDuration = m.DefaultReservationDuration,
                                    Capacity = m.Capacity,
                                    IsClosedForBooking = m.IsClosedForBooking,
                                    Active = true
                                };
                                sittings.Add(sitting);
                            }
                            date = date.AddDays(1);
                        }
                        await _context.Sittings.AddRangeAsync(sittings);
                        await _context.SaveChangesAsync();
                    }
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("Error", ex.Message);
                }
            }

            var sittingTypes = await _context.SittingTypes.Where(st => st.RestaurantId == m.RestaurantId).ToListAsync();
            m.SittingTypes = new SelectList(sittingTypes, nameof(SittingType.Id), nameof(SittingType.Description));
            return View(m);
        }

        public async Task<IActionResult> Details(int sittingId, int pageNumber = 1, string returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl ?? "/Admin/Sitting";

            var reservations = await _context.Reservations
                .Where(r => r.SittingId == sittingId && r.StatusId != 3)
                .ToListAsync();            

            int totalReservations = reservations.Count();
            var sitting = await _context.Sittings
                .Include(s => s.Restaurant)
                .Include(s => s.SittingType)
                .Include(s => s.SittingTemplate)
                .Include(s => s.Reservations.Where(r => r.StatusId != 3).OrderBy(r => r.StartTime).Skip(10 * (pageNumber - 1)).Take(10)) 
                .FirstOrDefaultAsync(s => s.Id == sittingId);

            var sittingDuration = sitting.EndTime - sitting.StartTime;
            var covers = reservations.Sum(r => r.Guests);
            var atCapacity = reservations.Sum(r => r.Guests / (double)sitting.Capacity * new TimeSpan(0, r.Duration, 0) / sittingDuration * 100);
            
            var m = _mapper.Map<Models.Sitting.Details>(sitting);
            m.TotalReservations = totalReservations;
            m.PageNumber = pageNumber;
            m.AtCapacity = Math.Round(atCapacity);
            return View(m);
        }

        [HttpPost]
        public async Task<IActionResult> EditSitting(Models.Sitting.Details m, string returnUrl)
        {
            returnUrl ??= "/Admin/Sitting";
            ViewBag.ReturnUrl = returnUrl;
            var originalSitting = await _context.Sittings
                .Include(s => s.SittingType)
                .Include(s => s.Reservations.Where(r => r.StatusId != 3))
                .FirstOrDefaultAsync(s => s.Id == m.Id);
            if (originalSitting == null) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var editedSitting = _mapper.Map<Sitting>(m);
                    if (originalSitting.Reservations.Sum(r => r.Guests) > editedSitting.Capacity)
                    {
                        throw new Exception("The new capacity would not accommodate the existing reservations.");
                    }
                    originalSitting.Reservations.ForEach(r =>
                    {
                        if (r.StartTime < editedSitting.StartTime || r.StartTime.AddMinutes(r.Duration) > editedSitting.EndTime)
                        {
                            throw new Exception("The new sitting times would conflict with existing bookings.");
                        }
                    });
                    originalSitting.StartTime = editedSitting.StartTime;
                    originalSitting.EndTime = editedSitting.EndTime;
                    originalSitting.Capacity = editedSitting.Capacity;
                    originalSitting.DefaultReservationDuration = editedSitting.DefaultReservationDuration;
                    originalSitting.Increment = editedSitting.Increment;
                    originalSitting.IsClosedForBooking = editedSitting.IsClosedForBooking;
                    _context.Sittings.Update(originalSitting);
                    await _context.SaveChangesAsync();
                    return LocalRedirect(returnUrl);
                } catch (Exception ex)
                {
                    ModelState.AddModelError("Error", ex.Message);
                }
            }
            m.SittingType = originalSitting.SittingType;
            m.Reservations = originalSitting.Reservations;
            return View("Details", m);
        }

        public async Task<IActionResult> ViewByDate(DateTime date)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var userProfile = await _context.People
                .FirstOrDefaultAsync(p => p.UserId == user.Id);
            var sittings = await _context.Sittings
                .Include(s => s.SittingType)
                .Include(s => s.Reservations.Where(r => r.StatusId != 3))
                .Where(s => s.RestaurantId == userProfile.RestaurantId && s.Active && s.StartTime.Date == date.Date)
                .OrderBy(s => s.StartTime)
                .ToListAsync();
            var m = new Models.Sitting.ViewByDate
            {
                Date = date,
                Sittings = sittings
            };
            return View(m);
        }        

        public async Task<IActionResult> ViewByType(int templateId, int pageNumber = 1)
        {
            int totalSittings = await _context.Sittings
                .Where(s => s.SittingTemplateId == templateId && s.Active && s.StartTime.Date >= DateTime.Today)
                .CountAsync();
            var temp = await _context.SittingTemplates
                .Include(t => t.SittingType)
                .Include(t => t.Sittings
                    .Where(s => s.SittingTemplateId == templateId && s.Active && s.StartTime.Date >= DateTime.Today)
                    .OrderBy(s => s.StartTime)
                    .Skip(10 * (pageNumber - 1))
                    .Take(10))
                .ThenInclude(s => s.Reservations.Where(r => r.StatusId != 3))                    
                .FirstOrDefaultAsync(t => t.Id == templateId);          
            var m = new Models.Sitting.ViewByType
            {
                TemplateId = temp.Id,
                Type = temp.SittingType.Description,
                Sittings = temp.Sittings,
                TotalSittings = totalSittings,
                PageNumber = pageNumber
            };
            return View(m);
        }

        public async Task<IActionResult> EditByType(int templateId)
        {
            var temp = await _context.SittingTemplates
                .Include(t => t.SittingType)
                .FirstOrDefaultAsync(t => t.Id == templateId);
            var m = _mapper.Map<Models.Sitting.EditByType>(temp);
            return View(m);
        }

        [HttpPost]
        public async Task<IActionResult> EditByType(Models.Sitting.EditByType m)
        {
            var sittings = await _context.Sittings
                            .Include(s => s.SittingType)
                            .Include(s => s.Reservations.Where(r => r.StatusId != 3))
                            .Where(s => s.SittingTemplateId == m.Id && s.Active && s.StartTime.Date >= DateTime.Today)
                            .ToListAsync();
            if (sittings.Count == 0) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var editedSittingTemplate = _mapper.Map<SittingTemplate>(m);
                    foreach (var s in sittings)
                    {
                        if (s.StartTime.Date > editedSittingTemplate.EndDate.Date || s.StartTime.Date < editedSittingTemplate.StartDate.Date)
                        {
                            throw new Exception("The new dates would exclude existing bookings.");
                        }
                        if (s.Reservations.Sum(r => r.Guests) > editedSittingTemplate.Capacity)
                        {
                            throw new Exception("The new capacity would not accommodate the existing reservations.");
                        }
                        s.Reservations.ForEach(r =>
                        {
                            if (r.StartTime < s.StartTime.Date.Add(editedSittingTemplate.StartTime.TimeOfDay) || r.StartTime.AddMinutes(r.Duration) > s.EndTime.Date.Add(editedSittingTemplate.EndTime.TimeOfDay))
                            {
                                throw new Exception("The new sitting times would conflict with existing bookings.");
                            }
                        });//This is nastaran
                        // Fixed bug. Now only copies over new time, not datetime
                        s.StartTime = s.StartTime.Date.Add(editedSittingTemplate.StartTime.TimeOfDay);
                        s.EndTime = s.EndTime.Date.Add(editedSittingTemplate.EndTime.TimeOfDay);
                        s.Capacity = editedSittingTemplate.Capacity;
                        s.Increment = editedSittingTemplate.Increment;
                        s.DefaultReservationDuration = editedSittingTemplate.DefaultReservationDuration;
                        _context.Sittings.Update(s);
                    }
                    _context.SittingTemplates.Update(editedSittingTemplate);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("Error", ex.Message);
                }
            }
            m.SittingType = await _context.SittingTypes.FindAsync(m.SittingTypeId);
            return View(m);
        }

        public async Task<IActionResult> Rename(int sittingTypeId)
        {
            var sittingType = await _context.SittingTypes.FindAsync(sittingTypeId);
            return View(sittingType);
        }

        [HttpPost]
        public async Task<IActionResult> Rename(SittingType m)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.SittingTypes.Update(m);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index", "Sitting", new { area = "Admin" });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("Error", ex.Message);
                }
            }
            return View(m);
        }

        public async Task<IActionResult> ConfirmCancel(int sittingId)
        {
            var sitting = await _context.Sittings
                .Include(s => s.SittingType)
                .Include(s => s.Reservations.Where(r => r.StatusId != 3))
                .FirstOrDefaultAsync(s => s.Id == sittingId);
            return View(sitting);
        }

        [HttpPost]
        public async Task<IActionResult> Cancel(int id)
        {
            try
            {
                var sitting = await _context.Sittings
                    .Include(s => s.Reservations.Where(r => r.StatusId != 3))
                    .FirstOrDefaultAsync(s => s.Id == id);
                if (sitting.Reservations.Any()) throw new Exception("This sitting has active reservations. It cannot be cancelled.");
                sitting.Active = false;
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        public async Task<IActionResult> ConfirmCancelByType(int templateId)
        {
            var template = await _context.SittingTemplates
                    .Include(st => st.Sittings.Where(s => s.StartTime > DateTime.Now))
                        .ThenInclude(s => s.Reservations.Where(r => r.StatusId != 3))
                    .FirstOrDefaultAsync(st => st.Id == templateId);
            return View(template);
        }

        // **************** Need to add "Cancelled" attribute on entities to implement this ******************************

        [HttpPost]
        public async Task<IActionResult> CancelByType(int id)
        {
            try
            {
                var template = await _context.SittingTemplates
                    .Include(st => st.Sittings.Where(s => s.Active && s.StartTime > DateTime.Now))
                        .ThenInclude(s => s.Reservations.Where(r => r.StatusId != 3))
                    .FirstOrDefaultAsync(st => st.Id == id);
                foreach (var s in template.Sittings)
                {
                    if (s.Reservations.Any()) throw new Exception("At least one sitting in this schedule has active reservations. It cannot be cancelled.");
                }
                template.Sittings.ForEach(s => s.Active = false);
                template.Active = false;
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        public async Task<IActionResult> CloseForBookings(int id, string returnUrl)
        {
            try
            {
                var sitting = await _context.Sittings.FindAsync(id);
                if (sitting == null) return NotFound();
                sitting.IsClosedForBooking = true;
                await _context.SaveChangesAsync();
                return LocalRedirect(returnUrl);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        public async Task<IActionResult> OpenForBookings(int id, string returnUrl)
        {
            try
            {
                var sitting = await _context.Sittings.FindAsync(id);
                if (sitting == null) return NotFound();
                sitting.IsClosedForBooking = false;
                await _context.SaveChangesAsync();
                return LocalRedirect(returnUrl);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
    }
}
