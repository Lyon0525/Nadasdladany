using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nadasdladany.Models;
using Nadasdladany.ViewModels;

namespace Nadasdladany.Data
{
    public class NewsController : Controller
    {
        private readonly NadasdladanyDbContext _context;
        private readonly ILogger<NewsController> _logger;

        public NewsController(NadasdladanyDbContext context, ILogger<NewsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: News
        // GET: News/Category/{categorySlug}
        public async Task<IActionResult> Index(string? categorySlug = null, int page = 1)
        {
            int pageSize = 6; // Number of articles per page
            ViewData["Title"] = "Hírek";

            IQueryable<Article> articlesQuery = _context.Articles
                                                        .Where(a => a.IsPublished && a.PublishedDate <= DateTime.UtcNow)
                                                        .Include(a => a.Category)
                                                        .OrderByDescending(a => a.PublishedDate);

            string? currentCategoryName = null;

            if (!string.IsNullOrEmpty(categorySlug))
            {
                var category = await _context.Categories.FirstOrDefaultAsync(c => c.Slug == categorySlug);
                if (category != null)
                {
                    articlesQuery = articlesQuery.Where(a => a.CategoryId == category.Id);
                    currentCategoryName = category.Name;
                    ViewData["Title"] = $"Hírek - {category.Name}";
                }
                else
                {
                    // Category slug provided but not found, maybe show all news or a not found message
                    _logger.LogWarning("Category with slug '{CategorySlug}' not found.", categorySlug);
                    // Optionally, redirect to the main news page or return NotFound()
                    // For now, we'll just ignore the filter and show all news.
                }
            }

            var totalArticles = await articlesQuery.CountAsync();
            var articles = await articlesQuery
                                  .Skip((page - 1) * pageSize)
                                  .Take(pageSize)
                                  .ToListAsync();

            var allCategories = await _context.Categories
                                        .OrderBy(c => c.Name)
                                        .ToListAsync();

            var viewModel = new NewsIndexViewModel
            {
                Articles = articles,
                Categories = allCategories,
                CurrentCategorySlug = categorySlug,
                CurrentCategoryName = currentCategoryName,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling(totalArticles / (double)pageSize),
                PageSize = pageSize,
                TotalArticles = totalArticles
            };

            return View(viewModel);
        }

        // GET: News/Details/{slug}
        public async Task<IActionResult> Details(string slug)
        {
            if (string.IsNullOrEmpty(slug))
            {
                _logger.LogWarning("Details action called with null or empty slug.");
                return NotFound("A keresett cikk nem található (hiányzó azonosító).");
            }

            var article = await _context.Articles
                                .Include(a => a.Category)
                                .FirstOrDefaultAsync(a => a.Slug == slug && a.IsPublished);

            if (article == null)
            {
                _logger.LogInformation("Article with slug '{ArticleSlug}' not found or not published.", slug);
                return NotFound($"A '{slug}' azonosítójú cikk nem található vagy nem publikus.");
            }

            // Optional: Increment view count (simple version, not protected against multiple refreshes by same user in short time)
            // article.ViewCount++;
            // await _context.SaveChangesAsync();

            ViewData["Title"] = article.Title;
            return View(article);
        }
    }
}
