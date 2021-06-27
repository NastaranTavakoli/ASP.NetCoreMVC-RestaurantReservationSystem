using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ReservationSystemDraft_v1.Data;

namespace ReservationSystemDraft_v1.Areas.Admin.Controllers
{
    [Authorize(Roles = "Manager")]
    public class AreaController : AdminBaseController
    {
        public AreaController(ApplicationDbContext context, UserManager<IdentityUser> userManager, IMapper mapper) : base(context, userManager, mapper) { }

        public async Task<Person> IdentifyUser()
        {
            var iu = await _userManager.FindByNameAsync(User.Identity.Name);
            var admin = _context.People.Include(p => p.Restaurant).FirstOrDefault(p => p.UserId == iu.Id);
            return admin;
        }
        public async Task<IActionResult> Index()
        {
            var admin = await IdentifyUser();
            var areas = await _context.Areas.Include(a => a.Tables).Where(a => a.RestaurantId == admin.RestaurantId).ToListAsync();
            if (areas == null) return NotFound();
            var m = new Models.Area.Index();
            m.Areas = areas;
            m.AreaOptions = new SelectList(areas, nameof(Area.Id), nameof(Area.Name));
            return View(m);
        }

        [HttpPost]
        public async Task<IActionResult> Index(Models.Area.Index m)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var area = await _context.Areas.Include(a => a.Tables).FirstOrDefaultAsync(a => a.Id == m.AreaId);
                    var t = _mapper.Map<Table>(m);
                    area.Tables.Add(t);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("Exception", ex.Message);
                }
            }
            var admin = await IdentifyUser();
            m.Areas = await _context.Areas.Include(a => a.Tables).Where(a => a.RestaurantId == admin.RestaurantId).ToListAsync();
            m.AreaOptions = new SelectList(m.Areas, nameof(Area.Id), nameof(Area.Name));
            return View(m);
        }

        public async Task<IActionResult> Create()
        {
            var admin = await IdentifyUser();
            var restaurant = await _context.Restaurants.FirstOrDefaultAsync(r => r.Id == admin.RestaurantId);
            if (restaurant == null) return NotFound();
            var m = new Models.Area.Create { RestaurantId = restaurant.Id };
            return View(m);
        }
        [HttpPost]
        public async Task<IActionResult> Create(Models.Area.Create m)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var a = _mapper.Map<Area>(m);
                    _context.Areas.Add(a);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index), a.Id);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("Exception", ex.Message);
                }
            }
            return View(m);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            var a = await _context.Areas.Include(a => a.Restaurant).FirstOrDefaultAsync(a => a.Id == id);
            if (a == null) return NotFound();
            var m = _mapper.Map<Models.Area.Edit>(a);
            return View(m);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(Models.Area.Edit m)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (!_context.Areas.Any(a => a.Id == m.Id)) return NotFound();
                    _context.Update(_mapper.Map<Area>(m));
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("Exception", ex.Message);
                }
            }
            return View(m);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            var a = await _context.Areas.Include(a => a.Restaurant).Include(a => a.Tables).FirstOrDefaultAsync(a => a.Id == id);
            if (a == null) return NotFound();
            var m = _mapper.Map<Models.Area.Details>(a);
            return View(m);
        }
        [HttpPost]
        public async Task<IActionResult> Delete(Models.Area.Details m)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var a = await _context.Areas.Include(a => a.Restaurant).Include(a => a.Tables).FirstOrDefaultAsync(a => a.Id == m.Id);
                    if (a == null) return NotFound();
                    _context.Areas.Remove(a);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("Exception", ex.Message);
                }
            }
            return View(m);
        }


        /*********************************************************** CRUD operations for Table **********************************************************/
        public async Task<IActionResult> EditTable(int? id)
        {
            var t = await _context.Tables
                .Include(t => t.Area)
                .FirstOrDefaultAsync(t => t.Id == id);
            if (t == null) return NotFound();
            var m = _mapper.Map<Models.Table.Edit>(t);
            m.AreaName = t.Area.Name;

            var admin = await IdentifyUser();
            m.Areas = await _context.Areas.Include(a => a.Tables).Where(a => a.RestaurantId == admin.RestaurantId).ToListAsync();
            m.AreaOptions = new SelectList(m.Areas, nameof(Area.Id), nameof(Area.Name), selectedValue: t.Area.Id);
            return View(m);
        }

        [HttpPost]
        public async Task<IActionResult> EditTable(Models.Table.Edit m)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var t = await _context.Tables
                        .Include(t => t.Area)
                        .FirstOrDefaultAsync(t => t.Id == m.Id);
                    if (t == null) return NotFound();
                    t.Name = m.Name;
                    t.Capacity = m.Capacity;
                    t.AreaId = m.AreaId;
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("Exception", ex.Message);
                }
            }


            var admin = await IdentifyUser();
            m.Areas = await _context.Areas.Include(a => a.Tables).Where(a => a.RestaurantId == admin.RestaurantId).ToListAsync();
            m.AreaOptions = new SelectList(m.Areas, nameof(Area.Id), nameof(Area.Name), selectedValue: m.AreaId);
            return View(m);
        }

        public async Task<IActionResult> DeleteTable(int? id)
        {
            var t = await _context.Tables.FirstOrDefaultAsync(t => t.Id == id);
            if (t == null) return NotFound();
            _context.Tables.Remove(t);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
