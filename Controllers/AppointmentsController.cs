using System;
using System.Collections.Generic;
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
            var applicationDbContext = _context.Appointments.Include(a => a.Service).Include(a => a.Trainer);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Appointments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointment = await _context.Appointments
                .Include(a => a.Service)
                .Include(a => a.Trainer)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (appointment == null)
            {
                return NotFound();
            }

            return View(appointment);
        }

        // GET: Appointments/Create
        public IActionResult Create()
        {
            ViewData["ServiceId"] = new SelectList(_context.Services, "Id", "Name");
            ViewData["TrainerId"] = new SelectList(_context.Trainers, "Id", "Name");
            return View();
        }

        // POST: Appointments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,MemberName,Date,Time,ServiceId,TrainerId,Price,IsApproved")] Appointment appointment)
        {
            // ÇAKIŞMA KONTROLÜ
            bool isConflict = _context.Appointments.Any(a =>
                a.TrainerId == appointment.TrainerId &&
                a.Date == appointment.Date &&
                a.Time == appointment.Time
            );

            if (isConflict)
            {
                ModelState.AddModelError("", "Bu eğitmen bu tarih ve saatte zaten dolu.");

                // Dropdown'ları tekrar doldur
                ViewData["ServiceId"] = new SelectList(_context.Services, "Id", "Name", appointment.ServiceId);
                ViewData["TrainerId"] = new SelectList(_context.Trainers, "Id", "Name", appointment.TrainerId);

                return View(appointment);
            }

            // Normal kayıt
            if (ModelState.IsValid)
            {
                _context.Add(appointment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["ServiceId"] = new SelectList(_context.Services, "Id", "Name", appointment.ServiceId);
            ViewData["TrainerId"] = new SelectList(_context.Trainers, "Id", "Name", appointment.TrainerId);
            return View(appointment);
        }


        // GET: Appointments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null)
            {
                return NotFound();
            }
            ViewData["ServiceId"] = new SelectList(_context.Services, "Id", "Name", appointment.ServiceId);
            ViewData["TrainerId"] = new SelectList(_context.Trainers, "Id", "Name", appointment.TrainerId);
            return View(appointment);
        }

        // POST: Appointments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,MemberName,Date,Time,ServiceId,TrainerId,Price,IsApproved")] Appointment appointment)
        {
            if (id != appointment.Id)
            {
                return NotFound();
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
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ServiceId"] = new SelectList(_context.Services, "Id", "Name", appointment.ServiceId);
            ViewData["TrainerId"] = new SelectList(_context.Trainers, "Id", "Name", appointment.TrainerId);
            return View(appointment);
        }

        // GET: Appointments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appointment = await _context.Appointments
                .Include(a => a.Service)
                .Include(a => a.Trainer)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (appointment == null)
            {
                return NotFound();
            }

            return View(appointment);
        }

        // POST: Appointments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment != null)
            {
                _context.Appointments.Remove(appointment);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AppointmentExists(int id)
        {
            return _context.Appointments.Any(e => e.Id == id);
        }

        [HttpGet]
    public IActionResult GetAvailableHours(int trainerId)
    {
        var trainer = _context.Trainers.FirstOrDefault(t => t.Id == trainerId);
        if (trainer == null) return NotFound();

        // Çalışma saatlerini parse et
        var parts = trainer.WorkingHours.Split('-');  // "09:00-18:00"
        var start = TimeOnly.Parse(parts[0]);
        var end = TimeOnly.Parse(parts[1]);

        List<string> hours = new List<string>();

        for (var time = start; time <= end; time = time.AddHours(1))
        {
            hours.Add(time.ToString("HH:mm"));
        }

        return Json(hours);
    }

    [HttpGet]
    public IActionResult GetServicePrice(int serviceId)
    {
        var service = _context.Services.FirstOrDefault(s => s.Id == serviceId);
        if (service == null)
            return Json(0);

        return Json(service.Price);
    }

        [HttpGet]
        public IActionResult GetServicesByTrainer(int trainerId)
        {
            var services = _context.TrainerServices
                .Where(ts => ts.TrainerId == trainerId)
                .Select(ts => new 
                {
                    id = ts.Service.Id,
                    name = ts.Service.Name
                })
                .ToList();

            return Json(services);
        }


    }

    
}
