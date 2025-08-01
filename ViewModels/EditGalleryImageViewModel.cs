using System.ComponentModel.DataAnnotations;

namespace Nadasdladany.ViewModels
{
    public class EditGalleryImageViewModel
    {
        [Required]
        public int Id { get; set; }

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