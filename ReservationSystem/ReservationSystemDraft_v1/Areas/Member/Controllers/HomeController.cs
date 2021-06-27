using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReservationSystemDraft_v1.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using ReservationSystemDraft_v1.Areas.Member.Models.Activity;

namespace ReservationSystemDraft_v1.Areas.Member.Controllers
{
    public class HomeController : MemberBaseController
    {
        public HomeController(ApplicationDbContext context, UserManager<IdentityUser> userManager, IMapper mapper) : base(context, userManager, mapper) { }

        public async Task<IActionResult> Index()
        {
            var member = await _userManager.FindByNameAsync(User.Identity.Name);
            var memberProfile = await _context.People.Include(p => p.Reservations).FirstOrDefaultAsync(p => p.UserId == member.Id);

            var reservations = await _context.Reservations
                .Where(r => r.CustomerId == memberProfile.Id)
                .Include(r => r.Status)
                .Include(r => r.Sitting)
                    .ThenInclude(s => s.Restaurant)                
                .ToListAsync();
            return View(reservations);
        }

        public async Task<IActionResult> Details(int id)
        {
            var reservation = await _context.Reservations.Include(r => r.Customer).Include(r => r.Status).Include(r => r.Sitting).ThenInclude(r => r.Restaurant).SingleOrDefaultAsync(r => r.Id == id);
            var details = new Request()
            {
                Id = reservation.Id,
                StartTime = reservation.StartTime,
                Duration = reservation.Duration,
                Guests = reservation.Guests,
                Email = reservation.Customer.Email,
                Phone = reservation.Customer.Phone,
                Notes = reservation.Notes,
                RestaurantName = reservation.Sitting.Restaurant.Name,
            };
            return View(details);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateRequest(Request request)
        {
            var reservation = await _context.Reservations.Include(r => r.Customer).SingleOrDefaultAsync(r => r.Id == request.Id);
            var reservationMappedToRequest = new Request()
            {
                Id = reservation.Id,
                StartTime = reservation.StartTime,
                Duration = reservation.Duration,
                Guests = reservation.Guests,
                Email = reservation.Customer.Email,
                Phone = reservation.Customer.Phone,
                Notes = reservation.Notes ??= "",
            };

            foreach (PropertyInfo pi in request.GetType().GetProperties())
            {
                if (pi.GetValue(request) != null)
                {
                    if (pi.GetValue(request).ToString() != pi.GetValue(reservationMappedToRequest).ToString())
                    {
                        if (!String.IsNullOrEmpty(reservation.Notes))
                        {
                            reservation.Notes += $"\nUpdate {pi.Name}: {pi.GetValue(request)}";
                        }
                        else
                        {
                            reservation.Notes += $"Update {pi.Name}: {pi.GetValue(request)}";
                        }
                        reservation.ActionRequired = true;
                        await _context.SaveChangesAsync();
                    }
                }

            }
            return (RedirectToAction(nameof(Index)));
        }

        public async Task<IActionResult> CancelRequest(int reservationId)
        {
            var reservation = await _context.Reservations.FindAsync(reservationId);
            reservation.Notes += "\nCustomer Asked for cancellation";
            reservation.ActionRequired = true;
            await _context.SaveChangesAsync();
            return (RedirectToAction(nameof(Index)));
        }



    }
}
