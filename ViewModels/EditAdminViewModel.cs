using System.ComponentModel.DataAnnotations;

namespace Nadasdladany.ViewModels
{
    public class EditAdminViewModel
    {
        [Required]
        public string Id { get; set; }

        [Required(ErrorMessage = "Az e-mail cím kötelező.")]
        [EmailAddress]
        public string Email { get; set; }

        // Optional: for resetting password
        [StringLength(100, ErrorMessage = "A {0} legalább {2} és legfeljebb {1} karakter hosszú lehet.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Új Jelszó (nem kötelező)")]
        public string? NewPassword { get; set; }
    }
}
