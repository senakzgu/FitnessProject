using System.ComponentModel.DataAnnotations.Schema;

namespace FitnessApp.Models
{
    public class Trainer
    {
        public int Id { get; set; }
        public string? Name { get; set; }          // Eğitmen adı
        public string? Specialty { get; set; }     // Uzmanlık alanı
        public string? WorkingHours { get; set; }  // Müsaitlik saatleri

        // Foreign Key - bağlı olduğu salon
        public int GymId { get; set; }
        public Gym? Gym { get; set; }
        public List<TrainerService>? TrainerServices { get; set; }

    }
}


