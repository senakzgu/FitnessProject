using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using FitnessApp.Data;
using FitnessApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FitnessApp.Controllers
{
    public class AppointmentsController : Controller
    {
        private readonly ApplicationDbContext _context;
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

            return View();
        }


        // --------------------------------------------------------
        // 5) ÜYE: RANDEVU OLUŞTURMA POST
        // --------------------------------------------------------
        [HttpPost]
        [Authorize(Roles = "Uye")]
        public IActionResult Create(Appointment appointment)
        {
            appointment.UserId = _userManager.GetUserId(User);
            appointment.IsApproved = false;

            // ================================
            //  ÇAKIŞMA KONTROLÜ
            // ================================
            var hasConflict = _context.Appointments.Any(a =>
                a.TrainerId == appointment.TrainerId &&
                a.Date == appointment.Date &&
                a.Time == appointment.Time
            );

            if (hasConflict)
            {
                TempData["Error"] = "Seçtiğiniz eğitmen bu saat için zaten randevuya sahip. Lütfen başka bir saat seçin.";
                return RedirectToAction("Create");
            }

            // ================================
            //  RANDEVU KAYDETME
            // ================================
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

        [HttpGet]
        public IActionResult GetServicesByGym(int gymId)
        {
            var services = _context.Services
                .Where(s => s.GymId == gymId)
                .Select(s => new { s.Id, s.Name })
                .ToList();

            return Json(services);
        }

        [HttpGet]
        public IActionResult GetTrainersByService(int serviceId)
        {
            var trainers = _context.TrainerServices
                .Where(ts => ts.ServiceId == serviceId)
                .Select(ts => new { ts.Trainer.Id, ts.Trainer.Name })
                .ToList();

            return Json(trainers);
        }

        [HttpGet]
        public IActionResult GetAvailableHours(int trainerId)
        {
            var hours = new List<string>
            {
                "10:00", "11:00", "12:00", "13:00",
                "14:00", "15:00", "16:00"
            };

            return Json(hours);
        }

        [HttpGet]
        public IActionResult GetServicePrice(int serviceId)
        {
            var price = _context.Services
                .Where(s => s.Id == serviceId)
                .Select(s => s.Price)
                .FirstOrDefault();

            return Json(price);
        }

        // --------------------------------------------------------
        // 6) ÜYE: RANDEVU İPTAL
        // --------------------------------------------------------
        [Authorize(Roles = "Uye")]
        public IActionResult Cancel(int id)
        {
            var userId = _userManager.GetUserId(User);

            var appointment = _context.Appointments
                .FirstOrDefault(a => a.Id == id && a.UserId == userId);

            if (appointment == null)
                return Unauthorized();

            // -------------------------
            // 1) Tarih + Saati DateTime'a dönüştür
            // -------------------------
            if (!TimeSpan.TryParse(appointment.Time, out var ts))
            {
                TempData["Error"] = "Randevu saati geçersiz.";
                return RedirectToAction("MyAppointments");
            }

            var timeOnly = new TimeOnly(ts.Hours, ts.Minutes);
            var appointmentDateTime = appointment.Date.ToDateTime(timeOnly);

            var now = DateTime.Now;

            // -------------------------
            // 2) ONAYLI MI?
            // -------------------------
            if (appointment.IsApproved)
            {
                // Onaylandıysa → 8 saat kuralı zorunlu
                var hoursLeft = (appointmentDateTime - now).TotalHours;

                if (hoursLeft < 8)
                {
                    TempData["Error"] =
                        "Onaylanan randevular, randevuya 8 saatten az kalmışsa iptal edilemez.";
                    return RedirectToAction("MyAppointments");
                }
            }

            // -------------------------
            // 3) ONAYSIZ → direkt iptal edilebilir
            // -------------------------

            _context.Appointments.Remove(appointment);
            _context.SaveChanges();

            TempData["Success"] = "Randevu başarıyla iptal edildi.";
            return RedirectToAction("MyAppointments");
        }




    }
}