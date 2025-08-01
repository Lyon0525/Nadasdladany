using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // For ToListAsync, OrderBy, etc.
using Nadasdladany.Data;           // Your DbContext namespace
using Nadasdladany.Models;          // Your Model namespace
using Nadasdladany.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace Nadasdladany.Controllers // Your controller namespace
{
    public class InstitutionsController : Controller
    {
        private readonly NadasdladanyDbContext _context; // Inject NadasdladanyDbContext
        private readonly ILogger<InstitutionsController> _logger; // Assuming you'll add logger
        private readonly IWebHostEnvironment _webHostEnvironment;

        public InstitutionsController(NadasdladanyDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: /Institutions or /Institutions/Index
        public async Task<IActionResult> Index()
        {
            ViewData["Title"] = "Intézményeink";

            var institutions = await _context.Institutions
                                        .Where(i => i.IsPublished)
                                        .OrderBy(i => i.DisplayOrder)
                                        .ThenBy(i => i.Name)
                                        .ToListAsync();

            return View(institutions); // Pass the list of Institution entities to the view
        }

        // GET: Institutions/Details/{slug}
        public async Task<IActionResult> Details(string slug)
        {
            if (string.IsNullOrEmpty(slug))
            {
                _logger?.LogWarning("Details action called with null or empty slug for Institution.");
                return BadRequest("Az intézmény azonosítója érvénytelen.");
            }

            var institution = await _context.Institutions
                                      .FirstOrDefaultAsync(i => i.Slug == slug && i.IsPublished);

            if (institution == null)
            {
                _logger?.LogWarning("Institution with slug '{Slug}' not found or not published.", slug);
                TempData["ErrorMessage"] = "A keresett intézmény (" + slug + ") nem található vagy jelenleg nem publikus.";
                return RedirectToAction(nameof(Index)); // Redirect to the list if not found
            }

            ViewData["Title"] = institution.Name;
            return View("InstitutionDetail", institution); // Pass the Institution object to InstitutionDetail.cshtml
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateInstitutionViewModel model)
        {
            if (ModelState.IsValid)
            {
                string relativeImagePath = await ProcessUploadedFile(model.ImageFile);

                var institution = new Institution
                {
                    Name = model.Name,
                    Description = model.Description,
                    Address = model.Address,
                    PhoneNumber = model.PhoneNumber,
                    Email = model.Email,
                    WebsiteUrl = model.WebsiteUrl,
                    ImageUrl = relativeImagePath,
                    Slug = await GenerateUniqueSlug(model.Name)
                };

                _context.Institutions.Add(institution);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Intézmény sikeresen létrehozva!";
                return RedirectToAction(nameof(Index));
            }

            TempData["ErrorMessage"] = "Hiba történt a mentés során. Ellenőrizze a megadott adatokat.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditInstitutionViewModel model)
        {
            if (ModelState.IsValid)
            {
                var institutionToUpdate = await _context.Institutions.FindAsync(model.Id);
                if (institutionToUpdate == null)
                {
                    TempData["ErrorMessage"] = "A szerkesztendő intézmény nem található.";
                    return RedirectToAction(nameof(Index));
                }

                // Handle image update
                if (model.ImageFile != null)
                {
                    DeleteExistingImage(institutionToUpdate.ImageUrl);
                    institutionToUpdate.ImageUrl = await ProcessUploadedFile(model.ImageFile);
                }
                else if (model.RemoveCurrentImage)
                {
                    DeleteExistingImage(institutionToUpdate.ImageUrl);
                    institutionToUpdate.ImageUrl = null;
                }

                // Update properties
                institutionToUpdate.Name = model.Name;
                institutionToUpdate.Description = model.Description;
                institutionToUpdate.Address = model.Address;
                institutionToUpdate.PhoneNumber = model.PhoneNumber;
                institutionToUpdate.Email = model.Email;
                institutionToUpdate.WebsiteUrl = model.WebsiteUrl;

                // Regenerate slug if name has changed
                if (institutionToUpdate.Name != model.Name)
                {
                    institutionToUpdate.Slug = await GenerateUniqueSlug(model.Name, model.Id);
                }

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Intézmény adatai sikeresen frissítve!";
                return RedirectToAction(nameof(Index));
            }

            TempData["ErrorMessage"] = "Hiba történt a mentés során. Ellenőrizze a megadott adatokat.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var institutionToDelete = await _context.Institutions.FindAsync(id);
            if (institutionToDelete == null)
            {
                TempData["ErrorMessage"] = "A törlendő intézmény nem található.";
                return RedirectToAction(nameof(Index));
            }

            // Delete the associated image from the server
            DeleteExistingImage(institutionToDelete.ImageUrl);

            _context.Institutions.Remove(institutionToDelete);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"A(z) \"{institutionToDelete.Name}\" nevű intézmény sikeresen törölve lett.";

            return RedirectToAction(nameof(Index));
        }

        #region Helper Methods

        private async Task<string> ProcessUploadedFile(IFormFile file)
        {
            if (file == null || file.Length == 0) return null;

            string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "img", "institutions");
            Directory.CreateDirectory(uploadsFolder);
            string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(file.FileName);
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            return $"/img/institutions/{uniqueFileName}";
        }

        private void DeleteExistingImage(string relativePath)
        {
            if (string.IsNullOrEmpty(relativePath)) return;
            var fullPath = Path.Combine(_webHostEnvironment.WebRootPath, relativePath.TrimStart('/'));
            if (System.IO.File.Exists(fullPath))
            {
                System.IO.File.Delete(fullPath);
            }
        }

        private async Task<string> GenerateUniqueSlug(string phrase, int? currentId = null)
        {
            if (string.IsNullOrWhiteSpace(phrase)) return Guid.NewGuid().ToString();

            string str = phrase.ToLowerInvariant().Trim();
            str = str.Replace('á', 'a').Replace('é', 'e').Replace('í', 'i').Replace('ó', 'o').Replace('ö', 'o').Replace('ő', 'o').Replace('ú', 'u').Replace('ü', 'u').Replace('ű', 'u');
            str = Regex.Replace(str, @"[^a-z0-9\s-]", "");
            str = Regex.Replace(str, @"\s+", "-");
            str = Regex.Replace(str, @"-+", "-");

            var originalSlug = str;
            int i = 1;
            while (await _context.Institutions.AnyAsync(inst => inst.Slug == str && inst.Id != currentId))
            {
                str = $"{originalSlug}-{i++}";
            }
            return str;
        }

        #endregion
    }
}