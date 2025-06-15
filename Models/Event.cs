using System.ComponentModel.DataAnnotations;

namespace Nadasdladany.Models
{
    public class Event
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Az esemény címe kötelező.")]
        [StringLength(150, ErrorMessage = "Az esemény címe maximum 150 karakter lehet.")]
        public string Title { get; set; }

        public string? Description { get; set; } // HTML content for more details

        [Required(ErrorMessage = "Az esemény kezdő időpontja kötelező.")]
        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; } // Optional, for multi-day events

        [StringLength(200)]
        public string? Location { get; set; }

        public bool IsAllDay { get; set; } = false;

        [StringLength(100)]
        public string? Organizer { get; set; }

        [StringLength(150)]
        public string? ContactInfo { get; set; } // Email or phone

        [Url(ErrorMessage = "Érvénytelen URL formátum.")]
        [StringLength(255)]
        public string? EventUrl { get; set; } // Link for more info or registration
    }
}
