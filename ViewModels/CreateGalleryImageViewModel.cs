using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Nadasdladany.ViewModels
{
    public class CreateGalleryImageViewModel
    {
        [Required(ErrorMessage = "A képfájl kiválasztása kötelező.")]
        [Display(Name = "Képfájl")]
        public IFormFile ImageFile { get; set; }

        [Required(ErrorMessage = "Az album kiválasztása kötelező.")]
        [Display(Name = "Album")]
        public int GalleryAlbumId { get; set; }

        [StringLength(150)]
        [Display(Name = "Cím")]
        public string? Title { get; set; }

        [StringLength(500)]
        [Display(Name = "Leírás")]
        public string? Description { get; set; }

        [StringLength(150)]
        [Display(Name = "Helyettesítő szöveg (SEO)")]
        public string? AltText { get; set; }
    }
}