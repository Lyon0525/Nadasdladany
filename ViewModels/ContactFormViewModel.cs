using System.ComponentModel.DataAnnotations;

namespace Nadasdladany.ViewModels // Or NadasdladanyWebApp.MVC.ViewModels if you use a ViewModels folder
{
    public class ContactFormViewModel
    {
        [Required(ErrorMessage = "Kérjük, adja meg a nevét.")]
        [StringLength(100, ErrorMessage = "A név maximum 100 karakter lehet.")]
        [Display(Name = "Teljes Név")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Kérjük, adja meg az e-mail címét.")]
        [EmailAddress(ErrorMessage = "Kérjük, adjon meg egy érvényes e-mail címet.")]
        [StringLength(100, ErrorMessage = "Az e-mail cím maximum 100 karakter lehet.")]
        [Display(Name = "E-mail Cím")]
        public string Email { get; set; }

        [StringLength(150, ErrorMessage = "A tárgy maximum 150 karakter lehet.")]
        [Display(Name = "Tárgy")]
        public string? Subject { get; set; } // Optional subject

        [Required(ErrorMessage = "Kérjük, írja le az üzenetét.")]
        [StringLength(2000, ErrorMessage = "Az üzenet nem haladhatja meg a 2000 karaktert.")]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Üzenet")]
        public string Message { get; set; }
    }
}