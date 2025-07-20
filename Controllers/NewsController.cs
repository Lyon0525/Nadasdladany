using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nadasdladany.Data;
using Nadasdladany.Models;
using Nadasdladany.ViewModels;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace Nadasdladany.Controllers
{
    public class NewsController : Controller
    {
        private readonly NadasdladanyDbContext _context;
        private readonly ILogger<NewsController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment; // ADDED: To get wwwroot path

        // UPDATED CONSTRUCTOR to include IWebHostEnvironment
        public NewsController(
            NadasdladanyDbContext context,
            ILogger<NewsController> logger,
            UserManager<ApplicationUser> userManager,
            IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _logger = logger;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
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
        public async Task<IActionResult> Create(CreateArticleViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Process the uploaded file and get its relative path for the DB
                string relativeImagePath = await ProcessUploadedFile(model.FeaturedImageFile);
                var user = await _userManager.GetUserAsync(User);

                var newArticle = new Article
                {
                    Title = model.Title,
                    Content = model.Content,
                    Excerpt = model.Excerpt,
                    FeaturedImageUrl = relativeImagePath, // Store the generated path
                    CategoryId = model.CategoryId,
                    Slug = await GenerateUniqueSlug(model.Title),
                    Author = user?.UserName ?? "Adminisztrátor",
                    IsPublished = true,
                    PublishedDate = DateTime.UtcNow,
                    LastModifiedDate = DateTime.UtcNow
                };

                if (string.IsNullOrWhiteSpace(newArticle.Excerpt))
                {
                    newArticle.Excerpt = CreateExcerptFromHtml(newArticle.Content);
                }

                _context.Articles.Add(newArticle);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Hír sikeresen hozzáadva!";
                return RedirectToAction(nameof(Index));
            }

            var errorList = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
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
                var articleToUpdate = await _context.Articles.FindAsync(model.Id);
                if (articleToUpdate == null)
                {
                    TempData["ErrorMessage"] = "A szerkesztendő hír nem található.";
                    return RedirectToAction(nameof(Index));
                }

                // Handle image update logic
                if (model.FeaturedImageFile != null)
                {
                    // A new file was uploaded, so delete the old one and save the new one
                    DeleteExistingImage(articleToUpdate.FeaturedImageUrl);
                    articleToUpdate.FeaturedImageUrl = await ProcessUploadedFile(model.FeaturedImageFile);
                }
                else if (model.RemoveCurrentImage)
                {
                    // The "remove" checkbox was ticked, so delete the old image and clear the DB field
                    DeleteExistingImage(articleToUpdate.FeaturedImageUrl);
                    articleToUpdate.FeaturedImageUrl = null;
                }
                // If neither of the above is true, the existing image is kept.

                articleToUpdate.Title = model.Title;
                articleToUpdate.Content = model.Content;
                articleToUpdate.Excerpt = model.Excerpt;
                articleToUpdate.CategoryId = model.CategoryId;
                articleToUpdate.LastModifiedDate = DateTime.UtcNow;

                if (string.IsNullOrWhiteSpace(articleToUpdate.Excerpt))
                {
                    articleToUpdate.Excerpt = CreateExcerptFromHtml(articleToUpdate.Content);
                }

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Hír sikeresen frissítve!";
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
            var articleToDelete = await _context.Articles.FindAsync(id);
            if (articleToDelete == null)
            {
                TempData["ErrorMessage"] = "A törlendő hír nem található.";
                return RedirectToAction(nameof(Index));

            }

            // Also delete the associated image file from the server
            DeleteExistingImage(articleToDelete.FeaturedImageUrl);

            _context.Articles.Remove(articleToDelete);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"A(z) \"{articleToDelete.Title}\" című hír sikeresen törölve lett.";

            return RedirectToAction(nameof(Index));
        }

        #region Private Helper Methods

        private async Task<string> ProcessUploadedFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return null; // Return null if no file is uploaded
            }

            // Define the path to the uploads folder
            string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "img", "news");
            // Ensure the directory exists
            Directory.CreateDirectory(uploadsFolder);

            // Generate a unique filename to prevent overwriting files
            string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(file.FileName);
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            // Save the file to the server
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            // Return the relative path to be stored in the database
            return $"/img/news/{uniqueFileName}";
        }

        private void DeleteExistingImage(string relativePath)
        {
            if (string.IsNullOrEmpty(relativePath))
            {
                return;
            }

            var fullPath = Path.Combine(_webHostEnvironment.WebRootPath, relativePath.TrimStart('/'));
            if (System.IO.File.Exists(fullPath))
            {
                System.IO.File.Delete(fullPath);
            }
        }

        private string CreateExcerptFromHtml(string content, int maxLength = 200)
        {
            if (string.IsNullOrWhiteSpace(content)) return null;

            var plainText = Regex.Replace(content, "<.*?>", string.Empty);
            if (plainText.Length <= maxLength) return plainText;

            return plainText.Substring(0, maxLength).TrimEnd() + "...";
        }

        private async Task<string> GenerateUniqueSlug(string phrase)
        {
            if (string.IsNullOrEmpty(phrase)) return Guid.NewGuid().ToString();

            string str = phrase.ToLowerInvariant();
            str = str.Replace('á', 'a').Replace('é', 'e').Replace('í', 'i').Replace('ó', 'o').Replace('ö', 'o').Replace('ő', 'o').Replace('ú', 'u').Replace('ü', 'u').Replace('ű', 'u');
            str = Regex.Replace(str, @"[^a-z0-9\s-]", "");
            str = Regex.Replace(str, @"\s+", " ").Trim();
            str = Regex.Replace(str, @"\s", "-");
            str = Regex.Replace(str, @"-+", "-");

            // Ensure slug is unique by appending a number if it already exists
            int i = 1;
            var originalSlug = str;
            while (await _context.Articles.AnyAsync(a => a.Slug == str))
            {
                str = $"{originalSlug}-{i++}";
            }

            return str;
        }

        #endregion
    }
}