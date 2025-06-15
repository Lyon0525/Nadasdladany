using System.ComponentModel.DataAnnotations;

namespace Nadasdladany.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "A kategória neve kötelező.")]
        [StringLength(100, ErrorMessage = "A kategória neve maximum 100 karakter lehet.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "A \"slug\" kötelező.")]
        [StringLength(100, ErrorMessage = "A \"slug\" maximum 100 karakter lehet.")]
        public string Slug { get; set; } // For SEO-friendly URLs, e.g., "onkormanyzati-hirek"

        public string? Description { get; set; }

        public virtual ICollection<Article> Articles { get; set; } = new List<Article>();
    }
}
