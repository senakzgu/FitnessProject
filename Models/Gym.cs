namespace FitnessApp.Models
{
    public class Gym
    {
        public int Id { get; set; }             // Primary Key
        public string? Name { get; set; }        // Salon adı
        public string? WorkingHours { get; set; } // Örn: “08:00 - 22:00”
    }
}
