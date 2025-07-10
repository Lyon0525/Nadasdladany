using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // Required for EF Core
using Nadasdladany.Data;         // Your DbContext namespace
using Nadasdladany.Models;        // Your Model namespace
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NadasdladanyWebApp.MVC.Controllers // Your controller namespace
{
    public class GalleryController : Controller
    {
        private readonly NadasdladanyDbContext _context; // Inject DbContext
        private readonly ILogger<GalleryController> _logger;

        public GalleryController(NadasdladanyDbContext context, ILogger<GalleryController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Gallery or Gallery/Index
        // string? albumSlug to filter by album
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

        // No Details action for individual images implemented here yet,
        // the Index view uses a modal for image preview.
        // If you want separate detail pages for images, you would add a Details(int id) action.
    }
}