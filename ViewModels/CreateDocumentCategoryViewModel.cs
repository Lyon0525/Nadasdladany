using System.ComponentModel.DataAnnotations;

namespace Nadasdladany.ViewModels
{
    public class CreateDocumentCategoryViewModel
    {
        [Required(ErrorMessage = "A kategória neve kötelező.")]
        [StringLength(100, ErrorMessage = "A név maximum 100 karakter lehet.")]
        public string Name { get; set; }

        [StringLength(255)]
        public string? Description { get; set; }
    }
}