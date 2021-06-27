using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReservationSystemDraft_v1.Data;
using ReservationSystemDraft_v1.Services;

namespace ReservationSystemDraft_v1.Areas.Admin.Controllers.APIs
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationTablesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly TableService _tableService;

        public ReservationTablesController(ApplicationDbContext context, TableService tableService)
        {
            _context = context;
            _tableService = tableService;
        }

        [HttpGet("reservation/{reservationId:int}")]
        public async Task<ActionResult<IEnumerable<ReservationTable>>> GetTablesForReservation(int reservationId)
        {
            var reservationTables = await _context.ReservationTables
                .Where(rt => rt.ReservationId == reservationId)
                .ToListAsync();

            if (!reservationTables.Any())
            {
                return NotFound();
            }

            return reservationTables;
        }

        // POST: api/ReservationTables
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ReservationTable>> PostReservationTable([FromBody] ReservationTable rt)
        {
            try
            {
                var reservationTable = await _tableService.AssignTableToReservation(rt.ReservationId, rt.TableId);
                return Ok();
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        // DELETE: api/ReservationTables/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteReservationTable(int id)
        {
            await _tableService.RemoveTableFromReservation(id);

            return NoContent();
        }

        [HttpDelete("reservation/{reservationId:int}")]
        public async Task<IActionResult> DeleteAllTablesForReservation(int reservationId)
        {
            await _tableService.RemoveAllTablesFromReservation(reservationId);

            return NoContent();
        }

        private bool ReservationTableExists(int id)
        {
            return _context.ReservationTables.Any(e => e.Id == id);
        }
    }
}
