using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nadasdladany.Data;
using Nadasdladany.Models;
using Nadasdladany.ViewModels; // <-- ADDED
using System.Linq;
using System.Threading.Tasks;

namespace Nadasdladany.Controllers // <-- Adjusted namespace to your project's standard
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
        [HttpGet]
        public async Task<IActionResult> PublicServices(string? category)
        {
            ViewData["Title"] = "Hasznos Linkek";
            if (!string.IsNullOrEmpty(category))
            {
                ViewData["Title"] = $"{category} - Hasznos Linkek";
            }

            // Prepare the ViewModel that will hold all our data
            var viewModel = new PublicServicesViewModel
            {
                CurrentCategory = category,
                AllCategories = await _context.UsefulLinks
                                    .Where(l => l.IsPublished && !string.IsNullOrEmpty(l.Category))
                                    .Select(l => l.Category)
                                    .Distinct()
                                    .OrderBy(c => c)
                                    .ToListAsync()
            };

            // Base query for all links
            IQueryable<UsefulLink> linksQuery = _context.UsefulLinks
                                                    .Where(l => l.IsPublished)
                                                    .OrderBy(l => l.Category)
                                                    .ThenBy(l => l.DisplayOrder)
                                                    .ThenBy(l => l.Title);

            if (!string.IsNullOrEmpty(category))
            {
                // ---- CASE 1: A CATEGORY IS SELECTED ----
                // Filter the query and populate the FilteredLinks list in the ViewModel
                viewModel.FilteredLinks = await linksQuery.Where(l => l.Category == category).ToListAsync();
            }
            else
            {
                // ---- CASE 2: NO CATEGORY IS SELECTED (SHOW ALL, GROUPED) ----
                var allLinks = await linksQuery.ToListAsync();
                // Group the links and populate the GroupedLinks dictionary in the ViewModel
                viewModel.GroupedLinks = allLinks
                    .GroupBy(l => l.Category ?? "Egyéb Kategória")
                    .OrderBy(g => g.Key == "Egyéb Kategória" ? "zzz" : g.Key)
                    .ToDictionary(g => g.Key, g => g.ToList());
            }

            // ALWAYS return the same view, passing our comprehensive ViewModel
            return View(viewModel);
        }

        // --- ADMIN ACTIONS ---

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateUsefulLinkViewModel model)
        {
            if (!ModelState.ContainsKey(nameof(model.OpenInNewTab)))
            {
                model.OpenInNewTab = false;
            }

            if (ModelState.IsValid)
            {
                var link = new UsefulLink
                {
                    Title = model.Title,
                    Url = model.Url,
                    Description = model.Description,
                    Category = model.Category,
                    OpenInNewTab = model.OpenInNewTab,
                    IsPublished = true
                };

                _context.UsefulLinks.Add(link);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Link sikeresen hozzáadva!";
                return RedirectToAction(nameof(PublicServices), new { category = model.Category });
            }

            TempData["ErrorMessage"] = "Hiba történt a mentés során. Ellenőrizze a megadott adatokat.";
            return RedirectToAction(nameof(PublicServices));
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditUsefulLinkViewModel model)
        {
            if (!ModelState.ContainsKey(nameof(model.OpenInNewTab)))
            {
                model.OpenInNewTab = false;
            }

            if (ModelState.IsValid)
            {
                var linkToUpdate = await _context.UsefulLinks.FindAsync(model.Id);
                if (linkToUpdate == null)
                {
                    TempData["ErrorMessage"] = "A szerkesztendő link nem található.";
                    return RedirectToAction(nameof(PublicServices));
                }

                linkToUpdate.Title = model.Title;
                linkToUpdate.Url = model.Url;
                linkToUpdate.Description = model.Description;
                linkToUpdate.Category = model.Category;
                linkToUpdate.OpenInNewTab = model.OpenInNewTab;

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Link sikeresen frissítve!";
                return RedirectToAction(nameof(PublicServices), new { category = model.Category });
            }

            TempData["ErrorMessage"] = "Hiba történt a mentés során. Ellenőrizze a megadott adatokat.";
            return RedirectToAction(nameof(PublicServices));
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var linkToDelete = await _context.UsefulLinks.FindAsync(id);
            if (linkToDelete == null)
            {
                TempData["ErrorMessage"] = "A törlendő link nem található.";
                return RedirectToAction(nameof(PublicServices));
            }

            _context.UsefulLinks.Remove(linkToDelete);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"A(z) \"{linkToDelete.Title}\" című link sikeresen törölve lett.";

            return RedirectToAction(nameof(PublicServices));
        }
    }
}