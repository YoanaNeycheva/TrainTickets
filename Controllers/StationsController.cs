using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TrainTickets.Data;
using TrainTickets.Models;

namespace TrainTickets.Controllers
{
    public class StationsController : BaseAdminController
    {
        private readonly ApplicationDbContext _context;

        public StationsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager) 
            : base(userManager)
        {
            _context = context;
        }

        // GET: Stations
        public async Task<IActionResult> Index()
        {
            if (!await IsAdmin())
            {
                return Forbid();
            }
            return View(await _context.Stations.ToListAsync());
        }

        // GET: Stations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var station = await _context.Stations
                .FirstOrDefaultAsync(m => m.Id == id);
            if (station == null)
            {
                return NotFound();
            }

            return View(station);
        }

        // GET: Stations/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Stations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,City")] Station station)
        {
            if (ModelState.IsValid)
            {
                _context.Add(station);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(station);
        }

        // GET: Stations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var station = await _context.Stations.FindAsync(id);
            if (station == null)
            {
                return NotFound();
            }
            return View(station);
        }

        // POST: Stations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,City")] Station station)
        {
            if (id != station.Id)
            {
                return NotFound();
            }

            var tripIds = await _context.Trips
                .Where(t => t.StartStationId == id || t.EndStationId == id)
                .Select(t => t.Id)
                .ToListAsync();

            // има ли билети за тези Trips
            bool hasTickets = await _context.Tickets
                .AnyAsync(t => tripIds.Contains(t.TripId));

            if (hasTickets)
            {
                ModelState.AddModelError("",
                    "Не може да редактирате гарата, защото има закупени " +
                    "билети за превози, които я използват.");

                return View(station);
            }

            if(!ModelState.IsValid)
            {
                return View(station);
            }

            _context.Update(station);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Stations/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var station = await _context.Stations
                .FirstOrDefaultAsync(m => m.Id == id);
            if (station == null)
            {
                return NotFound();
            }

            return View(station);
        }

        // POST: Stations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Намираме всички Trips, в които участва гарата
            var trips = await _context.Trips
                .Where(t => t.StartStationId == id || t.EndStationId == id)
                .Select(t => new
                {
                    Trip = t,
                    HasTickets = _context.Tickets.Any(ticket => ticket.TripId == t.Id)
                })
                .ToListAsync();

            // Ако някой Trip има билети → забраняваме изтриването
            if (trips.Any(t => t.HasTickets))
            {
                var station = await _context.Stations.FindAsync(id);

                ModelState.AddModelError("",
                    "Не може да изтриете тази гара, защото има закупени " +
                    "билети за превози, които минават през нея.");

                return View(station);
            }

            // Няма билети → трием всички Trips
            var tripsToDelete = trips.Select(t => t.Trip).ToList();
            _context.Trips.RemoveRange(tripsToDelete);

            // Трием гарата
            var stationToDelete = await _context.Stations.FindAsync(id);
            if (stationToDelete != null)
            {
                _context.Stations.Remove(stationToDelete);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
