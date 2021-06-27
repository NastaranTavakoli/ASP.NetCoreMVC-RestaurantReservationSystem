using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReservationSystemDraft_v1.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using ReservationSystemDraft_v1.Areas.Member.Models.Activity;
using static System.String;

namespace ReservationSystemDraft_v1.Areas.Member.Controllers.APIs
{
    [Route("api/member/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ReservationsController : ControllerBase
    {

        private readonly ApplicationDbContext _context;

        public ReservationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var user = User.Identity;
            var userProfile = await _context.People.FirstOrDefaultAsync(p => p.Email == user.Name);

            var reservations = await _context.Reservations
                .Where(r => r.CustomerId == userProfile.Id)
                .Include(r => r.Status)
                .Include(r => r.Sitting)
                .ThenInclude(s => s.Restaurant).Select(r => new
                {
                    r.Id,
                    r.StartTime,
                    r.Duration,
                    r.ConfirmationCode,
                    r.Guests,
                    r.ActionRequired,
                    Restaurant = r.Sitting.Restaurant.Name,
                    Status = r.Status.Description
                }).ToListAsync();
            return Ok(reservations);

        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var user = User.Identity;
            var userProfile = await _context.People.FirstOrDefaultAsync(p => p.Email == user.Name);

            var reservation = await _context.Reservations.Include(r => r.Customer).Include(r => r.Status).Include(r => r.Sitting).ThenInclude(r => r.Restaurant)
                .SingleOrDefaultAsync(r => r.Id == id);
            if (reservation == null) return NotFound();
            if (userProfile.Id == reservation.CustomerId)
            {
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
                return Ok(details);
            }

            return Forbid();
        }


        [HttpPatch("{id}")]
        public async Task<IActionResult> AddRequest(int id, [FromBody] Action action)
        {
            var user = User.Identity;
            var userProfile = await _context.People.FirstOrDefaultAsync(p => p.Email == user.Name);
            var reservation = await _context.Reservations.FindAsync(id);

            if (reservation == null) return NotFound();
            if (userProfile.Id == reservation.CustomerId)
            {
                if (action.RequestType.ToLower() == "update")
                {
                    action.Request.Id = id;
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

                    foreach (PropertyInfo pi in action.Request.GetType().GetProperties())
                    {
                        if (pi.GetValue(action.Request) != null)
                        {
                            if (pi.GetValue(action.Request)?.ToString() !=
                                pi.GetValue(reservationMappedToRequest)?.ToString())
                            {
                                if (!IsNullOrEmpty(reservation.Notes))
                                {
                                    reservation.Notes += $"\nUpdate {pi.Name}:{pi.GetValue(action.Request)}";
                                }
                                else
                                {
                                    reservation.Notes += $"Update {pi.Name}:{pi.GetValue(action.Request)}";
                                }

                                reservation.ActionRequired = true;
                                await _context.SaveChangesAsync();
                            }
                        }
                    }
                    return Ok();
                }
                if (action.RequestType.ToLower() == "cancel")
                {
                    reservation.Notes += "\nCustomer Asked for cancellation";
                    reservation.ActionRequired = true;
                    await _context.SaveChangesAsync();
                    return Ok();
                }
            }
            return Forbid();
        }
    }

}



