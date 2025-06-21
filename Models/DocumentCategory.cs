using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata;

namespace Nadasdladany.Models // Your model namespace
{
    public class DocumentCategory
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "A kategória neve kötelező.")]
        [StringLength(100, ErrorMessage = "A kategória neve maximum 100 karakter lehet.")]
        [Display(Name = "Kategória Neve")]
        public string Name { get; set; }

        [StringLength(250)]
        [Display(Name = "Leírás")]
        public string? Description { get; set; }

        [StringLength(110)] // Slightly longer than name for uniqueness
        [RegularExpression("^[a-z0-9]+(?:-[a-z0-9]+)*$", ErrorMessage = "A slug csak kisbetűket, számokat és kötőjelet tartalmazhat.")]
        [Display(Name = "Keresőbarát URL (Slug)")]
        public string? Slug { get; set; }

        public ICollection<Document> Documents { get; set; } = new List<Document>();
    }
}