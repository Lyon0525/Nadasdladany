using System.ComponentModel.DataAnnotations;

namespace Nadasdladany.Models
{
    public class Event
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Az esemény címe kötelező.")]
        [StringLength(150, ErrorMessage = "Az esemény címe maximum 150 karakter lehet.")]
        [Display(Name = "Esemény Címe")]
        public string Title { get; set; }

        [Display(Name = "Leírás")]
        public string? Description { get; set; } // HTML content for more details

        [Required(ErrorMessage = "Az esemény kezdő időpontja kötelező.")]
        [Display(Name = "Kezdés Időpontja")]
        public DateTime StartDate { get; set; }

        [Display(Name = "Befejezés Időpontja")]
        public DateTime? EndDate { get; set; } // Optional, for multi-day events

        [StringLength(200)]
        [Display(Name = "Helyszín")]
        public string? Location { get; set; }

        [Display(Name = "Egész Napos")]
        public bool IsAllDay { get; set; } = false;

        [StringLength(100)]
        [Display(Name = "Szervező")]
        public string? Organizer { get; set; }

        [StringLength(150)]
        [Display(Name = "Elérhetőség (Szervező)")]
        public string? ContactInfo { get; set; } // Email or phone

        [Url(ErrorMessage = "Érvénytelen URL formátum.")]
        [StringLength(255)]
        [Display(Name = "Esemény Weboldala (URL)")]
        public string? EventUrl { get; set; } // Link for more info or registration

        // --- Optional but Recommended Additions ---
        [Display(Name = "Publikálva")]
        public bool IsPublished { get; set; } = true;

        [StringLength(160, ErrorMessage = "A slug maximum 160 karakter lehet.")]
        [RegularExpression("^[a-z0-9]+(?:-[a-z0-9]+)*$", ErrorMessage = "A slug csak kisbetűket, számokat és kötőjelet tartalmazhat (pl. esemeny-neve-2024).")]
        [Display(Name = "Keresőbarát URL (Slug)")]
        public string? Slug { get; set; } // For SEO-friendly URLs like /events/details/event-title-slug
    }
}