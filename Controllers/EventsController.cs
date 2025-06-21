using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nadasdladany.Data;
using Nadasdladany.Models;

namespace Nadasdladany.Controllers
{
    public class EventsController : Controller
    {
        private readonly NadasdladanyDbContext _context; // Using your DbContext name
        private const int PageSize = 9; // Number of events per page, adjust as needed

        public EventsController(NadasdladanyDbContext context) // Constructor injects your DbContext
        {
            _context = context;
        }

        // GET: Events or Events/Index
        // Handles filtering for upcoming, past, or all events, and basic pagination
        public async Task<IActionResult> Index(string? filter = "upcoming", int page = 1)
        {
            ViewData["Title"] = "Eseménynaptár"; // Event Calendar
            IQueryable<Event> eventsQuery = _context.Events
                                               .Where(e => e.IsPublished); // Only show published events

            DateTime today = DateTime.UtcNow.Date; // Use UtcNow.Date for comparisons if your DB dates are UTC

            switch (filter?.ToLower())
            {
                case "past":
                    // For past events, EndDate is more relevant if it exists for multi-day events
                    eventsQuery = eventsQuery.Where(e => (e.EndDate.HasValue && e.EndDate.Value < today) || (!e.EndDate.HasValue && e.StartDate < today))
                                           .OrderByDescending(e => e.StartDate);
                    ViewData["ListTitle"] = "Korábbi Események"; // Past Events
                    break;
                case "all":
                    eventsQuery = eventsQuery.OrderByDescending(e => e.StartDate);
                    ViewData["ListTitle"] = "Összes Esemény"; // All Events
                    break;
                case "upcoming":
                default: // Default to upcoming events
                    eventsQuery = eventsQuery.Where(e => e.StartDate >= today || (e.EndDate.HasValue && e.EndDate.Value >= today))
                                           .OrderBy(e => e.StartDate); // Upcoming usually ordered ascending by StartDate
                    ViewData["ListTitle"] = "Közelgő Események"; // Upcoming Events
                    filter = "upcoming"; // Ensure filter variable is explicitly set for the view
                    break;
            }

            ViewBag.CurrentFilter = filter; // Pass current filter to the view for button active states

            // Basic Pagination Logic
            var totalEvents = await eventsQuery.CountAsync();
            var eventsToList = await eventsQuery.Skip((page - 1) * PageSize)
                                             .Take(PageSize)
                                             .ToListAsync();

            ViewBag.TotalEvents = totalEvents;
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = PageSize;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalEvents / PageSize);
            if (ViewBag.TotalPages == 0 && totalEvents > 0) ViewBag.TotalPages = 1; // Handle case with less than PageSize items

            return View(eventsToList); // Pass the list of Event objects to the view
        }

        // GET: Events/Details/{slug}
        // Displays the details of a single event using its slug
        public async Task<IActionResult> Details(string slug)
        {
            if (string.IsNullOrEmpty(slug))
            {
                // Consider logging this bad request
                return BadRequest("Event slug cannot be empty.");
            }

            var eventItem = await _context.Events
                                  .Where(e => e.IsPublished && e.Slug == slug)
                                  .FirstOrDefaultAsync();

            if (eventItem == null)
            {
                // Consider logging this occurrence
                TempData["ErrorMessage"] = "A keresett esemény nem található vagy nem publikus.";
                return RedirectToAction(nameof(Index)); // Or return a specific NotFound view
                // return NotFound(); // Simple NotFound page
            }

            ViewData["Title"] = eventItem.Title;
            return View(eventItem); // Pass the single Event object to the Details view
        }

        // --- Admin Actions for Events (Example Stubs - to be implemented in AdminController or AdminEventsController) ---
        // These would typically be in a separate Admin controller, secured with [Authorize]

        /*
        // GET: AdminEvents/Create
        public IActionResult Create()
        {
            return View(); // An admin view with a form to create an event
        }

        // POST: AdminEvents/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Description,StartDate,EndDate,Location,IsAllDay,Organizer,ContactInfo,EventUrl,IsPublished,Slug")] Event eventItem)
        {
            if (ModelState.IsValid)
            {
                // Generate slug if empty and title is provided
                if (string.IsNullOrEmpty(eventItem.Slug) && !string.IsNullOrEmpty(eventItem.Title))
                {
                    // eventItem.Slug = GenerateSlug(eventItem.Title); // Implement a slug generation utility
                }
                _context.Add(eventItem);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Esemény sikeresen létrehozva!";
                return RedirectToAction(nameof(Index), new { controller = "Events" }); // Redirect to public events list or an admin list
            }
            return View(eventItem); // Return to form with validation errors
        }
        */

        // TODO: Add Edit (GET & POST), Delete (GET & POST) actions for admin management
        // TODO: Implement a slug generation utility function if needed
    }
}

