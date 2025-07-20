using System.ComponentModel.DataAnnotations;

namespace Nadasdladany.ViewModels
{
    public class EditEventViewModel
    {
        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage = "Az esemény címe kötelező.")]
        [StringLength(150)]
        public string Title { get; set; }

        public string? Description { get; set; }

        [Required(ErrorMessage = "A kezdési időpont megadása kötelező.")]
        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        [StringLength(200)]
        public string? Location { get; set; }

        public bool IsAllDay { get; set; }

        [StringLength(100)]
        public string? Organizer { get; set; }

        [Url(ErrorMessage = "Érvénytelen URL formátum.")]
        [StringLength(255)]
        public string? EventUrl { get; set; }
    }
}
