using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ReservationSystemDraft_v1.Data;
using ReservationSystemDraft_v1.Helpers;
using ReservationSystemDraft_v1.Services;

namespace ReservationSystemDraft_v1.Controllers.APIs
{
    [Route("api/restaurants")]
    [ApiController]
    public class AvailabilitiesController : ControllerBase
    {
        private readonly ReservationService _reservationService;

        public AvailabilitiesController(ReservationService reservationService)
        {
            _reservationService = reservationService;
        }


        [HttpGet("{restaurantId}/availabilities")]
        public async Task<IActionResult> Get(int restaurantId, [FromQuery] AvailabilityQueryObject m)
        {

            var selectedDate = DateTime.Parse(m.SelectedDate);
            var availabilities = await _reservationService.GetActiveTimeSlots(90, m.Guests, selectedDate, restaurantId);
            var availabilitiesForClient = availabilities.Where(a => a.IsAvailable)
                .Select(a => new {SittingId = a.Sitting.Id, a.StartTime, a.Sitting.ShortDescription });

            var groupsBySittingType = availabilitiesForClient.GroupBy(a => a.SittingId).Select(grp => new {Sitting= grp.Key, Availabilities=grp,  grp.ToArray()[0].ShortDescription });
            
            return Ok(groupsBySittingType);
        }
    }
}
