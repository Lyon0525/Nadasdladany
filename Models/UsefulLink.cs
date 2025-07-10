using System.ComponentModel.DataAnnotations;

namespace Nadasdladany.Models
{
    public class UsefulLink
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "A link szövege kötelező.")]
        [StringLength(200, ErrorMessage = "A link szövege maximum 200 karakter lehet.")]
        [Display(Name = "Link Szövege / Cím")]
        public string Title { get; set; }

        [Required(ErrorMessage = "A link URL címe kötelező.")]
        [StringLength(500, ErrorMessage = "Az URL maximum 500 karakter lehet.")]
        [Url(ErrorMessage = "Érvénytelen URL formátum.")]
        [Display(Name = "Link URL")]
        public string Url { get; set; }

        [StringLength(500)]
        [Display(Name = "Rövid Leírás (opcionális)")]
        public string? Description { get; set; }

        [Display(Name = "Új Lapon Nyíljon Meg")]
        public bool OpenInNewTab { get; set; } = true; // Default to opening external links in new tab

        [StringLength(100)]
        [Display(Name = "Kategória (opcionális)")] // e.g., "Kormányzati", "Helyi Szolgáltatók", "Kultúra"
        public string? Category { get; set; }

        [Display(Name = "Publikálva")]
        public bool IsPublished { get; set; } = true;

        [Display(Name = "Megjelenítési Sorrend")]
        public int DisplayOrder { get; set; } = 0;
    }
}