using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ReservationSystemDraft_v1.Data;
using ReservationSystemDraft_v1.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace ReservationSystemDraft_v1.Areas.Admin.Controllers
{
    [Authorize(Roles = "Manager")]
    public class ReportController : AdminBaseController
    {
        public ReportController(ApplicationDbContext context, UserManager<IdentityUser> userManager, IMapper mapper, ReservationService reservationService) : base(context, userManager, mapper) { }

        public async Task<Person> IdentifyUser()
        {
            var iu = await _userManager.FindByNameAsync(User.Identity.Name);
            var admin = _context.People.Include(p => p.Restaurant).FirstOrDefault(p => p.UserId == iu.Id);
            return admin;
        }

        public List<DateTime> WeeklyDates(DateTime weekStart, DateTime weekEnd)
        {
            var weeklyDates = new List<DateTime>();
            for (DateTime i = weekStart; i < weekEnd; i = i.AddDays(1))
            {
                weeklyDates.Add(i);
            }
            return weeklyDates;
        }

        public async Task<List<Sitting>> GetWeeklySittings(DateTime weekStart, DateTime weekEnd)
        {
            var admin = await IdentifyUser();
            var sittings = await _context.Sittings
                .Include(s => s.Restaurant)
                    .ThenInclude(r => r.Areas)
                .Include(s => s.Reservations)
                .OrderBy(s => s.StartTime)
                .Include(s => s.SittingType)
                .Where(s => s.StartTime.Date >= weekStart.Date && s.StartTime.Date <= weekEnd.Date && s.RestaurantId == admin.RestaurantId)
                .ToListAsync();
            return sittings;
        }

        public List<List<int>> GetWeeklyNumberOfSeatsByState(int sittingTypeId, List<Sitting> sittings, List<DateTime> weeklyDates)
        {
            var sittingsBySittingTypeId = sittings.Where(s => s.SittingTypeId == sittingTypeId).ToList();

            var dailySeatsList = new List<List<int>>();
            var totalSeatsList = new List<int>();
            var completedSeatsList = new List<int>();
            var canceledSeatsList = new List<int>();

            foreach (var date in weeklyDates)
            {
                if (!sittingsBySittingTypeId.Any(s => s.StartTime.Date == date.Date))
                {
                    totalSeatsList.Add(0);
                    completedSeatsList.Add(0);
                    canceledSeatsList.Add(0);
                }
                else
                {
                    var sitting = sittingsBySittingTypeId.Where(s => s.StartTime.Date == date.Date).FirstOrDefault();
                    var bookingTotal = sitting.Reservations.Sum(r => r.Guests);
                    totalSeatsList.Add(bookingTotal);
                    // 1 = "Pending", 2 = "Confirmed", 3 = "Cancelled", 4 = "Seated", 5 = "Completed"
                    var completedBookingsSum = sitting.Reservations.Where(r => r.StatusId == 5).Sum(r => r.Guests);
                    completedSeatsList.Add(completedBookingsSum);
                    var canceledBookingsSum = sitting.Reservations.Where(r => r.StatusId == 3).Sum(r => r.Guests);
                    canceledSeatsList.Add(canceledBookingsSum);
                }
            }
            dailySeatsList.Add(totalSeatsList);
            dailySeatsList.Add(completedSeatsList);
            dailySeatsList.Add(canceledSeatsList);

            return dailySeatsList;
        }

        public List<int> GetReservationsByState(List<Sitting> sittings)
        {
            //var sittings = await GetWeeklySittings();
            var totalReservations = sittings.SelectMany(s => s.Reservations).ToList();
            var reservationCountList = new List<int>
            {
                totalReservations.Count,
                // 1 = "Pending", 2 = "Confirmed", 3 = "Cancelled", 4 = "Seated", 5 = "Completed"
                totalReservations.Where(r => r.StatusId == 5).ToList().Count,
                totalReservations.Where(r => r.StatusId == 3).ToList().Count,
                totalReservations.Where(r => r.StatusId != 3 && r.StatusId != 5 && r.StartTime >= DateTime.Now).ToList().Count
            };
            return reservationCountList;
        }

        public List<int> GetReservationsBySource(List<Sitting> sittings)
        {
            var totalReservations = sittings.SelectMany(s => s.Reservations).ToList();
            var reservationCountList = new List<int> 
            {
                //1 = "Online", 2 = "Email", 3 = "Phone", 4 = "In Person"
                totalReservations.Where(r => r.SourceId == 1).ToList().Count,
                totalReservations.Where(r => r.SourceId == 2).ToList().Count,
                totalReservations.Where(r => r.SourceId == 3).ToList().Count,
                totalReservations.Where(r => r.SourceId == 4).ToList().Count
            };
            return reservationCountList;
        }

        public async Task<List<Sitting>> GetSittingsByDate(DateTime date)
        {
            var admin = await IdentifyUser();
            var sittings = _context.Sittings
                .Include(s => s.Reservations)
                .OrderBy(s => s.StartTime)
                .Include(s => s.SittingType)
                .Where(s => s.StartTime.Date == date.Date && s.RestaurantId == admin.RestaurantId)
                .ToList();
            return sittings;
        }

        
        /************************************* Weekly review page *******************************************/
        public async Task<IActionResult> Index(DateTime? datetime)
        {
            var admin = await IdentifyUser();
            var m = new Models.Report.Index();
            if (datetime == null)
            {
                var today = DateTime.Today;
                m.WeekStart = today.AddDays(-(int)today.DayOfWeek);
            } 
            else
            {
                m.WeekStart = datetime.GetValueOrDefault().Date.AddDays(-(int)datetime.GetValueOrDefault().DayOfWeek);
            }
            m.WeekEnd = m.WeekStart.Date.AddDays(7).AddSeconds(-1);
            m.Sittings = await GetWeeklySittings(m.WeekStart, m.WeekEnd);
            var weeklyDates = WeeklyDates(m.WeekStart, m.WeekEnd);
            if (m.Sittings.Count != 0)
            {
                //Getting only the sitting types available in the specified week.
                var sittingTypes = await _context.SittingTypes.Include(st => st.Sittings).Where(s => s.Sittings.Any(s => s.StartTime.Date >= m.WeekStart.Date && s.StartTime.Date <= m.WeekEnd.Date && s.RestaurantId == admin.RestaurantId)).ToListAsync();
                m.SittingTypes = new SelectList(sittingTypes, "Id", "Description");
                m.WeeklySeatsBooked = GetWeeklyNumberOfSeatsByState(sittingTypes[0].Id, m.Sittings, weeklyDates);
                m.ReservationCountListByState = GetReservationsByState(m.Sittings);
                m.ReservationCountListBySource = GetReservationsBySource(m.Sittings);
            }
            return View(m);
        }

        [HttpPost]
        public async Task<IActionResult> Index(Models.Report.Index m)
        {
            var admin = await IdentifyUser();
            m.Sittings = await GetWeeklySittings(m.WeekStart, m.WeekEnd);
            var weeklyDates = WeeklyDates(m.WeekStart, m.WeekEnd);
            if (m.Sittings.Count != 0)
            {
                var sittingTypes = await _context.SittingTypes.Include(st => st.Sittings).Where(s => s.Sittings.Any(s => s.StartTime.Date >= m.WeekStart.Date && s.StartTime.Date <= m.WeekEnd.Date && s.RestaurantId == admin.RestaurantId)).ToListAsync();
                m.SittingTypes = new SelectList(sittingTypes, "Id", "Description");
                m.WeeklySeatsBooked = GetWeeklyNumberOfSeatsByState(m.SittingTypeId, m.Sittings, weeklyDates);
                m.ReservationCountListByState = GetReservationsByState(m.Sittings);
                m.ReservationCountListBySource = GetReservationsBySource(m.Sittings);
            }
            return View(m);
        }


        /****************************************** Daily review page **********************************************/
        public async Task<IActionResult> DailyReview(DateTime? datetime)
        {
            var admin = await IdentifyUser();
            var m = new Models.Report.DailyReview();
            if (datetime == null)
            {
                m.SelectedDate = DateTime.Today;
            }
            else
            {
                m.SelectedDate = (DateTime)datetime;
            }
            m.Sittings = await GetSittingsByDate(m.SelectedDate);
            foreach (var s in m.Sittings)
            {
                for (DateTime i = s.StartTime; i < s.EndTime; i = i.AddMinutes(30))
                {
                    m.TimeSlots.Add(i);
                    m.CapacityList.Add(s.Capacity);
                }
            }

            foreach (var ts in m.TimeSlots)
            {
                var reservations = m.Sittings.SelectMany(s => s.Reservations).Where(r => r.StartTime <= ts && r.EndTime >= ts).ToList();
                // 1 = "Pending", 2 = "Confirmed", 3 = "Canceled", 4 = "Seated", 5 = "Completed"
                var completedReservations = m.Sittings.SelectMany(s => s.Reservations).Where(r => r.StartTime <= ts && r.EndTime >= ts && r.StatusId == 5).ToList();
                var canceledReservations = m.Sittings.SelectMany(s => s.Reservations).Where(r => r.StartTime <= ts && r.EndTime >= ts && r.StatusId == 3).ToList();
                if (reservations.Count == 0)
                {
                    m.TotalSeats.Add(0);
                }
                else
                {
                    m.TotalSeats.Add(reservations.Sum(r => r.Guests));
                }
                if (completedReservations.Count == 0)
                {
                    m.SeatsCompleted.Add(0);
                }
                else
                {
                    m.SeatsCompleted.Add(completedReservations.Sum(r => r.Guests));
                }
                if (canceledReservations.Count == 0)
                {
                    m.SeatsCanceled.Add(0);
                }
                else
                {
                    m.SeatsCanceled.Add(canceledReservations.Sum(r => r.Guests));
                }
            }
            m.ReservationCountList = GetReservationsByState(m.Sittings);
            return View(m);
        }
    }
}
