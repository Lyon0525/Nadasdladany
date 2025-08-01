using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // Required for EF Core
using Nadasdladany.Data;         // Your DbContext namespace
using Nadasdladany.Models;        // Your Model namespace
using Nadasdladany.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NadasdladanyWebApp.MVC.Controllers // Your controller namespace
{
    [Authorize(Roles = "Administrator")]
    public class GalleryController : Controller
    {
        private readonly NadasdladanyDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ILogger<GalleryController> _logger;


        public GalleryController(NadasdladanyDbContext context, IWebHostEnvironment webHostEnvironment, ILogger<GalleryController> logger)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _logger = logger;
        }


        [AllowAnonymous]
        public async Task<IActionResult> Index(string? albumSlug)
        {
            ViewData["Title"] = "Galéria";

            IQueryable<GalleryImage> imagesQuery = _context.GalleryImages
                                                      .Where(img => img.IsPublished)
                                                      .Include(img => img.GalleryAlbum) // Eager load album info
                                                      .OrderBy(img => img.GalleryAlbumId) // Group by album visually
                                                      .ThenBy(img => img.DisplayOrder)
                                                      .ThenByDescending(img => img.UploadedDate);

            GalleryAlbum? currentAlbum = null;
            if (!string.IsNullOrEmpty(albumSlug))
            {
                currentAlbum = await _context.GalleryAlbums
                                        .FirstOrDefaultAsync(a => a.Slug == albumSlug && a.IsPublished);
                if (currentAlbum != null)
                {
                    imagesQuery = imagesQuery.Where(img => img.GalleryAlbumId == currentAlbum.Id);
                    ViewData["Title"] = $"{currentAlbum.Title} - Galéria";
                    ViewData["CurrentAlbumDescription"] = currentAlbum.Description;
                }
                else
                {
                    _logger.LogWarning("Gallery album with slug '{AlbumSlug}' not found or not published.", albumSlug);
                    TempData["ErrorMessage"] = "A keresett album nem található.";
                    // Optionally, show all images if album not found, or show an empty list
                    // imagesQuery = Enumerable.Empty<GalleryImage>().AsQueryable(); // To show no images
                }
            }

            ViewBag.Albums = await _context.GalleryAlbums
                                    .Where(a => a.IsPublished)
                                    .OrderBy(a => a.DisplayOrder)
                                    .ThenBy(a => a.Title)
                                    .ToListAsync();
            ViewBag.CurrentAlbumSlug = albumSlug;
            ViewBag.CurrentAlbum = currentAlbum;


            var imagesToDisplay = await imagesQuery.ToListAsync();

            // If you want to map to a ViewModel specifically for the view:
            // var viewModels = imagesToDisplay.Select(img => new GalleryImageViewModel { ... map properties ... }).ToList();
            // return View(viewModels);
            // For now, we'll pass GalleryImage entities directly, and the view uses that.

            return View(imagesToDisplay);
        }

        [HttpGet]
        public async Task<IActionResult> ManageAlbums()
        {
            ViewData["Title"] = "Albumok Kezelése";
            var albums = await _context.GalleryAlbums
                                    .OrderBy(a => a.DisplayOrder)
                                    .ThenBy(a => a.Title)
                                    .ToListAsync();
            return View(albums);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAlbum(CreateAlbumViewModel model)
        {
            if (ModelState.IsValid)
            {
                var album = new GalleryAlbum
                {
                    Title = model.Title,
                    Description = model.Description,
                    DisplayOrder = model.DisplayOrder,
                    Slug = await GenerateUniqueAlbumSlug(model.Title),
                    IsPublished = true
                };
                _context.GalleryAlbums.Add(album);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Az album sikeresen létrehozva.";
                return RedirectToAction(nameof(ManageAlbums));
            }
            TempData["ErrorMessage"] = "Hiba történt a létrehozás során.";
            return RedirectToAction(nameof(ManageAlbums));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAlbum(EditAlbumViewModel model)
        {
            if (ModelState.IsValid)
            {
                var album = await _context.GalleryAlbums.FindAsync(model.Id);
                if (album == null)
                {
                    TempData["ErrorMessage"] = "A szerkesztendő album nem található.";
                    return RedirectToAction(nameof(ManageAlbums));
                }

                // Regenerate slug only if the title has changed
                if (album.Title != model.Title)
                {
                    album.Slug = await GenerateUniqueAlbumSlug(model.Title, model.Id);
                }

                album.Title = model.Title;
                album.Description = model.Description;
                album.DisplayOrder = model.DisplayOrder;

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Az album adatai sikeresen frissítve.";
                return RedirectToAction(nameof(ManageAlbums));
            }
            TempData["ErrorMessage"] = "Érvénytelen adatok.";
            return RedirectToAction(nameof(ManageAlbums));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAlbum(int id)
        {
            // IMPORTANT: We include the related Images to check if the album is empty.
            var album = await _context.GalleryAlbums
                                  .Include(a => a.Images)
                                  .FirstOrDefaultAsync(a => a.Id == id);

            if (album == null)
            {
                TempData["ErrorMessage"] = "A törlendő album nem található.";
                return RedirectToAction(nameof(ManageAlbums));
            }

            // SAFETY CHECK: Prevent deletion of an album that contains images.
            if (album.Images.Any())
            {
                TempData["ErrorMessage"] = $"A(z) \"{album.Title}\" album nem törölhető, mert képeket tartalmaz. Kérjük, előbb helyezze át vagy törölje a képeket.";
                return RedirectToAction(nameof(ManageAlbums));
            }

            _context.GalleryAlbums.Remove(album);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Az album sikeresen törölve lett.";
            return RedirectToAction(nameof(ManageAlbums));
        }


        [HttpPost]
        [Authorize(Roles = "Administrator")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateImage(CreateGalleryImageViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Process the uploaded file
                string imagePath = await ProcessUploadedFile(model.ImageFile);

                // For simplicity, we won't generate a separate thumbnail in this version.
                // In a production system, you might use a library like SixLabors.ImageSharp to create a smaller thumbnail.
                string thumbnailUrl = imagePath;

                if (imagePath == null)
                {
                    TempData["ErrorMessage"] = "Hiba történt a kép feltöltése közben.";
                    return RedirectToAction(nameof(Index));
                }

                var galleryImage = new GalleryImage
                {
                    Title = model.Title,
                    Description = model.Description,
                    AltText = model.AltText,
                    ImageUrl = imagePath,
                    ThumbnailUrl = thumbnailUrl,
                    GalleryAlbumId = model.GalleryAlbumId,
                    UploadedDate = DateTime.UtcNow,
                    IsPublished = true
                };

                _context.GalleryImages.Add(galleryImage);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "A kép sikeresen feltöltve a galériába!";
                return RedirectToAction(nameof(Index));
            }

            TempData["ErrorMessage"] = "Érvénytelen adatok. A kép és az album kiválasztása kötelező.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditImage(EditGalleryImageViewModel model)
        {
            if (ModelState.IsValid)
            {
                var imageToUpdate = await _context.GalleryImages.FindAsync(model.Id);
                if (imageToUpdate == null)
                {
                    TempData["ErrorMessage"] = "A szerkesztendő kép nem található.";
                    return RedirectToAction(nameof(Index));
                }

                imageToUpdate.Title = model.Title;
                imageToUpdate.Description = model.Description;
                imageToUpdate.AltText = model.AltText;
                imageToUpdate.GalleryAlbumId = model.GalleryAlbumId;

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "A kép adatai sikeresen frissítve!";
                return RedirectToAction(nameof(Index));
            }

            TempData["ErrorMessage"] = "Érvénytelen adatok. Kérjük, ellenőrizze a beviteli mezőket.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteImage(int id)
        {
            var imageToDelete = await _context.GalleryImages.FindAsync(id);
            if (imageToDelete == null)
            {
                TempData["ErrorMessage"] = "A törlendő kép nem található.";
                return RedirectToAction(nameof(Index));
            }

            // Delete the physical file from the server
            DeleteExistingImage(imageToDelete.ImageUrl);
            if (imageToDelete.ImageUrl != imageToDelete.ThumbnailUrl) // In case you add separate thumbnails later
            {
                DeleteExistingImage(imageToDelete.ThumbnailUrl);
            }

            _context.GalleryImages.Remove(imageToDelete);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "A kép sikeresen törölve lett.";
            return RedirectToAction(nameof(Index));

        }

        #region Helper Methods

        private async Task<string> ProcessUploadedFile(IFormFile file)
        {
            if (file == null || file.Length == 0) return null;

            string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "img", "gallery");
            Directory.CreateDirectory(uploadsFolder);
            string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(file.FileName);
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            return $"/img/gallery/{uniqueFileName}";
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

        private async Task<string> GenerateUniqueAlbumSlug(string phrase, int? currentId = null)
        {
            // Fallback if the title is empty for some reason
            if (string.IsNullOrWhiteSpace(phrase))
                return Guid.NewGuid().ToString();

            // STEP 1: Create a basic, clean slug from the title
            string str = phrase.ToLowerInvariant().Trim();
            // Replace Hungarian accented characters with their simple counterparts
            str = str.Replace('á', 'a').Replace('é', 'e').Replace('í', 'i').Replace('ó', 'o').Replace('ö', 'o').Replace('ő', 'o').Replace('ú', 'u').Replace('ü', 'u').Replace('ű', 'u');
            // Remove any character that is not a lowercase letter, a number, a space, or a hyphen
            str = Regex.Replace(str, @"[^a-z0-9\s-]", "");
            // Replace one or more spaces with a single hyphen
            str = Regex.Replace(str, @"\s+", "-");
            // Replace multiple hyphens with a single one (e.g., "title---slug" becomes "title-slug")
            str = Regex.Replace(str, @"-+", "-");

            // STEP 2: Check if this slug already exists in the database
            var originalSlug = str;
            int i = 1;

            // This is the core logic: "Keep looping as long as you find any OTHER album that already has this slug"
            while (await _context.GalleryAlbums.AnyAsync(a => a.Slug == str && a.Id != currentId))
            {
                // If a duplicate is found, append a number and check again
                str = $"{originalSlug}-{i++}";
            }

            // STEP 3: Return the final, guaranteed-to-be-unique slug
            return str;
        }

        #endregion
    }
}