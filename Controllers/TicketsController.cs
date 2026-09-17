using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrainTickets.Data;
using TrainTickets.Models;
using TrainTickets.Models.ViewModels;

namespace TrainTickets.Controllers
{
    public class TicketsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public TicketsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Tickets
        public IActionResult Index(int? startStationId, int? endStationId, DateTime? date)
        {
            var tripsQuery = _context.Trips
                .Where(t => t.DepartureTime > DateTime.Now)
                .AsQueryable();

            if (startStationId.HasValue)
            {
                tripsQuery = tripsQuery.Where(t => t.StartStationId == startStationId);
            }

            if (endStationId.HasValue)
            {
                tripsQuery = tripsQuery.Where(t => t.EndStationId == endStationId);
            }

            if (date.HasValue)
            {
                tripsQuery = tripsQuery.Where(t =>
                    t.DepartureTime.Date == date.Value.Date);
            }

            var trips = tripsQuery
                .Select(t => new TicketIndexViewModel
                {
                    Id = t.Id,
                    TrainName = t.Train.Name,
                    StartStation = t.StartStation.Name,
                    EndStation = t.EndStation.Name,
                    DepartureTime = t.DepartureTime,
                    ArrivalTime = t.ArrivalTime,
                    Capacity = t.Train.Capacity,
                    SoldSeats = t.Tickets.Sum(x => x.Quantity),
                    TrainType = t.Train.Type
                })
                .OrderByDescending(t => t.DepartureTime)
                .ToList();

            var model = new TicketSearchViewModel
            {
                StartStationId = startStationId,
                EndStationId = endStationId,
                Date = date,
                Stations = _context.Stations
                    .Select(s => new SelectListItem
                    {
                        Value = s.Id.ToString(),
                        Text = s.Name
                    })
                    .ToList(),
                Trips = trips
            };

            return View(model);
        }



        // GET: Tickets/Details
        public async Task<IActionResult> Details(int id)
        {
            var userId = _userManager.GetUserId(User);

            var ticket = await _context.Tickets
                .Include(t => t.Trip)
                    .ThenInclude(tr => tr.Train)
                .Include(t => t.Trip)
                    .ThenInclude(tr => tr.StartStation)
                .Include(t => t.Trip)
                    .ThenInclude(tr => tr.EndStation)
                .Include(t => t.User)
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

            if (ticket == null)
                return NotFound();

            return View(ticket);
        }

        public IActionResult Buy(int tripId)
        {
            var trip = _context.Trips
                .Include(t => t.Train)
                .Include(t => t.StartStation)
                .Include(t => t.EndStation)
                .FirstOrDefault(t => t.Id == tripId);

            if (trip == null)
                return NotFound();

            var ticket = new Ticket
            {
                TripId = trip.Id,
                Quantity = 1,
                TotalPrice = trip.Price * 1, // Default quantity is 1
                Trip = trip
            };

            return View(ticket);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Buy([Bind("TripId,Quantity")] Ticket ticket)
        {
            var trip = await _context.Trips
                .Include(t => t.Train)
                .Include(t => t.StartStation)
                .Include(t => t.EndStation)
                .FirstOrDefaultAsync(t => t.Id == ticket.TripId);

            if (trip == null)
                return NotFound();

            // Не мога да се прави резервация за курс, който вече е отпътувал
            if (trip.DepartureTime <= DateTime.Now)
            {
                ModelState.AddModelError("",
                    "Не може да се прави резервация за превоз, който вече е отпътувал.");
            }

            var user = await _userManager.GetUserAsync(User);

            // Валидация на профила
            if (string.IsNullOrWhiteSpace(user.FirstName) ||
                string.IsNullOrWhiteSpace(user.MiddleName) ||
                string.IsNullOrWhiteSpace(user.LastName) ||
                string.IsNullOrWhiteSpace(user.PhoneNumber))
            {
                ModelState.AddModelError("",
                    "Моля, попълнете личната си информация " +
                    "(три имена и телефонен номер) в настройките " +
                    "на профила, преди да закупите билет.");
            }

            // Проверка за налични места
            int soldSeats = await _context.Tickets
                .Where(t => t.TripId == ticket.TripId)
                .SumAsync(t => t.Quantity);

            if (soldSeats + ticket.Quantity > trip.Train.Capacity)
            {
                ModelState.AddModelError("", "Няма достатъчно налични места.");
            }

            if (ticket.Quantity < 1 || ticket.Quantity > 10)
            {
                ModelState.AddModelError("", "Можете да закупите между 1 и 10 билета.");
            }

            // Колко билета вече е закупил този потребител за курса
            int userTicketsForTrip = await _context.Tickets
                .Where(t => t.TripId == ticket.TripId && t.UserId == user.Id)
                .SumAsync(t => t.Quantity);

            // Проверка дали ще надвиши лимита от 10 общо
            if (userTicketsForTrip + ticket.Quantity > 10)
            {
                ModelState.AddModelError("",
                    "Можете да закупите максимум 10 билета общо за този курс.");
            }

            // Ако има грешка при проверките изгледа се презарежда
            if (!ModelState.IsValid)
            {
                // Повторно зареждане на данните за маршрута, за да се покажат в изгледа
                ticket.Trip = trip;
                ticket.TotalPrice = trip.Price * ticket.Quantity;

                return View(ticket);
            }

            // Запазване на билета
            ticket.UserId = user.Id;
            ticket.PurchaseDate = DateTime.Now;

            _context.Tickets.Add(ticket);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(MyTickets));
        }

        public async Task<IActionResult> MyTickets()
        {
            var user = await _userManager.GetUserAsync(User);

            var tickets = await _context.Tickets
                .Where(t => t.UserId == user.Id)
                .Include(t => t.Trip.Train)
                .Include(t => t.Trip.StartStation)
                .Include(t => t.Trip.EndStation)
                .OrderByDescending(t => t.Trip.DepartureTime)
                .ToListAsync();

            return View(tickets);
        }
    }
}
