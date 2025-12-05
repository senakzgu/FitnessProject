namespace FitnessApp.Models
{
    public class Appointment
    {
        public int Id { get; set; }

<<<<<<< HEAD
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
=======
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

>>>>>>> 101432024641ce337e528306d3ec0ee1ec850162
    }
}
