using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ReservationSystemDraft_v1.Data;

namespace ReservationSystemDraft_v1.Areas.Admin.Controllers.APIs
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public CustomersController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }



        public async Task<IActionResult> Get(string search, int restaurantId)
        {
            var emails = await _context.People
                .Include(p => p.Reservations)
                    .ThenInclude(r => r.Sitting)
                .Where(p => p.Reservations.Any(r => r.Sitting.RestaurantId == restaurantId)
                    && p.Email.Contains(search))
                .OrderBy(e => e)
                .Take(5).Select(p => new { p.Email, p.Id }).ToListAsync();

            return Ok(emails);
        }



        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var person = await _context.People.FindAsync(id);

            return Ok(person);
        }


    }
}
