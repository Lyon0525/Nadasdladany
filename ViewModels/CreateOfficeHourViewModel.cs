using System;
using System.ComponentModel.DataAnnotations;

namespace Nadasdladany.ViewModels
{
    public class CreateOfficeHourViewModel
    {
        [Required(ErrorMessage = "A nap kiválasztása kötelező.")]
        [Display(Name = "Nap")]
        public DayOfWeek DayOfWeek { get; set; }

        [Required(ErrorMessage = "Az időpont leírása kötelező.")]
        [StringLength(100, ErrorMessage = "A leírás maximum 100 karakter lehet.")]
        [Display(Name = "Időpont / Leírás")]
        public string TimeDescription { get; set; }

        [Display(Name = "Megjelenítési Sorrend")]
        [Range(0, 100, ErrorMessage = "A sorrend 0 és 100 között lehet.")]
        public int DisplayOrder { get; set; } = 99;
    }
}