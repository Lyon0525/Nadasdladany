using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nadasdladany.Models
{
    public class Article
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "A cikk címe kötelező.")]
        [StringLength(200, ErrorMessage = "A cikk címe maximum 200 karakter lehet.")]
        public string Title { get; set; }

        [Required(ErrorMessage = "A \"slug\" kötelező.")]
        [StringLength(250, ErrorMessage = "A \"slug\" maximum 250 karakter lehet.")]
        public string Slug { get; set; } // For SEO-friendly URLs, e.g., "fontos-kozlemeny-vizelzaras"

        [Required(ErrorMessage = "A cikk tartalma kötelező.")]
        public string Content { get; set; } // HTML content

        [StringLength(500, ErrorMessage = "A kivonat maximum 500 karakter lehet.")]
        public string? Excerpt { get; set; } // Short summary

        [StringLength(255)]
        public string? FeaturedImageUrl { get; set; }

        public DateTime PublishedDate { get; set; } = DateTime.UtcNow;
        public DateTime LastModifiedDate { get; set; } = DateTime.UtcNow;

        [StringLength(100)]
        public string? Author { get; set; }

        public bool IsPublished { get; set; } = true;
        public int ViewCount { get; set; } = 0;

        [Required(ErrorMessage = "Kategória kiválasztása kötelező.")]
        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public virtual Category? Category { get; set; }
    }
}
