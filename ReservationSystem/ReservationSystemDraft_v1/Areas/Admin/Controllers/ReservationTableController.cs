using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ReservationSystemDraft_v1.Data;
using ReservationSystemDraft_v1.Services;
using ReservationSystemDraft_v1.Areas.Admin.Models.ReservationTable;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ReservationSystemDraft_v1.Areas.Admin.Controllers
{
    public class ReservationTableController : AdminBaseController
    {
        private readonly TableService _tableService;

        public ReservationTableController(ApplicationDbContext context, UserManager<IdentityUser> userManager, IMapper mapper, TableService tableService) : base(context, userManager, mapper)
        {
            _tableService = tableService;
        }

        public async Task<IActionResult> Index(DateTime? date = null, int sittingId = 0, int reservationId = 0, string returnUrl = null)
        {
            var selectedDate = date ?? DateTime.Today;
            var iu = await _userManager.FindByNameAsync(User.Identity.Name);
            var admin = _context.People.SingleOrDefault(p => iu.Id == p.UserId);

            var sittings = await _context.Sittings
                .Include(s => s.SittingType)
                .Where(s => s.RestaurantId == admin.RestaurantId
                    && s.StartTime.Date == selectedDate.Date)
                .ToListAsync();

            if (sittings.Any())
            {
                sittingId = sittingId != 0 ? sittingId : sittings[0].Id;
            }

            var tables = await _context.Tables
                .Include(t => t.Area)
                .Where(t => t.Area.RestaurantId == admin.RestaurantId)
                .ToListAsync();

            var reservationsToAssignTables = await _context.Reservations
                .Include(r => r.Customer)
                .Include(r => r.ReservationTables).
                    ThenInclude(rt => rt.Table)
                .Where(r => r.Sitting.RestaurantId == admin.RestaurantId
                    && r.SittingId == sittingId
                    && r.StatusId == 2 
                    && r.StartTime.Date == selectedDate.Date)
                .ToListAsync();

            var clashingTables = new List<ReservationTable>();
            foreach (var r1 in reservationsToAssignTables)
            {
                foreach(var r2 in reservationsToAssignTables.Where(r => r.Id != r1.Id))
                {
                    if (r1.StartTime < r2.EndTime && r1.EndTime > r2.StartTime)
                    {
                        clashingTables.AddRange(r1.ReservationTables.Where(rt => r2.ReservationTables.Select(t => t.TableId).Contains(rt.TableId)));
                    }
                }
            }

            var m = new ReservationTableIndex
            {
                Date = selectedDate,
                Sittings = sittings,
                SittingId = sittingId,
                Reservations = reservationsToAssignTables,
                ReservationId = reservationId,
                Tables = new SelectList(tables, nameof(Table.Id), nameof(Table.NameAndCapacity)),
                ClashingTables = clashingTables
            };
            ViewBag.ReturnUrl = returnUrl ?? "/Admin/Reservation";
            return View(m);
        }



        public async Task<IActionResult> SelectTables(int reservationId, string returnUrl = "/Admin/ReservationTable")
        {
            ViewBag.ReturnUrl = returnUrl;

            var sitting = _context.Reservations
                .Include(r => r.Sitting)
                .SingleOrDefault(r => r.Id == reservationId)
                .Sitting;            
            var reservationTablesOfSitting = _context.ReservationTables
                .Include(rt => rt.Reservation)
                .Where(rt => rt.Reservation.SittingId == sitting.Id);
            var reservation = _context.Reservations
                .Include(r => r.Customer)
                .Include(r => r.ReservationTables)
                .ThenInclude(rt => rt.Table)
                .SingleOrDefault(r => r.Id == reservationId);
            var occupiedTablesIds = new List<int>();

            foreach (var rt in reservationTablesOfSitting)
            {
                if (rt.Reservation.StartTime <= reservation.StartTime && rt.Reservation.EndTime > reservation.StartTime || rt.Reservation.StartTime < reservation.EndTime && rt.Reservation.EndTime >= reservation.StartTime || rt.Reservation.StartTime >= reservation.StartTime && rt.Reservation.EndTime <= reservation.EndTime)
                {
                    occupiedTablesIds.Add(rt.TableId);
                }
            }

            var tables = _context.Tables.Where(t => t.Area.RestaurantId == sitting.RestaurantId);
            var freeTables = new List<Table>();

            foreach (var table in tables)
            {
                if (!occupiedTablesIds.Contains(table.Id))
                {
                    freeTables.Add(table);
                }
            }
            ViewBag.Customer = reservation.Customer.FullName;
            ViewBag.ReservationTime = reservation.StartTime;
            ViewBag.Areas = await _context.Areas.Where(a => a.RestaurantId == sitting.RestaurantId).ToListAsync();
            ViewBag.ReservedTables = reservation.ReservationTables.Select(rt => rt.Table.Name).ToList();
            ViewBag.ReservationId = reservationId;
            ViewBag.NumOfGuests = reservation.Guests;
            var seatedGuests = 0;
            reservation.ReservationTables.ForEach(rt => seatedGuests += rt.Table.Capacity);
            ViewBag.NumOfSeatedGuests = seatedGuests;
            return View(freeTables);
        }


        public async Task<IActionResult> AssignTable(int tableId, int reservationId, string returnUrl = "Admin/ReservationTable")
        {
            var reservationTable = await _tableService.AssignTableToReservation(reservationId, tableId);
            var table = await _context.Tables.FindAsync(tableId);
            var reservation = _context.Reservations.Include(r => r.ReservationTables).ThenInclude(rt => rt.Table).SingleOrDefault(r => r.Id == reservationId);
            reservation.ReservationTables.Add(reservationTable);
            var seatedGuests = 0;
            reservation.ReservationTables.ForEach(rt => seatedGuests += rt.Table.Capacity);
            if (reservation.Guests > seatedGuests)
            {             
                return RedirectToAction(nameof(SelectTables), new { reservationId = reservationId, returnUrl = returnUrl });
            }         
            return LocalRedirect(returnUrl);
        }


        public async Task<IActionResult> RemoveTables(int reservationId)
        {
            await _tableService.RemoveAllTablesFromReservation(reservationId);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> RemoveTable(int id)
        {
            await _tableService.RemoveTableFromReservation(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
