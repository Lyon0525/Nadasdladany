using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nadasdladany.Data;
using Nadasdladany.Models;
using Nadasdladany.ViewModels;
using System.Diagnostics;
using Microsoft.Extensions.Caching.Memory;

namespace Nadasdladany.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly NadasdladanyDbContext _context;
        private readonly IMemoryCache _cache;

        public HomeController(ILogger<HomeController> logger, NadasdladanyDbContext context, IMemoryCache cache)
        {
            _logger = logger;
            _context = context;
            _cache = cache;
        }

        public async Task<IActionResult> Index()
        {
            // --- This is the complete, correct code with no placeholders ---

            var today = DateTime.UtcNow.Date;

            var latestNews = await _context.Articles
                                     .Where(a => a.IsPublished && a.PublishedDate <= DateTime.UtcNow)
                                     .OrderByDescending(a => a.PublishedDate)
                                     .Include(a => a.Category)
                                     .Take(3)
                                     .ToListAsync();

            var upcomingEvents = await _context.Events
                                       .Where(e => e.StartDate.Date >= today)
                                       .OrderBy(e => e.StartDate)
                                       .Take(5)
                                       .ToListAsync();

            var settings = await _context.SiteSettings.ToDictionaryAsync(s => s.SettingKey, s => s.SettingValue);

            // NEW: Fetch the Mayor's data from the Representatives table
            var mayor = await _context.Representatives
                                .FirstOrDefaultAsync(r => r.Role == RepresentativeRole.Polgarmester && r.IsPublished);

            // Helper function remains the same
            string GetSetting(string key, string defaultValue) => settings.ContainsKey(key) ? settings[key] : defaultValue;

            var viewModel = new HomeViewModel
            {
                LatestNews = latestNews,
                UpcomingEvents = upcomingEvents,
                SiteName = "Nádasdladány Község Honlapja",
                HeroTitle = "Üdvözöljük Nádasdladány Honlapján!",
                HeroSubtitle = "Fedezze fel községünk életét...",
                WelcomeTitle = GetSetting("WelcomeTitle", "Tisztelt Látogató!"),
                WelcomeMessageParagraph1 = GetSetting("WelcomeMessageParagraph1", "Default welcome message 1."),
                WelcomeMessageParagraph2 = GetSetting("WelcomeMessageParagraph2", "Default welcome message 2."),

                // Populate the ViewModel with dynamic mayor data
                MayorDisplayName = mayor?.Name ?? "Polgármester Neve",
                MayorRole = mayor?.CustomTitleOverride ?? "Polgármester",
                MayorImageUrl = mayor?.ImageUrl ?? "/img/mayor-placeholder.png"
            };

            return View(viewModel);
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")] 
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateWelcomeMessage(HomeViewModel model)
        {
            try
            {
                await UpdateSettingAsync("WelcomeTitle", model.WelcomeTitle);
                await UpdateSettingAsync("WelcomeMessageParagraph1", model.WelcomeMessageParagraph1);
                await UpdateSettingAsync("WelcomeMessageParagraph2", model.WelcomeMessageParagraph2);
                await UpdateSettingAsync("MayorName", model.MayorName);

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Üdvözlõ üzenet sikeresen frissítve!";
            }
            catch (Exception ex)
            {
                // Log the exception
                TempData["ErrorMessage"] = "Hiba történt az üzenet frissítése közben.";
            }

            return RedirectToAction("Index");
        }

        // Helper method to update a setting
        private async Task UpdateSettingAsync(string key, string value)
        {
            var setting = await _context.SiteSettings.FindAsync(key);
            if (setting != null)
            {
                setting.SettingValue = value;
                _context.SiteSettings.Update(setting);
            }
            else
            {
                _context.SiteSettings.Add(new SiteSetting { SettingKey = key, SettingValue = value });
            }
        }

        public IActionResult Privacy()
        {
            ViewData["Title"] = "Adatvédelmi és Adatkezelési Nyilatkozat"; 
            return View(); 
        }

        public IActionResult AccessibilityStatement()
        {
            ViewData["Title"] = "Akadálymentesítési Nyilatkozat";
            return View();
        }
    }
}
