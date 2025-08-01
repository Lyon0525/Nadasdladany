using System.ComponentModel.DataAnnotations;

namespace Nadasdladany.ViewModels
{
    public class EditDocumentCategoryViewModel
    {
        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage = "A kategória neve kötelező.")]
        [StringLength(100, ErrorMessage = "A név maximum 100 karakter lehet.")]
        public string Name { get; set; }

        [StringLength(255)]
        public string? Description { get; set; }
    }
}