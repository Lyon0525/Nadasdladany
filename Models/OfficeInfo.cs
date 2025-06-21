using System.ComponentModel.DataAnnotations;

namespace Nadasdladany.Models
{
    public class OfficeInfo
    {
        public int Id { get; set; } // Primary key, though we might only have one record

        [Required(ErrorMessage = "A hivatal neve kötelező.")]
        [StringLength(150)]
        [Display(Name = "Hivatal Neve")]
        public string OfficeName { get; set; } = "Polgármesteri Hivatal";

        [StringLength(5000)]
        [Display(Name = "A Hivatalról (leírás)")]
        public string? AboutOffice { get; set; } // HTML content supported

        [Required(ErrorMessage = "A cím megadása kötelező.")]
        [StringLength(200)]
        [Display(Name = "Cím")]
        public string Address { get; set; }

        [StringLength(30)]
        [Display(Name = "Telefonszám")]
        public string? PhoneNumber { get; set; }

        [EmailAddress(ErrorMessage = "Érvénytelen email formátum.")]
        [StringLength(100)]
        [Display(Name = "E-mail cím")]
        public string? Email { get; set; }

        [Url(ErrorMessage = "Érvénytelen URL formátum.")]
        [StringLength(255)]
        [Display(Name = "Honlap (ha eltérő)")]
        public string? WebsiteUrl { get; set; }

        // You could add more fields like a direct link to a Google Maps embed URL
        [StringLength(500)]
        [Display(Name = "Google Térkép Beágyazási Kód/URL")]
        public string? GoogleMapsEmbedUrl { get; set; }
    }
}