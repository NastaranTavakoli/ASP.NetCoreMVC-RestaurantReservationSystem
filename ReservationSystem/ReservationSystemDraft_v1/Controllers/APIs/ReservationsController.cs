using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ReservationSystemDraft_v1.Data;
using ReservationSystemDraft_v1.Models.Reservation;

namespace ReservationSystemDraft_v1.Controllers.APIs
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public ReservationsController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<IActionResult> Create(Details m)
        {
            var sitting = await _context.Sittings.Include(s => s.Restaurant).FirstOrDefaultAsync(s => s.Id == m.SittingId);
            var existingPerson = _context.People.FirstOrDefault(p => p.Email == m.Email);

            Person temp = null;
            try
            {
                if (existingPerson == null)
                {
                    var newPerson = _mapper.Map<Person>(m);
                    await _context.People.AddAsync(newPerson);
                    await _context.SaveChangesAsync();
                    temp = newPerson;
                }
                else
                {
                    _mapper.Map(m, existingPerson);
                    await _context.SaveChangesAsync();
                    temp = existingPerson;
                }

                var r = _mapper.Map<Reservation>(m);
                r.Duration = sitting.DefaultReservationDuration;
                r.SourceId = 1;
                r.StatusId = 1;
                r.StartTime = DateTime.Parse(m.SelectedDate).Add(TimeSpan.Parse(m.selectedTime));
                r.Sitting = sitting;
                r.CustomerId = temp.Id;
                r.Customer = temp;

                var rnd = new Random();
                r.ConfirmationCode =
                    $"{r.Customer.Id}{m.SittingId}{r.Customer.Reservations.Count}{rnd.Next(1000000, 9999999)}";


                await _context.Reservations.AddAsync(r);
                await _context.SaveChangesAsync();
                var result = new
                {
                    sitting.Restaurant.Name,
                    r.Guests,
                    r.StartTime,
                    r.ConfirmationCode,
                    temp.FullName
                };
                return Ok(result);
            }
            catch (Exception ex)
            {
                //if creating a Reservation fails, delete the new person from Person table.
                if (existingPerson == null && temp != null)
                {
                    _context.People.Remove(temp);
                    await _context.SaveChangesAsync();
                }
                ModelState.AddModelError("Exception", ex.InnerException.Message);
                return BadRequest(ModelState);
            }
        }
    }
}

