using System.ComponentModel.DataAnnotations;

namespace Nadasdladany.ViewModels
{
    public class CreateUsefulLinkViewModel
    {
        [Required(ErrorMessage = "A cím megadása kötelező.")]
        [StringLength(150)]
        public string Title { get; set; }

        [Required(ErrorMessage = "Az URL megadása kötelező.")]
        [Url(ErrorMessage = "Érvényes URL címet adjon meg (pl. https://pelda.hu).")]
        [StringLength(255)]
        public string Url { get; set; }

        [StringLength(200)]
        public string? Description { get; set; }

        [StringLength(100)]
        public string? Category { get; set; }

        public bool OpenInNewTab { get; set; } = true;
    }
}