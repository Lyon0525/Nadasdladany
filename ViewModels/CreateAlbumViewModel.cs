using System.ComponentModel.DataAnnotations;

namespace Nadasdladany.ViewModels
{
    public class CreateAlbumViewModel
    {
        [Required(ErrorMessage = "Az album címe kötelező.")]
        [StringLength(100, ErrorMessage = "A cím maximum 100 karakter lehet.")]
        [Display(Name = "Album Címe")]
        public string Title { get; set; }

        [StringLength(500)]
        [Display(Name = "Leírás (opcionális)")]
        public string? Description { get; set; }

        [Display(Name = "Megjelenítési Sorrend")]
        [Range(0, 100, ErrorMessage = "A sorrend 0 és 100 között lehet.")]
        public int DisplayOrder { get; set; } = 99;
    }
}