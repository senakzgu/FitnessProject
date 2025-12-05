using System.ComponentModel.DataAnnotations;

namespace FitnessApp.Models.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Email alanı zorunludur.")]
        [EmailAddress(ErrorMessage = "Geçerli bir mail adresi giriniz.")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Şifre zorunludur.")]
        [MinLength(6, ErrorMessage = "Şifre en az 6 karakter olmalıdır.")]
        [RegularExpression(
            @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z0-9]).+$",
            ErrorMessage = "Şifre en az bir büyük harf, bir küçük harf, bir rakam ve bir özel karakter içermelidir."
        )]


        public string? Password { get; set; }

        [Required(ErrorMessage = "Şifre tekrar zorunludur.")]
        [Compare("Password", ErrorMessage = "Şifreler eşleşmiyor.")]
        public string? ConfirmPassword { get; set; }

        [Phone(ErrorMessage = "Lütfen geçerli bir telefon giriniz.")]
        public string? Phone { get; set; }
    }
}
