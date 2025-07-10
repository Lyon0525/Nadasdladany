using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nadasdladany.Models // Your models namespace
{
    public class GalleryImage
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "A kép címe kötelező, ha nincs leírás.")]
        [StringLength(150, ErrorMessage = "A cím maximum 150 karakter lehet.")]
        [Display(Name = "Kép Címe")]
        public string? Title { get; set; } // Can be optional if description or filename is primary

        [StringLength(500, ErrorMessage = "A leírás maximum 500 karakter lehet.")]
        [Display(Name = "Leírás")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "A kép elérési útja kötelező.")]
        [StringLength(255)]
        [Display(Name = "Kép URL / Elérési Út")] // Relative path like /images/gallery/my_image.jpg
        public string ImageUrl { get; set; }

        [StringLength(255)]
        [Display(Name = "Bélyegkép URL / Elérési Út")] // Relative path like /images/gallery/thumbs/my_image_thumb.jpg
        public string? ThumbnailUrl { get; set; } // Make it optional; if null, could use ImageUrl for both

        [StringLength(200)]
        [Display(Name = "Alternatív Szöveg (Alt text)")]
        public string? AltText { get; set; }

        [Display(Name = "Feltöltés Dátuma")]
        public DateTime UploadedDate { get; set; } = DateTime.UtcNow;

        [Display(Name = "Publikálva")]
        public bool IsPublished { get; set; } = true;

        [Display(Name = "Megjelenítési Sorrend")]
        public int DisplayOrder { get; set; } = 0;

        // Optional: Foreign Key to an Album/Category
        [Display(Name = "Album")]
        public int? GalleryAlbumId { get; set; } // Nullable if an image doesn't have to belong to an album
        public GalleryAlbum? GalleryAlbum { get; set; } // Navigation property
    }
}