using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Nadasdladany.Models // Your models namespace
{
    public class GalleryAlbum
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Az album címe kötelező.")]
        [StringLength(100, ErrorMessage = "Az album címe maximum 100 karakter lehet.")]
        [Display(Name = "Album Címe")]
        public string Title { get; set; }

        [StringLength(500)]
        [Display(Name = "Album Leírása")]
        public string? Description { get; set; }

        [StringLength(110)]
        [RegularExpression("^[a-z0-9]+(?:-[a-z0-9]+)*$", ErrorMessage = "A slug csak kisbetűket, számokat és kötőjelet tartalmazhat.")]
        [Display(Name = "Keresőbarát URL (Slug)")]
        public string? Slug { get; set; }

        [Display(Name = "Létrehozás Dátuma")]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        [Display(Name = "Publikálva")]
        public bool IsPublished { get; set; } = true;

        [Display(Name = "Megjelenítési Sorrend")]
        public int DisplayOrder { get; set; } = 0;

        // Navigation property: An album can contain many images
        public ICollection<GalleryImage> Images { get; set; } = new List<GalleryImage>();
    }
}