using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TrainTickets.Data;
using TrainTickets.Enums;
using TrainTickets.Models;

namespace TrainTickets.Controllers
{
    public class TrainsController : BaseAdminController
    {
        private readonly ApplicationDbContext _context;

        public TrainsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager) 
            : base(userManager)
        {
            _context = context;
        }

        // GET: Trains
        public async Task<IActionResult> Index()
        {
            if (!await IsAdmin())
            {
                return Forbid();
            }
            return View(await _context.Trains.ToListAsync());
        }

        // GET: Trains/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var train = await _context.Trains
                .Include(t => t.Trips)
                .ThenInclude(tr => tr.StartStation)
                .Include(t => t.Trips)
                .ThenInclude(tr => tr.EndStation)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (train == null)
                return NotFound();

            return View(train);
        }

        // GET: Trains/Create
        public IActionResult Create()
        {
            ViewBag.TrainTypes = Enum.GetValues(typeof(TrainType))
                .Cast<TrainType>()
                .Select(t => new SelectListItem
                {
                    Value = t.ToString(),
                    Text = t.GetDisplayName()
                });
            return View();
        }

        // POST: Trains/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Train train)
        {
            if (train.Capacity < 10)
            {
                ModelState.AddModelError("", "Влакът " +
                    "трябва да има поне 10 места.");
                LoadTrainTypes(train);
            }
            if (train.Capacity > 200)
            {
                ModelState.AddModelError("", "Влакът " +
                    "не може да има повече от 200 места.");
                LoadTrainTypes(train);
            }

            if (!ModelState.IsValid)
            {
                ViewBag.TrainTypes = Enum.GetValues(typeof(TrainType))
                    .Cast<TrainType>()
                    .Select(t => new SelectListItem
                    {
                        Value = t.ToString(),
                        Text = t.GetDisplayName()
                    })
                    .ToList();

                return View(train);
            }

            _context.Add(train);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Trains/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var train = await _context.Trains.FindAsync(id);
            if (train == null)
            {
                return NotFound();
            }

            ViewBag.TrainTypes = Enum.GetValues(typeof(TrainType))
                .Cast<TrainType>()
                .Select(t => new SelectListItem
                {
                    Value = t.ToString(),
                    Text = t.GetDisplayName(),
                    Selected = t == train.Type
                })
                .ToList();

            return View(train);
        }

        // POST: Trains/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Train train)
        {
            if (id != train.Id)
            {
                return NotFound();
            }

            if (train.Capacity < 10)
            {
                ModelState.AddModelError("", "Влакът трябва да има поне 10 места.");
                LoadTrainTypes(train);
            }
            if (train.Capacity > 200)
            {
                ModelState.AddModelError("", "Влакът не може да има повече от 200 места.");
                LoadTrainTypes(train);
            }

            // Взимаме всички превози на този влак
            var trips = await _context.Trips
                .Where(t => t.TrainId == id)
                .Include(t => t.Tickets)
                .ToListAsync();

            // Проверка: има ли бъдещ или текущ превоз
            bool hasActiveOrFutureTrip = trips.Any(t =>
                t.DepartureTime > DateTime.Now ||
                (t.DepartureTime <= DateTime.Now && t.ArrivalTime >= DateTime.Now)
            );

            // Проверка: има ли билети
            bool hasTickets = trips.Any(t => t.Tickets.Any());

            // Забраняваме САМО ако има билети И активен/бъдещ превоз
            if (hasTickets && hasActiveOrFutureTrip)
            {
                ModelState.AddModelError("",
                    "Не може да редактирате влака, защото има активни или бъдещи превози с закупени билети.");
                LoadTrainTypes(train);
            }

            if (ModelState.IsValid)
            {
                _context.Update(train);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            LoadTrainTypes(train);
            return View(train);
        }

        // GET: Trains/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var train = await _context.Trains
                .FirstOrDefaultAsync(m => m.Id == id);
            if (train == null)
            {
                return NotFound();
            }

            return View(train);
        }

        // POST: Trains/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var train = await _context.Trains
                .Include(t => t.Trips)
                    .ThenInclude(tr => tr.Tickets)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (train == null)
                return NotFound();

            // Взимаме всички превози на този влак
            var trips = await _context.Trips
                .Where(t => t.TrainId == id)
                .Include(t => t.Tickets)
                .ToListAsync();

            // Проверка: има ли бъдещ или текущ превоз
            bool hasActiveOrFutureTrip = trips.Any(t =>
                t.DepartureTime > DateTime.Now ||
                (t.DepartureTime <= DateTime.Now && t.ArrivalTime >= DateTime.Now)
            );

            // Проверка: има ли билети
            bool hasTickets = trips.Any(t => t.Tickets.Any());

            // Забраняваме САМО ако има билети И активен/бъдещ превоз
            if (hasTickets && hasActiveOrFutureTrip)
            {
                ModelState.AddModelError("",
                    "Не може да изтриете влака, защото има активни или бъдещи" +
                    " превози с закупени билети.");
            }

            _context.Trips.RemoveRange(train.Trips);
            _context.Trains.Remove(train);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TrainExists(int id)
        {
            return _context.Trains.Any(e => e.Id == id);
        }
        private void LoadTrainTypes(Train? train = null)
        {
            ViewBag.TrainTypes = Enum.GetValues(typeof(TrainType))
                .Cast<TrainType>()
                .Select(t => new SelectListItem
                {
                    Value = t.ToString(),
                    Text = t.GetDisplayName(),
                    Selected = train != null && t == train.Type
                })
                .ToList();
        }
    }
}
