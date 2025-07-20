using System.ComponentModel.DataAnnotations;

namespace Nadasdladany.ViewModels
{
    public class EditArticleViewModel
    {
        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage = "A cikk címe kötelező.")]
        [StringLength(200, ErrorMessage = "A cikk címe maximum 200 karakter lehet.")]
        public string Title { get; set; }

        [Required(ErrorMessage = "A cikk tartalma kötelező.")]
        public string Content { get; set; }

        [StringLength(500, ErrorMessage = "A kivonat maximum 500 karakter lehet.")]
        public string? Excerpt { get; set; }

        [StringLength(255)]
        public string? FeaturedImageUrl { get; set; }

        [Display(Name = "Új Kiemelt Kép Feltöltése")]
        public IFormFile? FeaturedImageFile { get; set; }

        public string? ExistingImageUrl { get; set; }

        public bool RemoveCurrentImage { get; set; }

        [Required(ErrorMessage = "Kategória kiválasztása kötelező.")]
        public int CategoryId { get; set; }
    }
}