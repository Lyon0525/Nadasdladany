using Microsoft.AspNetCore.Http; // ADD THIS DIRECTIVE for IFormFile
using System.ComponentModel.DataAnnotations;

namespace Nadasdladany.ViewModels
{
    public class EditMayorViewModel
    {
        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage = "A név megadása kötelező.")]
        [StringLength(150)]
        public string Name { get; set; }

        [StringLength(200)]
        public string? CustomTitleOverride { get; set; }

        [EmailAddress]
        [StringLength(100)]
        public string? Email { get; set; }

        [StringLength(30)]
        public string? PhoneNumber { get; set; }

        public string? ReceptionHoursInfo { get; set; }

        public string? Biography { get; set; }

        public IFormFile? ImageFile { get; set; }

        public bool RemoveCurrentImage { get; set; }
    }
}