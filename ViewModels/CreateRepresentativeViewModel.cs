using Nadasdladany.Models;
using System.ComponentModel.DataAnnotations;

namespace Nadasdladany.ViewModels
{
    public class CreateRepresentativeViewModel
    {
        [Required(ErrorMessage = "A név megadása kötelező.")]
        [StringLength(150)]
        [Display(Name = "Név")]
        public string Name { get; set; }

        [Required(ErrorMessage = "A beosztás/szerepkör megadása kötelező.")]
        [Display(Name = "Beosztás/Szerepkör")]
        public RepresentativeRole Role { get; set; }

        [Display(Name = "Egyéni Cím / Titulus (felülírja a beosztást)")]
        [StringLength(200)]
        public string? CustomTitleOverride { get; set; }

        [EmailAddress(ErrorMessage = "Érvénytelen email formátum.")]
        [StringLength(100)]
        [Display(Name = "E-mail cím")]
        public string? Email { get; set; }

        [StringLength(30)]
        [Display(Name = "Telefonszám")]
        public string? PhoneNumber { get; set; }

        [Display(Name = "Fotó URL (pl. /img/reps/kepviselo.jpg)")]
        [StringLength(255)]
        public string? ImageUrl { get; set; }

        [Display(Name = "Rövid Bemutatkozás")]
        public string? Biography { get; set; }

        [Required(ErrorMessage = "A megjelenítési sorrend kötelező.")]
        [Range(0, 100, ErrorMessage = "A sorrend 0 és 100 között lehet.")]
        [Display(Name = "Megjelenítési Sorrend")]
        public int DisplayOrder { get; set; } = 99; // Default to last
    }
}
