using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FitnessApp.Data;
using FitnessApp.Models;
using FitnessApp.Models.ViewModels; 

namespace FitnessApp.Controllers
{
    public class TrainersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TrainersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Trainers
        public async Task<IActionResult> Index()
        {
            var trainers = _context.Trainers
                .Include(t => t.Gym)
                .Include(t => t.TrainerServices)
                    .ThenInclude(ts => ts.Service);

            return View(await trainers.ToListAsync());

        }

        // GET: Trainers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var trainer = await _context.Trainers
                .Include(t => t.Gym)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (trainer == null)
            {
                return NotFound();
            }

            return View(trainer);
        }

        // GET: Trainers/Create
        public IActionResult Create()
        {
            var vm = new TrainerEditViewModel
            {
                AllServices = _context.Services.ToList()
            };

            ViewData["GymId"] = new SelectList(_context.Gyms, "Id", "Name");
            return View(vm);
        }

        // POST: Trainers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TrainerEditViewModel vm)
        {
            if (ModelState.IsValid)
            {
                // Trainer kaydet
                _context.Add(vm.Trainer);
                await _context.SaveChangesAsync();

                // Hizmet ilişkilerini ekle
                foreach (var serviceId in vm.SelectedServiceIds)
                {
                    _context.TrainerServices.Add(new TrainerService
                    {
                        TrainerId = vm.Trainer.Id,
                        ServiceId = serviceId
                    });
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            vm.AllServices = _context.Services.ToList();
            ViewData["GymId"] = new SelectList(_context.Gyms, "Id", "Name");
            return View(vm);
        }

        // GET: Trainers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var trainer = await _context.Trainers
            .Include(t => t.TrainerServices)
            .FirstOrDefaultAsync(t => t.Id == id);
            if (trainer == null)
            {
                return NotFound();
            }

            var vm = new TrainerEditViewModel
            {
                Trainer = trainer,
                AllServices = _context.Services.ToList(),
                SelectedServiceIds = trainer.TrainerServices
                                            .Select(ts => ts.ServiceId)
                                            .ToList()
            };
            ViewData["GymId"] = new SelectList(_context.Gyms, "Id", "Name", trainer.GymId);
            return View(vm);
        }

        // POST: Trainers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TrainerEditViewModel vm)
        {
            if (id != vm.Trainer.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                // Trainer güncelle
                _context.Update(vm.Trainer);
                await _context.SaveChangesAsync();

                // Eski hizmetleri sil
                var oldServices = _context.TrainerServices
                    .Where(ts => ts.TrainerId == vm.Trainer.Id);
                _context.TrainerServices.RemoveRange(oldServices);

                // Yeni hizmetleri ekle
                foreach (var serviceId in vm.SelectedServiceIds)
                {
                    _context.TrainerServices.Add(new TrainerService
                    {
                        TrainerId = vm.Trainer.Id,
                        ServiceId = serviceId
                    });
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            vm.AllServices = _context.Services.ToList();
            ViewData["GymId"] = new SelectList(_context.Gyms, "Id", "Name", vm.Trainer.GymId);
            return View(vm);
        }


        // GET: Trainers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var trainer = await _context.Trainers
                .Include(t => t.Gym)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (trainer == null)
            {
                return NotFound();
            }

            return View(trainer);
        }

        // POST: Trainers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var trainer = await _context.Trainers.FindAsync(id);
            if (trainer != null)
            {
                _context.Trainers.Remove(trainer);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TrainerExists(int id)
        {
            return _context.Trainers.Any(e => e.Id == id);
        }
    }
}
