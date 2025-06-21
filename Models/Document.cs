using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; // For NotMapped if you add helper properties

namespace Nadasdladany.Models // Your model namespace
{
    public class Document
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "A dokumentum címe kötelező.")]
        [StringLength(255, ErrorMessage = "A cím maximum 255 karakter lehet.")]
        [Display(Name = "Dokumentum Címe")]
        public string Title { get; set; }

        [Display(Name = "Rövid Leírás")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "A fájl elérési útja kötelező.")]
        [StringLength(500)] // Store a relative path from a base documents folder
        [Display(Name = "Fájl Elérési Útja")]
        public string FilePath { get; set; } // e.g., "rendeletek/2024/2024-01-rendelet.pdf"

        [StringLength(100)]
        [Display(Name = "Fájl Típusa (pl. PDF, DOCX)")]
        public string? FileType { get; set; } // Can be inferred from FilePath extension or stored

        [Display(Name = "Fájl Mérete (byte)")]
        public long? FileSizeInBytes { get; set; }

        [Display(Name = "Feltöltés Dátuma")]
        public DateTime UploadedDate { get; set; } = DateTime.UtcNow;

        [Display(Name = "Publikus")]
        public bool IsPublished { get; set; } = true;

        [Display(Name = "Utolsó Módosítás Dátuma")]
        public DateTime? LastModifiedDate { get; set; }


        // Foreign Key
        [Required(ErrorMessage = "Kategória megadása kötelező.")]
        [Display(Name = "Dokumentum Kategória")]
        public int DocumentCategoryId { get; set; }

        // Navigation Property
        public DocumentCategory DocumentCategory { get; set; }
    }
}