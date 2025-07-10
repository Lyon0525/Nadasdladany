using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nadasdladany.Data;    // Your DbContext namespace
using Nadasdladany.Models; // Your Model namespace
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic; // For List<string>

namespace NadasdladanyWebApp.MVC.Controllers // Your controller namespace
{
    public class LinksController : Controller
    {
        private readonly NadasdladanyDbContext _context;
        private readonly ILogger<LinksController> _logger;

        public LinksController(NadasdladanyDbContext context, ILogger<LinksController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: /Links/PublicServices
        public async Task<IActionResult> PublicServices(string? category)
        {
            ViewData["Title"] = "Hasznos Linkek";
            if (!string.IsNullOrEmpty(category))
            {
                ViewData["Title"] = $"{category} - Hasznos Linkek";
            }

            IQueryable<UsefulLink> linksQuery = _context.UsefulLinks
                                                    .Where(l => l.IsPublished)
                                                    .OrderBy(l => l.Category)
                                                    .ThenBy(l => l.DisplayOrder)
                                                    .ThenBy(l => l.Title);

            if (!string.IsNullOrEmpty(category))
            {
                linksQuery = linksQuery.Where(l => l.Category == category);
            }

            var links = await linksQuery.ToListAsync();

            // Get distinct categories for filter buttons or headings
            ViewBag.LinkCategories = await _context.UsefulLinks
                                            .Where(l => l.IsPublished && !string.IsNullOrEmpty(l.Category))
                                            .Select(l => l.Category)
                                            .Distinct()
                                            .OrderBy(c => c)
                                            .ToListAsync();
            ViewBag.CurrentLinkCategory = category;

            // Group links by category for display if no specific category is selected
            if (string.IsNullOrEmpty(category))
            {
                var groupedLinks = links
                    .GroupBy(l => l.Category ?? "Egyéb Kategória") // Group uncategorized links
                    .OrderBy(g => g.Key)
                    .ToDictionary(g => g.Key, g => g.ToList());
                return View("PublicServicesGrouped", groupedLinks); // Use a different view for grouped display
            }

            return View(links); // Pass the filtered or all links to Views/Links/PublicServices.cshtml
        }
    }
}