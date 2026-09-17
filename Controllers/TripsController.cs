using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TrainTickets.Data;
using TrainTickets.Enums;
using TrainTickets.Models;
using TrainTickets.Models.ViewModels;

namespace TrainTickets.Controllers
{
    public class TripsController : BaseAdminController
    {
        private readonly ApplicationDbContext _context;

        public TripsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager) : base(userManager)
        {
            _context = context;
        }

        // GET: Trips
        public async Task<IActionResult> Index()
        {
            if (!await IsAdmin())
            {
                return Forbid();
            }
            var applicationDbContext = _context.Trips
                .Include(t => t.EndStation)
                .Include(t => t.StartStation)
                .Include(t => t.Train);

            return View(await applicationDbContext
                .OrderByDescending(t => t.DepartureTime).ToListAsync());
        }

        // GET: Trips/Details
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var trip = await _context.Trips
                .Include(t => t.Train)
                .Include(t => t.StartStation)
                .Include(t => t.EndStation)
                .Include(t => t.Tickets)
                .ThenInclude(ticket => ticket.User)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (trip == null)
                return NotFound();

            return View(trip);
        }


        // GET: Trips/Create
        public IActionResult Create()
        {
            DateTime now = DateTime.Now;

            var model = new TripViewModel
            {
                DepartureTime = now,
                ArrivalTime = now.AddHours(1),

                Trains = _context.Trains
            .Select(t => new SelectListItem
            {
                Value = t.Id.ToString(),
                Text = $"{t.Name} ({t.Type.GetDisplayName()})"
            })
            .ToList(),

                Stations = _context.Stations
            .Select(s => new SelectListItem
            {
                Value = s.Id.ToString(),
                Text = s.Name
            })
            .ToList()
            };

            return View(model);
        }

        // POST: Trips/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(TripViewModel model)
        {
            if (model.StartStationId == model.EndStationId)
            {
                ModelState.AddModelError("", "Началната и крайна гара не могат да бъдат еднакви.");
            }

            if (model.DepartureTime < DateTime.Now)
            {
                ModelState.AddModelError("", "Не може да се създаде превоз с изминала дата.");
            }

            if (model.DepartureTime >= model.ArrivalTime)
            {
                ModelState.AddModelError("", "Времето на отпътуване трябва да е по-рано от времето на пристигане.");
            }

            if (model.Price <= 0)
            {
                ModelState.AddModelError("", "Моля въведете цена.");
            }

            bool trainIsBusy = _context.Trips.Any(t =>
                t.TrainId == model.TrainId &&
                model.DepartureTime < t.ArrivalTime &&
                model.ArrivalTime > t.DepartureTime
            );

            if (trainIsBusy)
            {
                ModelState.AddModelError("",
                    "Избраният влак е зает за посочения период от време.");
            }

            if (!ModelState.IsValid)
            {
                LoadTripDropdowns(model);
                return View(model);
            }

            var trip = new Trip
            {
                TrainId = model.TrainId,
                StartStationId = model.StartStationId,
                EndStationId = model.EndStationId,
                DepartureTime = model.DepartureTime,
                ArrivalTime = model.ArrivalTime,
                Price = model.Price
            };

            _context.Trips.Add(trip);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
        // GET: Trips/Edit
        public IActionResult Edit(int id)
        {
            var trip = _context.Trips
                .Include(t => t.Train)
                .Include(t => t.StartStation)
                .Include(t => t.EndStation)
                .FirstOrDefault(t => t.Id == id);

            if (trip == null)
            {
                return NotFound();
            }

            var model = new TripViewModel
            {
                Id = trip.Id,
                TrainId = trip.Train.Id,
                StartStationId = trip.StartStation.Id,
                EndStationId = trip.EndStation.Id,
                DepartureTime = trip.DepartureTime,
                ArrivalTime = trip.ArrivalTime,
                Price = trip.Price,
                Trains = _context.Trains
                    .Select(t => new SelectListItem
                    {
                        Value = t.Id.ToString(),
                        Text = $"{t.Name} ({t.Type.GetDisplayName()})",
                        Selected = (t.Id == trip.TrainId)
                    })
                    .ToList(),

                Stations = _context.Stations
                    .Select(s => new SelectListItem
                    {
                        Value = s.Id.ToString(),
                        Text = s.Name
                    })
                    .ToList()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TripViewModel model)
        {
            if (model.StartStationId == model.EndStationId)
            {
                ModelState.AddModelError("", "Началната и крайна гара не могат да бъдат еднакви.");
                LoadTripDropdowns(model);
            }

            if (model.DepartureTime < DateTime.Now)
            {
                ModelState.AddModelError("", "Не може да се създаде превоз с изминала дата.");
                LoadTripDropdowns(model);
            }

            if (model.DepartureTime >= model.ArrivalTime)
            {
                ModelState.AddModelError("", "Времето на отпътуване трябва да е по-рано от времето на пристигане.");
                LoadTripDropdowns(model);
            }

            if (model.Price <= 0)
            {
                ModelState.AddModelError("", "Моля въведете цена.");
                LoadTripDropdowns(model);
            }

            bool trainIsBusy = _context.Trips.Any(t =>
                t.TrainId == model.TrainId &&
                t.Id != model.Id &&
                model.DepartureTime < t.ArrivalTime &&
                model.ArrivalTime > t.DepartureTime
            );


            if (trainIsBusy)
            {
                ModelState.AddModelError("",
                    "Избраният влак е зает за посочения период от време.");
                LoadTripDropdowns(model);
            }

            bool hasTickets = await _context.Tickets.AnyAsync(t => t.TripId == id);
            if (hasTickets)
            {
                ModelState.AddModelError("",
                    "Този превоз не може да бъде редактиран, защото има закупени билети.");

                LoadTripDropdowns(model);

                return View(model);
            }

            if (!ModelState.IsValid)
            {
                LoadTripDropdowns(model);
                return View(model);
            }

            var trip = _context.Trips.Find(model.Id);

            if (trip == null)
            {
                return NotFound();
            }

            trip.TrainId = model.TrainId;
            trip.StartStationId = model.StartStationId;
            trip.EndStationId = model.EndStationId;
            trip.DepartureTime = model.DepartureTime;
            trip.ArrivalTime = model.ArrivalTime;
            trip.Price = model.Price;

            _context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        // GET: Trips/Delete
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var trip = await _context.Trips
                .Include(t => t.EndStation)
                .Include(t => t.StartStation)
                .Include(t => t.Train)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (trip == null)
            {
                return NotFound();
            }

            return View(trip);
        }

        // POST: Trips/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tripToDelete = await _context.Trips.FindAsync(id);

            if (tripToDelete.ArrivalTime >= DateTime.Now)
            {
                bool hasTickets = await _context.Tickets
                .AnyAsync(t => t.TripId == id);

                if (hasTickets)
                {
                    var trip = await _context.Trips
                        .Include(t => t.Train)
                        .Include(t => t.StartStation)
                        .Include(t => t.EndStation)
                        .FirstOrDefaultAsync(t => t.Id == id);

                    ModelState.AddModelError("",
                        "Този превоз не може да бъде изтрит, защото има закупени билети.");

                    return View(trip);
                }
            }

            if (tripToDelete != null)
            {
                _context.Trips.Remove(tripToDelete);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private void LoadTripDropdowns(TripViewModel model)
        {
            model.Trains = _context.Trains
                .Select(t => new SelectListItem
                {
                    Value = t.Id.ToString(),
                    Text = $"{t.Name} ({t.Type.GetDisplayName()})",
                    Selected = t.Id == model.TrainId
                }).ToList();

            model.Stations = _context.Stations
                .Select(s => new SelectListItem
                {
                    Value = s.Id.ToString(),
                    Text = s.Name,
                    Selected = s.Id == model.StartStationId 
                    || s.Id == model.EndStationId
                }).ToList();
        }

    }
}
