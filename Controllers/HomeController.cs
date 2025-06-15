using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nadasdladany.Data;
using Nadasdladany.Models;
using Nadasdladany.ViewModels;
using System.Diagnostics;

namespace Nadasdladany.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly NadasdladanyDbContext _context;

        public HomeController(ILogger<HomeController> logger, NadasdladanyDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var today = DateTime.UtcNow.Date; // Use UTC for consistency with database

            var latestNews = await _context.Articles
                                     .Where(a => a.IsPublished && a.PublishedDate <= DateTime.UtcNow)
                                     .OrderByDescending(a => a.PublishedDate)
                                     .Include(a => a.Category)
                                     .Take(3)
                                     .ToListAsync();

            var upcomingEvents = await _context.Events
                                       .Where(e => e.StartDate >= today)
                                       .OrderBy(e => e.StartDate)
                                       .Take(5)
                                       .ToListAsync();

            var viewModel = new HomeViewModel
            {
                LatestNews = latestNews,
                UpcomingEvents = upcomingEvents
                // The other properties of HomePageViewModel (SiteName, HeroTitle etc.)
                // will use their default values unless overridden here from a config or DB.
            };

            // This will look for Views/Home/Index.cshtml
            return View(viewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }
    }
}
