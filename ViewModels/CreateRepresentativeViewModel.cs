using Microsoft.AspNetCore.Http; // ADD THIS for IFormFile
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

        [StringLength(200)]
        [Display(Name = "Egyéni Cím / Titulus")]
        public string? CustomTitleOverride { get; set; }

        [EmailAddress(ErrorMessage = "Érvénytelen email formátum.")]
        [StringLength(100)]
        [Display(Name = "E-mail cím")]
        public string? Email { get; set; }

        [StringLength(30)]
        [Display(Name = "Telefonszám")]
        public string? PhoneNumber { get; set; }

        [Display(Name = "Rövid Bemutatkozás")]
        public string? Biography { get; set; }

        [Required(ErrorMessage = "A megjelenítési sorrend kötelező.")]
        [Range(0, 100)]
        [Display(Name = "Megjelenítési Sorrend")]
        public int DisplayOrder { get; set; } = 99;

        [Display(Name = "Fotó feltöltése")]
        public IFormFile? ImageFile { get; set; }
    }
}