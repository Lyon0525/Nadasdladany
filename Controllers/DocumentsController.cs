using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting; // For IWebHostEnvironment to get wwwroot path
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging; // <<--- ADDED THIS USING DIRECTIVE
using Nadasdladany.Data; // Your DbContext namespace
using Nadasdladany.Models; // Your Model namespace
using Nadasdladany.ViewModels;
using System.IO;                  // For Path.Combine, System.IO.File
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NadasdladanyWebApp.MVC.Controllers // Assuming this is your desired controller namespace
{
    public class DocumentsController : Controller
    {
        private readonly NadasdladanyDbContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly ILogger<DocumentsController> _logger; // Field for the logger

        // CORRECTED CONSTRUCTOR: Inject ILogger and assign it
        public DocumentsController(NadasdladanyDbContext context,
                                   IWebHostEnvironment hostingEnvironment,
                                   ILogger<DocumentsController> logger)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
            _logger = logger; // Assign the injected logger to the field
        }

        // GET: Documents or Documents/Index
        // string? categorySlug to filter by category
        // string? searchTerm for basic search
        public async Task<IActionResult> Index(string? categorySlug, string? searchTerm, int page = 1)
        {
            ViewData["Title"] = "Dokumentumtár";
            int pageSize = 15; // Documents per page

            IQueryable<Document> documentsQuery = _context.Documents
                                                    .Where(d => d.IsPublished)
                                                    .Include(d => d.DocumentCategory) // Eager load category
                                                    .OrderByDescending(d => d.UploadedDate);

            var currentCategory = await _context.DocumentCategories.FirstOrDefaultAsync(dc => dc.Slug == categorySlug);

            if (currentCategory != null)
            {
                documentsQuery = documentsQuery.Where(d => d.DocumentCategoryId == currentCategory.Id);
                ViewData["ListTitle"] = $"{currentCategory.Name} Dokumentumai";
                ViewData["CurrentCategoryName"] = currentCategory.Name;
            }
            else
            {
                ViewData["ListTitle"] = "Összes Dokumentum";
            }

            if (!string.IsNullOrEmpty(searchTerm))
            {
                documentsQuery = documentsQuery.Where(d => d.Title.Contains(searchTerm) ||
                                                       (d.Description != null && d.Description.Contains(searchTerm)));
                ViewBag.CurrentSearchTerm = searchTerm;
            }

            ViewBag.Categories = await _context.DocumentCategories.OrderBy(c => c.Name).ToListAsync();
            ViewBag.CurrentCategorySlug = categorySlug;

            var totalDocuments = await documentsQuery.CountAsync();
            var documents = await documentsQuery.Skip((page - 1) * pageSize)
                                             .Take(pageSize)
                                             .ToListAsync();

            ViewBag.TotalDocuments = totalDocuments;
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalDocuments / pageSize);
            if (ViewBag.TotalPages == 0 && totalDocuments > 0) ViewBag.TotalPages = 1;

            return View(documents);
        }

        // GET: Documents/Download/{id}
        public async Task<IActionResult> Download(int id)
        {
            var document = await _context.Documents
                                         .Include(d => d.DocumentCategory) // Include category for redirect context
                                         .FirstOrDefaultAsync(d => d.Id == id);

            if (document == null || !document.IsPublished)
            {
                _logger.LogWarning($"Attempt to download non-existent or unpublished document. ID: {id}");
                TempData["DownloadErrorMessage"] = "A keresett dokumentum nem található vagy nem publikus."; // Specific message for this
                return RedirectToAction(nameof(Index));
            }

            string webRootPath = _hostingEnvironment.WebRootPath;
            string relativeFilePath = document.FilePath.TrimStart('/', '\\');
            string filePath = Path.Combine(webRootPath, relativeFilePath);

            if (!System.IO.File.Exists(filePath))
            {
                _logger.LogError($"DOWNLOAD ERROR: File not found on server for document ID: {id}, Title: '{document.Title}'. Expected path: '{filePath}'. Stored FilePath: '{document.FilePath}'");
                // Set a TempData message that the Index view can display
                TempData["DownloadErrorMessage"] = $"A \"{document.Title}\" című dokumentum fájlja jelenleg nem elérhető a szerveren. Kérjük, értesítse az adminisztrátort.";
                return RedirectToAction(nameof(Index), new { categorySlug = document.DocumentCategory?.Slug }); // Redirect back to index, potentially in the same category
            }

            var memory = new MemoryStream();
            try
            {
                using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    await stream.CopyToAsync(memory);
                }
                memory.Position = 0;

                string contentType = "application/octet-stream"; // Default
                if (!string.IsNullOrEmpty(document.FileType))
                {
                    switch (document.FileType.ToUpperInvariant())
                    {
                        case "PDF": contentType = "application/pdf"; break;
                        case "DOC": contentType = "application/msword"; break;
                        case "DOCX": contentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document"; break;
                        case "XLS": contentType = "application/vnd.ms-excel"; break;
                        case "XLSX": contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"; break;
                        case "PPT": contentType = "application/vnd.ms-powerpoint"; break;
                        case "PPTX": contentType = "application/vnd.openxmlformats-officedocument.presentationml.presentation"; break;
                        case "TXT": contentType = "text/plain"; break;
                        case "JPG": case "JPEG": contentType = "image/jpeg"; break;
                        case "PNG": contentType = "image/png"; break;
                    }
                }

                string downloadFileName = Path.GetFileName(document.FilePath);
                _logger.LogInformation($"Document downloaded: ID: {id}, Title: '{document.Title}', File: '{downloadFileName}'");
                return File(memory, contentType, downloadFileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error during file download for document ID: {id}, Title: '{document.Title}'. Path: '{filePath}'");
                TempData["DownloadErrorMessage"] = $"Hiba történt a \"{document.Title}\" című dokumentum letöltése közben. Kérjük, próbálja újra később, vagy értesítse az adminisztrátort.";
                return RedirectToAction(nameof(Index), new { categorySlug = document.DocumentCategory?.Slug });
            }
        }

        [HttpGet]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> ManageDocumentCategories()
        {
            ViewData["Title"] = "Dokumentum Kategóriák Kezelése";
            var categories = await _context.DocumentCategories.OrderBy(c => c.Name).ToListAsync();
            return View(categories);
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCategory(CreateDocumentCategoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                var category = new DocumentCategory
                {
                    Name = model.Name,
                    Description = model.Description,
                    Slug = await GenerateUniqueCategorySlug(model.Name)
                };

                _context.DocumentCategories.Add(category);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Az új kategória sikeresen létrehozva.";
                return RedirectToAction(nameof(ManageDocumentCategories));
            }
            TempData["ErrorMessage"] = "Hiba történt a létrehozás során. A név megadása kötelező.";
            return RedirectToAction(nameof(ManageDocumentCategories));
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCategory(EditDocumentCategoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                var categoryToUpdate = await _context.DocumentCategories.FindAsync(model.Id);
                if (categoryToUpdate == null)
                {
                    TempData["ErrorMessage"] = "A szerkesztendő kategória nem található.";
                    return RedirectToAction(nameof(ManageDocumentCategories));
                }

                if (categoryToUpdate.Name != model.Name)
                {
                    categoryToUpdate.Slug = await GenerateUniqueCategorySlug(model.Name, model.Id);
                }

                categoryToUpdate.Name = model.Name;
                categoryToUpdate.Description = model.Description;

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "A kategória adatai sikeresen frissítve.";
                return RedirectToAction(nameof(ManageDocumentCategories));
            }
            TempData["ErrorMessage"] = "Érvénytelen adatok.";
            return RedirectToAction(nameof(ManageDocumentCategories));
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var categoryToDelete = await _context.DocumentCategories
                                               .Include(c => c.Documents)
                                               .FirstOrDefaultAsync(c => c.Id == id);
            if (categoryToDelete == null)
            {
                TempData["ErrorMessage"] = "A törlendő kategória nem található.";
                return RedirectToAction(nameof(ManageDocumentCategories));
            }

            // SAFETY CHECK: Prevent deleting a category that contains documents.
            if (categoryToDelete.Documents.Any())
            {
                TempData["ErrorMessage"] = $"A(z) \"{categoryToDelete.Name}\" kategória nem törölhető, mert dokumentumokat tartalmaz. Kérjük, előbb helyezze át vagy törölje a dokumentumokat.";
                return RedirectToAction(nameof(ManageDocumentCategories));
            }

            _context.DocumentCategories.Remove(categoryToDelete);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "A kategória sikeresen törölve lett.";

            return RedirectToAction(nameof(ManageDocumentCategories));
        }

        #region Helper Methods

        private async Task<string> GenerateUniqueCategorySlug(string phrase, int? currentId = null)
        {
            if (string.IsNullOrWhiteSpace(phrase)) return Guid.NewGuid().ToString();

            string str = phrase.ToLowerInvariant().Trim();
            // ... (Your standard slug generation logic for Hungarian characters) ...
            str = str.Replace('á', 'a').Replace('é', 'e').Replace('í', 'i').Replace('ó', 'o').Replace('ö', 'o').Replace('ő', 'o').Replace('ú', 'u').Replace('ü', 'u').Replace('ű', 'u');
            str = Regex.Replace(str, @"[^a-z0-9\s-]", "");
            str = Regex.Replace(str, @"\s+", "-");
            str = Regex.Replace(str, @"-+", "-");

            var originalSlug = str;
            int i = 1;
            // Check for uniqueness against the DocumentCategories table
            while (await _context.DocumentCategories.AnyAsync(c => c.Slug == str && c.Id != currentId))
            {
                str = $"{originalSlug}-{i++}";
            }
            return str;
        }

        #endregion
    }
}