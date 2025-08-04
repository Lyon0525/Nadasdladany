using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nadasdladany.Data;
using Nadasdladany.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nadasdladany.Controllers
{
    public class SearchController : Controller
    {
        private readonly NadasdladanyDbContext _context;

        public SearchController(NadasdladanyDbContext context)
        {
            _context = context;
        }

        // GET: /Search/Index?query=...
        public async Task<IActionResult> Index(string query)
        {
            ViewData["Title"] = $"Keresési eredmények: '{query}'";
            ViewBag.SearchQuery = query;

            var results = new List<SearchResultViewModel>();

            if (string.IsNullOrWhiteSpace(query))
            {
                // Ha üres a keresés, egy üres listát adunk vissza (vagy átirányíthatunk a főoldalra)
                return View(results);
            }

            // Keresés a hírekben (Articles)
            var newsResults = await _context.Articles
                .Where(a => a.IsPublished && (a.Title.Contains(query) || a.Content.Contains(query)))
                .Take(10) // Limitáljuk a találatok számát típusonként
                .Select(a => new SearchResultViewModel
                {
                    Title = a.Title,
                    Description = a.Excerpt ?? "Nincs leírás.", // Használjuk a kivonatot
                    Url = Url.Action("Details", "News", new { slug = a.Slug }),
                    ResultType = "Hír",
                    TypeIconCssClass = "bi-newspaper"
                }).ToListAsync();
            results.AddRange(newsResults);

            // Keresés az eseményekben (Events)
            var eventResults = await _context.Events
                .Where(e => e.IsPublished && (e.Title.Contains(query) || e.Description.Contains(query)))
                .Take(10)
                .Select(e => new SearchResultViewModel
                {
                    Title = e.Title,
                    Description = e.Description ?? "Nincs leírás.",
                    Url = Url.Action("Details", "Events", new { slug = e.Slug }),
                    ResultType = "Esemény",
                    TypeIconCssClass = "bi-calendar-event-fill"
                }).ToListAsync();
            results.AddRange(eventResults);

            // Keresés a dokumentumokban (Documents)
            var docResults = await _context.Documents
                .Where(d => d.IsPublished && (d.Title.Contains(query) || d.Description.Contains(query)))
                .Take(10)
                .Select(d => new SearchResultViewModel
                {
                    Title = d.Title,
                    Description = d.Description ?? "Nincs leírás.",
                    Url = Url.Action("Download", "Documents", new { id = d.Id }),
                    ResultType = "Dokumentum",
                    TypeIconCssClass = "bi-file-earmark-text-fill"
                }).ToListAsync();
            results.AddRange(docResults);

            return View(results);
        }
    }
}