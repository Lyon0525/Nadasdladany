using System.ComponentModel.DataAnnotations;

namespace Nadasdladany.ViewModels
{
    public class EditAlbumViewModel
    {
        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage = "Az album címe kötelező.")]
        [StringLength(100)]
        public string Title { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        [Display(Name = "Megjelenítési Sorrend")]
        [Range(0, 100)]
        public int DisplayOrder { get; set; }
    }
}