using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nadasdladany.Data;
using Nadasdladany.Models;
using Nadasdladany.ViewModels; // Required for the CreateEventViewModel
using System.Text.RegularExpressions; // Required for slug generation

namespace Nadasdladany.Controllers
{
    public class EventsController : Controller
    {
        private readonly NadasdladanyDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager; // <-- Added

        // PageSize is adjusted so a full page of 9 items can include the "Add" card for admins
        private const int PageSize = 8;

        // UPDATED CONSTRUCTOR: Injects both DbContext and UserManager
        public EventsController(NadasdladanyDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Events/Index
        // No major changes were needed here, this action correctly retrieves and filters events.
        public async Task<IActionResult> Index(string? filter = "upcoming", int page = 1)
        {
            ViewData["Title"] = "Eseménynaptár";
            IQueryable<Event> eventsQuery = _context.Events
                                               .Where(e => e.IsPublished);

            DateTime today = DateTime.UtcNow.Date;

            switch (filter?.ToLower())
            {
                case "past":
                    eventsQuery = eventsQuery.Where(e => (e.EndDate.HasValue && e.EndDate.Value < today) || (!e.EndDate.HasValue && e.StartDate < today))
                                           .OrderByDescending(e => e.StartDate);
                    ViewData["ListTitle"] = "Korábbi Események";
                    break;
                case "all":
                    eventsQuery = eventsQuery.OrderByDescending(e => e.StartDate);
                    ViewData["ListTitle"] = "Összes Esemény";
                    break;
                case "upcoming":
                default:
                    eventsQuery = eventsQuery.Where(e => e.StartDate.Date >= today || (e.EndDate.HasValue && e.EndDate.Value.Date >= today))
                                           .OrderBy(e => e.StartDate);
                    ViewData["ListTitle"] = "Közelgő Események";
                    filter = "upcoming";
                    break;
            }

            var totalEvents = await eventsQuery.CountAsync();
            var eventsToList = await eventsQuery.Skip((page - 1) * PageSize)
                                             .Take(PageSize)
                                             .ToListAsync();

            ViewBag.CurrentFilter = filter;
            ViewBag.TotalEvents = totalEvents;
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = PageSize;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalEvents / PageSize);

            return View(eventsToList);
        }

        // GET: Events/Details/{slug}
        // No changes were needed for this action.
        public async Task<IActionResult> Details(string slug)
        {
            if (string.IsNullOrEmpty(slug))
            {
                return BadRequest("Event slug cannot be empty.");
            }

            var eventItem = await _context.Events
                                  .Where(e => e.IsPublished && e.Slug == slug)
                                  .FirstOrDefaultAsync();

            if (eventItem == null)
            {
                TempData["ErrorMessage"] = "A keresett esemény nem található vagy nem publikus.";
                return RedirectToAction(nameof(Index));
            }

            ViewData["Title"] = eventItem.Title;
            return View(eventItem);
        }


        // --- NEW ACTION FOR HANDLING THE 'ADD EVENT' MODAL FORM ---
        [HttpPost]
        [Authorize(Roles = "Administrator")] // Secures the endpoint
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateEventViewModel model)
        {
            // The IsAllDay checkbox only sends a value if it's checked ('true'), not 'false' if unchecked.
            // We ensure it has a value.
            if (!ModelState.ContainsKey(nameof(model.IsAllDay)))
            {
                model.IsAllDay = false;
            }

            if (ModelState.IsValid)
            {
                // Create a new Event database model from the incoming ViewModel
                var newEvent = new Event
                {
                    Title = model.Title,
                    Description = model.Description,
                    StartDate = model.StartDate,
                    EndDate = model.EndDate,
                    Location = model.Location,
                    IsAllDay = model.IsAllDay,
                    Organizer = model.Organizer,
                    EventUrl = model.EventUrl,
                    IsPublished = true, // Default to published
                    Slug = await GenerateUniqueSlug(model.Title, model.StartDate) // Generate a unique slug
                };

                _context.Events.Add(newEvent);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Az esemény sikeresen létrehozva!";
                return RedirectToAction(nameof(Index), new { filter = "upcoming" });
            }

            // If model validation fails, compile the errors and show them to the user.
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
        public async Task<IActionResult> Edit(EditEventViewModel model)
        {
            // The IsAllDay checkbox is only sent if checked. This ensures it's false if unchecked.
            if (!ModelState.ContainsKey(nameof(model.IsAllDay)))
            {
                model.IsAllDay = false;
            }

            if (ModelState.IsValid)
            {
                // Find the original event in the database
                var eventToUpdate = await _context.Events.FindAsync(model.Id);
                if (eventToUpdate == null)
                {
                    TempData["ErrorMessage"] = "A szerkesztendő esemény nem található.";
                    return RedirectToAction(nameof(Index));
                }

                // Update the properties
                eventToUpdate.Title = model.Title;
                eventToUpdate.Description = model.Description;
                eventToUpdate.StartDate = model.StartDate;
                eventToUpdate.EndDate = model.EndDate;
                eventToUpdate.Location = model.Location;
                eventToUpdate.IsAllDay = model.IsAllDay;
                eventToUpdate.Organizer = model.Organizer;
                eventToUpdate.EventUrl = model.EventUrl;

                // Regenerate the slug in case the title or date changed
                eventToUpdate.Slug = await GenerateUniqueSlug(model.Title, model.StartDate, model.Id);

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Az esemény sikeresen frissítve!";
                return RedirectToAction(nameof(Index), new { filter = ViewBag.CurrentFilter ?? "upcoming" });
            }

            TempData["ErrorMessage"] = "Hiba történt a mentés során. Ellenőrizze a megadott adatokat.";
            return RedirectToAction(nameof(Index));
        }

        // --- NEW ACTION FOR DELETING AN EVENT ---
        [HttpPost]
        [Authorize(Roles = "Administrator")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var eventToDelete = await _context.Events.FindAsync(id);
            if (eventToDelete == null)
            {
                TempData["ErrorMessage"] = "A törlendő esemény nem található, vagy már törölve lett.";
                return RedirectToAction(nameof(Index));
            }

            _context.Events.Remove(eventToDelete);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"A(z) \"{eventToDelete.Title}\" című esemény sikeresen törölve lett.";

            return RedirectToAction(nameof(Index));
        }


        // --- UPDATED HELPER METHOD TO CREATE A UNIQUE URL-FRIENDLY SLUG ---
        private async Task<string> GenerateUniqueSlug(string phrase, DateTime date, int? currentEventId = null)
        {
            if (string.IsNullOrWhiteSpace(phrase))
                return Guid.NewGuid().ToString();

            string str = phrase.ToLowerInvariant().Trim();
            str = str.Replace('á', 'a').Replace('é', 'e').Replace('í', 'i').Replace('ó', 'o').Replace('ö', 'o').Replace('ő', 'o').Replace('ú', 'u').Replace('ü', 'u').Replace('ű', 'u');
            str = Regex.Replace(str, @"[^a-z0-9\s-]", "");
            str = Regex.Replace(str, @"\s+", "-");

            str = Regex.Replace(str, @"-+", "-");
            str = $"{str}-{date:yyyy-MM-dd}";

            var originalSlug = str;
            int i = 1;

            // Check if slug exists on ANY OTHER event
            while (await _context.Events.AnyAsync(e => e.Slug == str && e.Id != currentEventId))
            {
                str = $"{originalSlug}-{i++}";
            }

            return str;
        }
    }
}