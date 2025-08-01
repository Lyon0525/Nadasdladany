using System;
using System.ComponentModel.DataAnnotations;

namespace Nadasdladany.ViewModels
{
    public class EditOfficeHourViewModel
    {
        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage = "A nap kiválasztása kötelező.")]
        [Display(Name = "Nap")]
        public DayOfWeek DayOfWeek { get; set; }

        [Required(ErrorMessage = "Az időpont leírása kötelező.")]
        [StringLength(100)]
        [Display(Name = "Időpont / Leírás")]
        public string TimeDescription { get; set; }

        [Display(Name = "Megjelenítési Sorrend")]
        [Range(0, 100)]
        public int DisplayOrder { get; set; }
    }
}