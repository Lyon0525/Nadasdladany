using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // For ToListAsync, OrderBy, etc.
using Nadasdladany.Data;           // Your DbContext namespace
using Nadasdladany.Models;          // Your Model namespace
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nadasdladany.Controllers // Your controller namespace
{
    public class InstitutionsController : Controller
    {
        private readonly NadasdladanyDbContext _context; // Inject NadasdladanyDbContext
        private readonly ILogger<InstitutionsController> _logger; // Assuming you'll add logger

        public InstitutionsController(NadasdladanyDbContext context)
        {
            _context = context;
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
    }
}