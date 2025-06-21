using System.ComponentModel.DataAnnotations;

namespace Nadasdladany.Models // Your models namespace
{
    public class Institution
    {
        [Key]
        public int Id { get; set; } // Primary Key for the database table

        [Required(ErrorMessage = "Az intézmény neve kötelező.")]
        [StringLength(150, ErrorMessage = "A név maximum 150 karakter lehet.")]
        [Display(Name = "Intézmény Neve")]
        public string Name { get; set; }

        [Display(Name = "Leírás")]
        public string? Description { get; set; } // Can be a short intro, HTML if needed

        [StringLength(255)]
        [Display(Name = "Cím")]
        public string? Address { get; set; }

        [StringLength(30)]
        [Display(Name = "Telefonszám")]
        public string? PhoneNumber { get; set; }

        [EmailAddress(ErrorMessage = "Érvénytelen email formátum.")]
        [StringLength(100)]
        [Display(Name = "E-mail cím")]
        public string? Email { get; set; }

        [Url(ErrorMessage = "Érvénytelen URL formátum.")]
        [StringLength(255)]
        [Display(Name = "Weboldal (URL)")]
        public string? WebsiteUrl { get; set; }

        [StringLength(255)]
        [Display(Name = "Kép URL")]
        public string? ImageUrl { get; set; } // e.g., /images/institutions/iskola.jpg

        [StringLength(50)]
        [Display(Name = "Ikon CSS Osztály")]
        public string? IconCssClass { get; set; } // e.g., "bi bi-book-half"

        [StringLength(160)] // For routing to a potential detail page
        [RegularExpression("^[a-z0-9]+(?:-[a-z0-9]+)*$", ErrorMessage = "A slug csak kisbetűket, számokat és kötőjelet tartalmazhat.")]
        [Display(Name = "Keresőbarát URL (Slug)")]
        public string? Slug { get; set; } // e.g., "iskola", "ovoda"

        [Display(Name = "Publikálva")]
        public bool IsPublished { get; set; } = true;

        [Display(Name = "Megjelenítési Sorrend")]
        public int DisplayOrder { get; set; } = 0; // To control the order they appear in lists
    }
}