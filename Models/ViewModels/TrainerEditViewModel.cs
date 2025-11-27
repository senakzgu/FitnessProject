using FitnessApp.Models;

namespace FitnessApp.Models.ViewModels
{
    public class TrainerEditViewModel
    {
        public Trainer Trainer { get; set; } = new Trainer();

        public List<Service> AllServices { get; set; } = new();
        public List<int> SelectedServiceIds { get; set; } = new();
    }
}
