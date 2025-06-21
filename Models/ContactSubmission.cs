// File: Nadasdladany/Models/ContactSubmission.cs (or your models folder)
using System;
using System.ComponentModel.DataAnnotations;

namespace Nadasdladany.Models // Using your established model namespace
{
    public class ContactSubmission
    {
        public int Id { get; set; } // Primary Key

        [Required(ErrorMessage = "Kérjük, adja meg a nevét.")]
        [StringLength(100)]
        [Display(Name = "Név")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Kérjük, adja meg az e-mail címét.")]
        [EmailAddress(ErrorMessage = "Érvénytelen e-mail cím formátum.")]
        [StringLength(100)]
        [Display(Name = "E-mail cím")]
        public string Email { get; set; }

        [StringLength(150)] // Increased from 100 for consistency with Event Title
        [Display(Name = "Tárgy")]
        public string? Subject { get; set; } // Optional subject

        [Required(ErrorMessage = "Kérjük, írja le az üzenetét.")]
        [StringLength(2000, ErrorMessage = "Az üzenet nem haladhatja meg a 2000 karaktert.")]
        [Display(Name = "Üzenet")]
        public string Message { get; set; }

        [Display(Name = "Küldés Dátuma")]
        public DateTime SubmittedDate { get; set; } = DateTime.UtcNow;

        [Display(Name = "Olvasva")]
        public bool IsRead { get; set; } = false; // For admin tracking

        [StringLength(500)]
        [Display(Name = "Adminisztrátori Jegyzetek")]
        public string? AdminNotes { get; set; } // For internal notes by admin
    }
}