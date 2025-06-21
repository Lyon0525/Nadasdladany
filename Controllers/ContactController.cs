using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;    // For ILogger
using Nadasdladany.Data;           // For NadasdladanyDbContext (your DbContext namespace)
using Nadasdladany.Models;           // For ContactSubmission and ContactFormViewModel (your models namespace)
using Nadasdladany.ViewModels;
using System;
using System.Threading.Tasks;

namespace NadasdladanyWebApp.MVC.Controllers // Your controller namespace
{
    public class ContactController : Controller
    {
        private readonly NadasdladanyDbContext _context; // Assuming you want to save to DB
        private readonly ILogger<ContactController> _logger;

        // Constructor for DB saving and logging
        public ContactController(NadasdladanyDbContext context, ILogger<ContactController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // Constructor if NOT saving to DB (only logging, for example)
        // public ContactController(ILogger<ContactController> logger)
        // {
        //    _logger = logger;
        // }


        // GET: /Contact or /Contact/Index
        [HttpGet]
        public IActionResult Index()
        {
            ViewData["Title"] = "Kapcsolatfelvétel";
            return View(new ContactFormViewModel()); // Pass an empty ViewModel to the view for the form
        }

        // POST: /Contact or /Contact/SubmitMessage (matching asp-action in form)
        [HttpPost]
        [ValidateAntiForgeryToken]
        // Renamed the action to align with form asp-action, or use [HttpPost("Index")]
        public async Task<IActionResult> SubmitMessage(ContactFormViewModel model)
        {
            ViewData["Title"] = "Kapcsolatfelvétel";

            if (ModelState.IsValid)
            {
                try
                {
                    // --- OPTION 1: Save to Database (if ContactSubmission entity and DbContext are set up) ---
                    var submission = new ContactSubmission
                    {
                        Name = model.Name,
                        Email = model.Email,
                        Subject = model.Subject,
                        Message = model.Message,
                        SubmittedDate = DateTime.UtcNow,
                        IsRead = false
                        // AdminNotes = null; // Initially
                    };

                    _context.ContactSubmissions.Add(submission);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Contact form submission saved from {Email} with subject {Subject}", model.Email, model.Subject);
                    // --- End Option 1 ---

                    // TODO: Implement actual email sending logic here if desired
                    // Example: await _emailSender.SendEmailAsync("admin@nadasdladany.hu", $"Új üzenet: {model.Subject}", $"Feladó: {model.Name} <{model.Email}>\n\nÜzenet:\n{model.Message}");

                    TempData["ContactFormSuccess"] = "Köszönjük üzenetét! Hamarosan felvesszük Önnel a kapcsolatot.";
                    ModelState.Clear(); // Clear the form
                    return View("Index", new ContactFormViewModel()); // Return to Index view with a new empty model
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Hiba történt a kapcsolatfelvételi űrlap feldolgozása közben.");
                    TempData["ContactFormError"] = "Sajnáljuk, hiba történt az üzenet elküldése közben. Kérjük, próbálja meg később.";
                    // Return the view with the submitted model to show errors and preserve user input
                    return View("Index", model);
                }
            }

            // If ModelState is not valid, return the view with the model to display validation errors
            TempData["ContactFormError"] = "Kérjük, javítsa a pirossal jelölt hibákat az űrlapon.";
            return View("Index", model);
        }


    }
}