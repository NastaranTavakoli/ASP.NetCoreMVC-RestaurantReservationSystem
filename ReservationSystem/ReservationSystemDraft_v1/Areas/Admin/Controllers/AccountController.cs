using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using ReservationSystemDraft_v1.Areas.Admin.Models.Account;
using ReservationSystemDraft_v1.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace ReservationSystemDraft_v1.Areas.Admin.Controllers
{
    [Authorize(Roles = "Manager")]
    public class AccountController : AdminBaseController
    {
        public AccountController(ApplicationDbContext context, UserManager<IdentityUser> userManager, IMapper mapper) : base(context, userManager, mapper)
        {
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var userProfile = await _context.People
                .FirstOrDefaultAsync(p => p.UserId == user.Id);

            // Get all the User identities who are in the right role
            var allManagers = await _userManager.GetUsersInRoleAsync("Manager");
            var allStaff = await _userManager.GetUsersInRoleAsync("Staff");

            var managerIds = allManagers.Select(m => m.Id).ToList();
            
            // Get everyone from People table who is either a Manager or Staff at that restaurant
            var employees = await _context.People
                .Where(p => (allManagers.Select(m => m.Id).Contains(p.UserId) || allStaff.Select(s => s.Id).Contains(p.UserId)) && p.RestaurantId == userProfile.RestaurantId)
                .ToListAsync();

            var managers = employees.Where(e => allManagers.Select(m => m.Id).Contains(e.UserId)).ToList();
            var staff = employees.Where(e => !allManagers.Select(m => m.Id).Contains(e.UserId)).ToList();
           
            return View(new Models.Account.Index { Staff = staff, Managers = managers });
        }

        public IActionResult Create()
        {
            var newAdmin = new Create();
            return View(newAdmin);
        }


        [HttpPost]
        public async Task<IActionResult> Create(Create m)
        {
            if (ModelState.IsValid)
            {
                var user = new IdentityUser { UserName = m.Email, Email = m.Email, EmailConfirmed = true };
                var result = await _userManager.CreateAsync(user, m.Password);
                IdentityUser iu = await _userManager.FindByNameAsync(User.Identity.Name);
                var admin = _context.People.SingleOrDefault(p => iu.Id == p.UserId);
                var person = new Person()
                {
                    FirstName = m.FirstName,
                    LastName = m.LastName,
                    Email = m.Email,
                    Phone = m.Phone,
                    RestaurantId = admin.RestaurantId,
                    UserId = user.Id
                };
                if (result.Succeeded)
                {
                    try
                    {
                        await _userManager.AddToRoleAsync(user, "Staff");
                        await _context.People.AddAsync(person);
                        await _context.SaveChangesAsync();
                        return RedirectToAction("Index", "Home");
                    }
                     catch (Exception)
                    {
                        await _userManager.DeleteAsync(user);
                        ModelState.AddModelError("Exception", "Something went wrong!!");
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);

                }
            }

            return View(m);
        }

        [HttpPost]
        public async Task<IActionResult> GrantManagerAccess(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                await _userManager.AddToRoleAsync(user, "Manager");
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public async Task<IActionResult> RevokeManagerAccess(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user.UserName == User.Identity.Name) throw new Exception("You cannot revoke your own manager access.");
                await _userManager.RemoveFromRoleAsync(user, "Manager");
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
