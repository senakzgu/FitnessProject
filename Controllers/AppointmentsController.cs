using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FitnessApp.Data;
using FitnessApp.Models;

namespace FitnessApp.Controllers
{
    public class AppointmentsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AppointmentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Appointments
        public async Task<IActionResult> Index()
        {
            var list = _context.Appointments
                .Include(a => a.Gym)
                .Include(a => a.Service)
                .Include(a => a.Trainer);

            return View(await list.ToListAsync());
        }

        // GET: Appointments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var appointment = await _context.Appointments
                .Include(a => a.Gym)
                .Include(a => a.Service)
                .Include(a => a.Trainer)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (appointment == null)
                return NotFound();

            return View(appointment);
        }

        // GET: Appointments/Create
        public IActionResult Create()
        {
            ViewData["GymId"] = new SelectList(_context.Gyms, "Id", "Name");
            ViewData["ServiceId"] = new SelectList(_context.Services, "Id", "Name");
            ViewData["TrainerId"] = new SelectList(_context.Trainers, "Id", "Name");

            return View();
        }

        // POST: Appointments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,MemberName,Date,Time,ServiceId,TrainerId,GymId,Price,IsApproved")] Appointment appointment)
        {
            // Çakışma kontrolü
            bool isConflict = _context.Appointments.Any(a =>
                a.TrainerId == appointment.TrainerId &&
                a.Date == appointment.Date &&
                a.Time == appointment.Time
            );

            if (isConflict)
            {
                ModelState.AddModelError("", "Bu eğitmen bu saat aralığında zaten dolu.");

                ViewData["GymId"] = new SelectList(_context.Gyms, "Id", "Name", appointment.GymId);
                ViewData["ServiceId"] = new SelectList(_context.Services, "Id", "Name", appointment.ServiceId);
                ViewData["TrainerId"] = new SelectList(_context.Trainers, "Id", "Name", appointment.TrainerId);

                return View(appointment);
            }

            if (ModelState.IsValid)
            {
                _context.Add(appointment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["GymId"] = new SelectList(_context.Gyms, "Id", "Name", appointment.GymId);
            ViewData["ServiceId"] = new SelectList(_context.Services, "Id", "Name", appointment.ServiceId);
            ViewData["TrainerId"] = new SelectList(_context.Trainers, "Id", "Name", appointment.TrainerId);

            return View(appointment);
        }

        // GET: Appointments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var appointment = await _context.Appointments
                .Include(a => a.Gym)
                .Include(a => a.Service)
                .Include(a => a.Trainer)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (appointment == null)
                return NotFound();

            // Gym dropdown
            ViewData["GymId"] = new SelectList(_context.Gyms, "Id", "Name", appointment.GymId);

            // Seçili salonun hizmetleri
            var services = _context.Services
                .Where(s => s.GymId == appointment.GymId)
                .ToList();

            ViewData["ServiceId"] = new SelectList(services, "Id", "Name", appointment.ServiceId);

            // Hizmeti veren eğitmen listesi
            var trainers = _context.TrainerServices
                .Where(ts => ts.ServiceId == appointment.ServiceId)
                .Select(ts => ts.Trainer)
                .ToList();

            ViewData["TrainerId"] = new SelectList(trainers, "Id", "Name", appointment.TrainerId);

            return View(appointment);
        }

        // POST: Appointments/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,MemberName,Date,Time,ServiceId,TrainerId,GymId,Price,IsApproved")] Appointment appointment)
        {
            if (id != appointment.Id)
                return NotFound();

            // Çakışma kontrolü
            bool isConflict = _context.Appointments.Any(a =>
                a.Id != appointment.Id &&
                a.TrainerId == appointment.TrainerId &&
                a.Date == appointment.Date &&
                a.Time == appointment.Time
            );

            if (isConflict)
            {
                ModelState.AddModelError("", "Bu eğitmen bu tarih ve saatte zaten başka bir randevuya sahip.");

                ViewData["GymId"] = new SelectList(_context.Gyms, "Id", "Name", appointment.GymId);
                return View(appointment);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(appointment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AppointmentExists(appointment.Id))
                        return NotFound();
                    else
                        throw;
                }

                return RedirectToAction(nameof(Index));
            }

            ViewData["GymId"] = new SelectList(_context.Gyms, "Id", "Name", appointment.GymId);
            return View(appointment);
        }

        // GET: Appointments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var appointment = await _context.Appointments
                .Include(a => a.Gym)
                .Include(a => a.Service)
                .Include(a => a.Trainer)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (appointment == null)
                return NotFound();

            return View(appointment);
        }

        // POST: Appointments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);

            if (appointment != null)
                _context.Appointments.Remove(appointment);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AppointmentExists(int id)
        {
            return _context.Appointments.Any(e => e.Id == id);
        }


        // -----------------------  AJAX ENDPOINTS  -----------------------

        // GYM → SERVICES
        [HttpGet]
        public IActionResult GetServicesByGym(int gymId)
        {
            var services = _context.Services
                .Where(s => s.GymId == gymId)
                .Select(s => new { id = s.Id, name = s.Name })
                .ToList();

            return Json(services);
        }

        // SERVICE → TRAINERS
        [HttpGet]
        public async Task<IActionResult> GetTrainersByService(int serviceId)
        {
            var trainers = await _context.TrainerServices
                .Where(ts => ts.ServiceId == serviceId)
                .Select(ts => new { id = ts.Trainer.Id, name = ts.Trainer.Name })
                .ToListAsync();

            return Json(trainers);
        }

        // TRAINER → HOURS
        [HttpGet]
        public IActionResult GetAvailableHours(int trainerId)
        {
            var trainer = _context.Trainers.FirstOrDefault(t => t.Id == trainerId);
            if (trainer == null) return NotFound();

            var parts = trainer.WorkingHours.Split('-');
            var start = TimeOnly.Parse(parts[0]);
            var end = TimeOnly.Parse(parts[1]);

            var hours = new List<string>();
            for (var time = start; time <= end; time = time.AddHours(1))
                hours.Add(time.ToString("HH:mm"));

            return Json(hours);
        }

        // SERVICE → PRICE
        [HttpGet]
        public IActionResult GetServicePrice(int serviceId)
        {
            var service = _context.Services.FirstOrDefault(s => s.Id == serviceId);
            return Json(service?.Price ?? 0);
        }
    }
}
