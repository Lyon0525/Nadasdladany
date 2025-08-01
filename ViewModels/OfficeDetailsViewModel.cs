using System.ComponentModel.DataAnnotations;

namespace Nadasdladany.ViewModels
{
    public class OfficeDetailsViewModel
    {
        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage = "A hivatal nevének megadása kötelező.")]
        [StringLength(150)]
        public string OfficeName { get; set; }

        public string? AboutOffice { get; set; }

        [StringLength(200)]
        public string? Address { get; set; }

        [StringLength(30)]
        public string? PhoneNumber { get; set; }

        [EmailAddress]
        [StringLength(100)]
        public string? Email { get; set; }

        [Url]
        [StringLength(255)]
        public string? WebsiteUrl { get; set; }

        public string? GoogleMapsEmbedUrl { get; set; }
    }
}
