using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReservationSystemDraft_v1.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReservationSystemDraft_v1.Areas.Admin.Controllers
{
    public class HomeController : AdminBaseController
    {
        public HomeController(ApplicationDbContext context, UserManager<IdentityUser> userManager, IMapper mapper) : base(context, userManager, mapper)
        {
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var userProfile = await _context.People.FirstOrDefaultAsync(p => p.UserId == user.Id);
            var restaurant = await _context.Restaurants.FindAsync(userProfile.RestaurantId);
            if (restaurant == null) return NotFound();
            return View(restaurant);
        }
    }
}
