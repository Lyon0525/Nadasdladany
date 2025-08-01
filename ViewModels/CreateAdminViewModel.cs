using System.ComponentModel.DataAnnotations;

namespace Nadasdladany.ViewModels
{
    public class CreateAdminViewModel
    {
        [Required(ErrorMessage = "Az e-mail cím kötelező.")]
        [EmailAddress(ErrorMessage = "Érvénytelen e-mail cím formátum.")]
        [Display(Name = "E-mail cím")]
        public string Email { get; set; }

        [Required(ErrorMessage = "A jelszó kötelező.")]
        [StringLength(100, ErrorMessage = "A {0} legalább {2} és legfeljebb {1} karakter hosszú lehet.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Jelszó")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Jelszó megerősítése")]
        [Compare("Password", ErrorMessage = "A jelszó és a megerősítő jelszó nem egyezik.")]
        public string ConfirmPassword { get; set; }
    }
}
