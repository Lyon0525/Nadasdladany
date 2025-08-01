using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Nadasdladany.ViewModels
{
    public class CreateInstitutionViewModel
    {
        [Required(ErrorMessage = "Az intézmény nevének megadása kötelező.")]
        [StringLength(150)]
        public string Name { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        [StringLength(200)]
        public string? Address { get; set; }

        [StringLength(30)]
        public string? PhoneNumber { get; set; }

        [EmailAddress]
        [StringLength(100)]
        public string? Email { get; set; }

        [Url]
        [StringLength(255)]
        public string? WebsiteUrl { get; set; }

        // This will be used for the file upload
        public IFormFile? ImageFile { get; set; }
    }
}