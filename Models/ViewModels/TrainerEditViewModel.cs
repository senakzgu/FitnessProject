using System.Collections.Generic;

namespace FitnessApp.Models.ViewModels
{
    public class TrainerEditViewModel
    {
        // Trainer entity'si
        public Trainer Trainer { get; set; } = new Trainer();

        // Seçilen hizmetler (checkbox)
        public List<int> SelectedServiceIds { get; set; } = new();

        // Formda listelenecek tüm hizmetler
        public List<Service> AllServices { get; set; } = new();

        // Formda salon dropdownı için
        public List<Gym> AllGyms { get; set; } = new();
    }
}

