namespace FitnessApp.Models
{
    public class Appointment
    {
        public int Id { get; set; }

        // Üye bilgisi (Identity ekleyince UserId yapacağız)
        public string? MemberName { get; set; }

        // Zaman bilgileri
        public DateOnly Date { get; set; }

        public string? Time { get; set; }

        // Hizmet bilgisi
        public int ServiceId { get; set; }
        public Service? Service { get; set; }

        // Antrenör bilgisi
        public int TrainerId { get; set; }
        public Trainer? Trainer { get; set; }

        // Ücret
        public decimal? Price { get; set; }

        // Onay durumu
        public bool IsApproved { get; set; }
        public int GymId { get; set; }
        public Gym? Gym { get; set; }

    }
}
