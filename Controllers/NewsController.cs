using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nadasdladany.Data;
using Nadasdladany.Models; 
using Nadasdladany.ViewModels;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Nadasdladany.Controllers
{
    public class NewsController : Controller
    {
        // Your DbContext, plus Logger and the new UserManager
        private readonly NadasdladanyDbContext _context;
        private readonly ILogger<NewsController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;

        // UPDATED CONSTRUCTOR: Added UserManager for handling user-related actions
        public NewsController(
            NadasdladanyDbContext context,
            ILogger<NewsController> logger,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _logger = logger;
            _userManager = userManager;
        }

        // GET: News
        // GET: News/Category/{categorySlug}
        // THIS ACTION REMAINS THE SAME. It provides the data for the view, including categories for the modal.
        public async Task<IActionResult> Index(string? categorySlug = null, int page = 1)
        {
            int pageSize = 6;
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
                    _logger.LogWarning("Category with slug '{CategorySlug}' not found.", categorySlug);
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
        // THIS ACTION REMAINS THE SAME.
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

            ViewData["Title"] = article.Title;
            return View(article);
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        [ValidateAntiForgeryToken]
        // KEY CHANGE #2: The action now accepts the ViewModel, not the database model
        public async Task<IActionResult> Create(CreateArticleViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);

                // Manually create the Article object from the valid ViewModel
                var newArticle = new Article
                {
                    Title = model.Title,
                    Content = model.Content,
                    Excerpt = model.Excerpt,
                    FeaturedImageUrl = model.FeaturedImageUrl,
                    CategoryId = model.CategoryId,
                    Slug = GenerateSlug(model.Title), // Generate slug here
                    Author = user?.UserName ?? "Adminisztrátor",
                    IsPublished = true, // Default to published
                    PublishedDate = DateTime.UtcNow,
                    LastModifiedDate = DateTime.UtcNow
                };

                // Auto-generate excerpt if it was left blank
                if (string.IsNullOrWhiteSpace(newArticle.Excerpt) && !string.IsNullOrWhiteSpace(newArticle.Content))
                {
                    var plainText = Regex.Replace(newArticle.Content, "<.*?>", string.Empty);
                    newArticle.Excerpt = plainText.Length <= 200 ? plainText : plainText.Substring(0, 200) + "...";
                }

                _context.Articles.Add(newArticle);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Hír sikeresen hozzáadva!";
                return RedirectToAction(nameof(Index));
            }

            // If we get here, something failed. Let's provide a more detailed error.
            // This builds a string of all validation errors.
            var errorList = ModelState.Values
                                .SelectMany(v => v.Errors)
                                .Select(e => e.ErrorMessage)
                                .ToList();
            TempData["ErrorMessage"] = "Hiba történt a mentés során: " + string.Join(" ", errorList);

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditArticleViewModel model)
        {
            if (ModelState.IsValid)
            {
                // 1. Find the existing article in the database
                var articleToUpdate = await _context.Articles.FindAsync(model.Id);

                if (articleToUpdate == null)
                {
                    TempData["ErrorMessage"] = "A szerkesztendő hír nem található.";
                    return RedirectToAction(nameof(Index));
                }

                // 2. Update its properties with the data from the form
                articleToUpdate.Title = model.Title;
                articleToUpdate.Content = model.Content;
                articleToUpdate.Excerpt = model.Excerpt;
                articleToUpdate.FeaturedImageUrl = model.FeaturedImageUrl;
                articleToUpdate.CategoryId = model.CategoryId;
                articleToUpdate.LastModifiedDate = DateTime.UtcNow; // Update modification date

                // Important: We generally do not change the slug or author on an edit to preserve links.
                // Regenerate excerpt if it was cleared
                if (string.IsNullOrWhiteSpace(articleToUpdate.Excerpt) && !string.IsNullOrWhiteSpace(articleToUpdate.Content))
                {
                    var plainText = Regex.Replace(articleToUpdate.Content, "<.*?>", string.Empty);
                    articleToUpdate.Excerpt = plainText.Length <= 200 ? plainText : plainText.Substring(0, 200) + "...";
                }

                try
                {
                    // 3. Save the changes
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Hír sikeresen frissítve!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    TempData["ErrorMessage"] = "Hiba történt a frissítés során. Kérjük, próbálja újra.";
                }

                return RedirectToAction(nameof(Index));
            }

            // If model state is invalid, show an error
            TempData["ErrorMessage"] = "Hiba történt a mentés során. Ellenőrizze a megadott adatokat.";
            return RedirectToAction(nameof(Index));
        }

        // --- NEW ACTION FOR DELETING AN ARTICLE ---
        [HttpPost]
        [Authorize(Roles = "Administrator")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            // 1. Find the article to delete
            var articleToDelete = await _context.Articles.FindAsync(id);

            if (articleToDelete == null)
            {
                TempData["ErrorMessage"] = "A törlendő hír nem található, vagy már törölve lett.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                // 2. Remove it and save changes
                _context.Articles.Remove(articleToDelete);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"A(z) \"{articleToDelete.Title}\" című hír sikeresen törölve lett.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting article with ID {ArticleId}", id);
                TempData["ErrorMessage"] = "Hiba történt a törlés során. Kérjük, próbálja újra.";
            }

            return RedirectToAction(nameof(Index));
        }

        // --- HELPER METHOD TO GENERATE A URL-FRIENDLY SLUG ---
        private string GenerateSlug(string phrase)
        {
            if (string.IsNullOrEmpty(phrase))
                return string.Empty;

            string str = phrase.ToLowerInvariant();

            // Basic replacements for Hungarian characters
            str = str.Replace('á', 'a').Replace('é', 'e').Replace('í', 'i').Replace('ó', 'o').Replace('ö', 'o').Replace('ő', 'o').Replace('ú', 'u').Replace('ü', 'u').Replace('ű', 'u');

            // Remove invalid characters
            str = Regex.Replace(str, @"[^a-z0-9\s-]", "");
            // Convert multiple spaces into one space
            str = Regex.Replace(str, @"\s+", " ").Trim();
            // Replace spaces with hyphens
            str = Regex.Replace(str, @"\s", "-");

            // Prevent multiple consecutive hyphens
            str = Regex.Replace(str, @"-+", "-");

            return str;
        }
    }
}