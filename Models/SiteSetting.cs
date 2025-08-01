using System.ComponentModel.DataAnnotations;

namespace Nadasdladany.Models
{
    public class SiteSetting
    {
        [Key]
        [StringLength(100)]
        public string SettingKey { get; set; }

        [Required]
        public string SettingValue { get; set; }
    }
}
