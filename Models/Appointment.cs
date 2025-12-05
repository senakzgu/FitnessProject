namespace FitnessApp.Models
{
    public class Appointment
    {
        public int Id { get; set; }

        // Randevuyu alan üyenin Identity User ID'si
        public string? UserId { get; set; }

        // Üyenin adı (isteğe bağlı)
        public string? MemberName { get; set; }

        // Tarih & Saat
        public DateOnly Date { get; set; }
        public string? Time { get; set; }

        // Hizmet
        public int ServiceId { get; set; }
        public Service? Service { get; set; }

        // Antrenör
        public int TrainerId { get; set; }
        public Trainer? Trainer { get; set; }

        // Salon
        public int GymId { get; set; }
        public Gym? Gym { get; set; }

        // Ücret (opsiyonel)
        public decimal? Price { get; set; }

        // ONAY DURUMU — Admin onay mekanizması için gerekli
        public bool IsApproved { get; set; } = false;
    }
}
