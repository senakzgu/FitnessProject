<<<<<<< HEAD
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using FitnessApp.Data;
using FitnessApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
=======
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FitnessApp.Data;
using FitnessApp.Models;
>>>>>>> 101432024641ce337e528306d3ec0ee1ec850162

namespace FitnessApp.Controllers
{
    public class AppointmentsController : Controller
    {
        private readonly ApplicationDbContext _context;
<<<<<<< HEAD
        private readonly UserManager<IdentityUser> _userManager;

        public AppointmentsController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // --------------------------------------------------------
        // 1) ADMIN: TÜM RANDEVULARI GÖRÜR
        // --------------------------------------------------------
        [Authorize(Roles = "Admin")]
        public IActionResult Index()
        {
            var allAppointments = _context.Appointments
                .Include(a => a.Trainer)
                .Include(a => a.Service)
                .Include(a => a.Gym)
                .ToList();

            return View(allAppointments);
        }

        // --------------------------------------------------------
        // 2) ADMIN: RANDEVU ONAYLAR
        // --------------------------------------------------------
        [Authorize(Roles = "Admin")]
        public IActionResult Approve(int id)
        {
            var appointment = _context.Appointments.FirstOrDefault(a => a.Id == id);
            if (appointment == null)
                return NotFound();

            appointment.IsApproved = true;
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        // --------------------------------------------------------
        // 3) ÜYE: KENDİ RANDEVULARI
        // --------------------------------------------------------
        [Authorize(Roles = "Uye")]
        public IActionResult MyAppointments()
        {
            var userId = _userManager.GetUserId(User);

            var myAppointments = _context.Appointments
                .Where(a => a.UserId == userId)
                .Include(a => a.Trainer)
                .Include(a => a.Service)
                .Include(a => a.Gym)
                .ToList();

            return View(myAppointments);
        }

        // --------------------------------------------------------
        // 4) ÜYE: RANDEVU OLUŞTURMA SAYFASI
        // --------------------------------------------------------
        [Authorize(Roles = "Uye")]
        public IActionResult Create()
        {
            // Drop-down listeleri doğru formatta gönderiyoruz
            ViewBag.GymId = new SelectList(_context.Gyms, "Id", "Name");
            ViewBag.ServiceId = new SelectList(_context.Services, "Id", "Name");

            // Eğitmen listesi AJAX ile dinamik doldurulacak ama boş SelectList gerektirir
            ViewBag.TrainerId = new SelectList(_context.Trainers, "Id", "Name");
=======

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
>>>>>>> 101432024641ce337e528306d3ec0ee1ec850162

            return View();
        }

<<<<<<< HEAD

        // --------------------------------------------------------
        // 5) ÜYE: RANDEVU OLUŞTURMA POST
        // --------------------------------------------------------
        [HttpPost]
        [Authorize(Roles = "Uye")]
        public IActionResult Create(Appointment appointment)
        {
            appointment.UserId = _userManager.GetUserId(User);
            appointment.IsApproved = false; // Yeni randevu → onay bekliyor

            if (ModelState.IsValid)
            {
                _context.Appointments.Add(appointment);
                _context.SaveChanges();
                return RedirectToAction("MyAppointments");
            }

            return View(appointment);
        }
        // ============================
        // AJAX ENDPOINTLERİ
        // ============================

=======
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
>>>>>>> 101432024641ce337e528306d3ec0ee1ec850162
        [HttpGet]
        public IActionResult GetServicesByGym(int gymId)
        {
            var services = _context.Services
                .Where(s => s.GymId == gymId)
<<<<<<< HEAD
                .Select(s => new { s.Id, s.Name })
=======
                .Select(s => new { id = s.Id, name = s.Name })
>>>>>>> 101432024641ce337e528306d3ec0ee1ec850162
                .ToList();

            return Json(services);
        }

<<<<<<< HEAD
        [HttpGet]
        public IActionResult GetTrainersByService(int serviceId)
        {
            var trainers = _context.TrainerServices
                .Where(ts => ts.ServiceId == serviceId)
                .Select(ts => new { ts.Trainer.Id, ts.Trainer.Name })
                .ToList();
=======
        // SERVICE → TRAINERS
        [HttpGet]
        public async Task<IActionResult> GetTrainersByService(int serviceId)
        {
            var trainers = await _context.TrainerServices
                .Where(ts => ts.ServiceId == serviceId)
                .Select(ts => new { id = ts.Trainer.Id, name = ts.Trainer.Name })
                .ToListAsync();
>>>>>>> 101432024641ce337e528306d3ec0ee1ec850162

            return Json(trainers);
        }

<<<<<<< HEAD
        [HttpGet]
        public IActionResult GetAvailableHours(int trainerId)
        {
            var hours = new List<string>
            {
                "10:00", "11:00", "12:00", "13:00",
                "14:00", "15:00", "16:00"
            };
=======
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
>>>>>>> 101432024641ce337e528306d3ec0ee1ec850162

            return Json(hours);
        }

<<<<<<< HEAD
        [HttpGet]
        public IActionResult GetServicePrice(int serviceId)
        {
            var price = _context.Services
                .Where(s => s.Id == serviceId)
                .Select(s => s.Price)
                .FirstOrDefault();

            return Json(price);
        }

        // İPTAL İŞLEMİ
        [Authorize(Roles = "Uye")]
        public IActionResult Cancel(int id)
        {
            var userId = _userManager.GetUserId(User);

            var appointment = _context.Appointments
                .FirstOrDefault(a => a.Id == id && a.UserId == userId);

            if (appointment == null)
                return Unauthorized(); // başka birinin randevusunu iptal edemez

            _context.Appointments.Remove(appointment);
            _context.SaveChanges();

            return RedirectToAction("MyAppointments");
        }



=======
        // SERVICE → PRICE
        [HttpGet]
        public IActionResult GetServicePrice(int serviceId)
        {
            var service = _context.Services.FirstOrDefault(s => s.Id == serviceId);
            return Json(service?.Price ?? 0);
        }
>>>>>>> 101432024641ce337e528306d3ec0ee1ec850162
    }
}
