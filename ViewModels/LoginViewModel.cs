using System.ComponentModel.DataAnnotations;

namespace Nadasdladany.ViewModels // Or your chosen ViewModels namespace
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "E-mail cím megadása kötelező.")]
        [EmailAddress(ErrorMessage = "Érvénytelen e-mail cím formátum.")]
        [Display(Name = "E-mail cím")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Jelszó megadása kötelező.")]
        [DataType(DataType.Password)]
        [Display(Name = "Jelszó")]
        public string Password { get; set; }

        [Display(Name = "Emlékezz rám?")]
        public bool RememberMe { get; set; }
    }
}