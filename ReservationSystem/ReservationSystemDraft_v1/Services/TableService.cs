using Microsoft.EntityFrameworkCore;
using ReservationSystemDraft_v1.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ReservationSystemDraft_v1.Services
{
    public class TableService
    {
        private readonly ApplicationDbContext _context;

        public TableService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ReservationTable> AssignTableToReservation(int reservationId, int tableId)
        {
            var tableAssignment = new ReservationTable { ReservationId = reservationId, TableId = tableId };
            if (_context.ReservationTables.Any(rt => rt.ReservationId == reservationId && rt.TableId == tableId))
            {
                throw new InvalidOperationException("This table has already been assigned to this reservation.");
            }
            _context.ReservationTables.Add(tableAssignment);
            await _context.SaveChangesAsync();
            return await _context.ReservationTables
                .Include(rt => rt.Table)
                .FirstOrDefaultAsync(rt => rt.Id == tableAssignment.Id);
        }

        public async Task<bool> RemoveTableFromReservation(int reservationTableId)
        {
            try
            {
                var tableAssignment = await _context.ReservationTables.FindAsync(reservationTableId);
                _context.ReservationTables.Remove(tableAssignment);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> RemoveAllTablesFromReservation(int reservationId)
        {
            try
            {
                var tables = _context.ReservationTables.Where(rt => rt.ReservationId == reservationId);
                _context.ReservationTables.RemoveRange(tables);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
