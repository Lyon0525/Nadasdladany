using System.ComponentModel.DataAnnotations;

namespace Nadasdladany.Models
{
    public class OfficeHourEntry // For the Polgármesteri Hivatal's general opening hours
    {
        public int Id { get; set; }

        [Required]
        public DayOfWeek DayOfWeek { get; set; } // 0=Sunday, 1=Monday...

        [StringLength(100, ErrorMessage = "Az időpont leírása túl hosszú.")]
        [Display(Name = "Időpont Leírása")] // e.g., "8:00 - 12:00 és 13:00 - 16:00" or "Nincs ügyfélfogadás"
        public string TimeDescription { get; set; }

        [Display(Name = "Sorrend")] // To control display order if needed
        public int DisplayOrder { get; set; }
    }
}