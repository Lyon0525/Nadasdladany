using System.ComponentModel.DataAnnotations;

namespace Nadasdladany.Models
{
    public class Representative
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "A név megadása kötelező.")]
        [StringLength(150)]
        [Display(Name = "Név")]
        public string Name { get; set; }

        [Required(ErrorMessage = "A beosztás/szerepkör megadása kötelező.")]
        [Display(Name = "Beosztás/Szerepkör")]
        public RepresentativeRole Role { get; set; }

        [Display(Name = "Egyéni Cím / Titulus")] // e.g., "Képviselő, Pénzügyi Bizottság Elnöke" if Role is Kepviselo
        [StringLength(200)]
        public string? CustomTitleOverride { get; set; }


        [EmailAddress(ErrorMessage = "Érvénytelen email formátum.")]
        [StringLength(100)]
        [Display(Name = "E-mail cím")]
        public string? Email { get; set; }

        [StringLength(30)]
        [Display(Name = "Telefonszám")]
        public string? PhoneNumber { get; set; }

        [Display(Name = "Fogadóóra Leírása")]
        public string? ReceptionHoursInfo { get; set; } // e.g., "Minden hónap első keddjén 10-12 óra között, előzetes egyeztetéssel."

        [Display(Name = "Fotó URL")]
        [StringLength(255)]
        public string? ImageUrl { get; set; } // Relative path like /images/representatives/kepviselo.jpg

        [Display(Name = "Rövid Bemutatkozás")]
        public string? Biography { get; set; } // HTML content could be allowed

        [Display(Name = "Publikálva")]
        public bool IsPublished { get; set; } = true;

        [Display(Name = "Megjelenítési Sorrend")]
        public int DisplayOrder { get; set; } // To control order in lists
    }
}