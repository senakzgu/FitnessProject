namespace FitnessApp.Models
{
    public class Service
    {
        public int Id { get; set; }
        public string? Name { get; set; }        // Hizmet adı: Fitness, Yoga...
        public int Duration { get; set; }       // Dakika cinsinden
        public decimal Price { get; set; }      // Ücret

        // Bağlı olduğu salon
        public int GymId { get; set; }
        public Gym? Gym { get; set; }
        public List<TrainerService>? TrainerServices { get; set; }

    }
}
