using System.ComponentModel.DataAnnotations;

namespace Nadasdladany.ViewModels
{
    public class CreateEventViewModel
    {
        [Required(ErrorMessage = "Az esemény címe kötelező.")]
        [StringLength(150, ErrorMessage = "A cím maximum 150 karakter lehet.")]
        [Display(Name = "Esemény címe")]
        public string Title { get; set; }

        [Display(Name = "Leírás (opcionális)")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "A kezdési időpont megadása kötelező.")]
        [Display(Name = "Kezdés időpontja")]
        public DateTime StartDate { get; set; }

        [Display(Name = "Befejezés időpontja (opcionális)")]
        public DateTime? EndDate { get; set; }

        [StringLength(200)]
        [Display(Name = "Helyszín (opcionális)")]
        public string? Location { get; set; }

        [Display(Name = "Egész napos esemény")]
        public bool IsAllDay { get; set; }

        [StringLength(100)]
        [Display(Name = "Szervező (opcionális)")]
        public string? Organizer { get; set; }

        [Url(ErrorMessage = "Kérjük, érvényes webcímet adjon meg.")]
        [StringLength(255)]
        [Display(Name = "Esemény weboldala (opcionális)")]
        public string? EventUrl { get; set; }
    }
}
