using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // For Include, ToListAsync etc.
using Nadasdladany.Data;    // Your DbContext namespace
using Nadasdladany.Models; // Your Model namespace
using Nadasdladany.ViewModels;
using System.Linq;
using System.Threading.Tasks;

namespace NadasdladanyWebApp.MVC.Controllers // Or Nadasdladany.Controllers
{
    public class MunicipalityController : Controller
    {
        private readonly NadasdladanyDbContext _context; // Inject NadasdladanyDbContext

        public MunicipalityController(NadasdladanyDbContext context)
        {
            _context = context;
        }

        // GET: /Municipality/Office
        public async Task<IActionResult> Office()
        {
            ViewData["Title"] = "Polgármesteri Hivatal";
            var officeInfo = await _context.OfficeInfos.FirstOrDefaultAsync(oi => oi.Id == 1); // Assuming Id=1 for the main office info
            var officeHours = await _context.OfficeHourEntries.OrderBy(oh => oh.DisplayOrder).ThenBy(oh => oh.DayOfWeek).ToListAsync();
            var keyStaff = await _context.Representatives
                                   .Where(r => r.IsPublished &&
                                          (r.Role == RepresentativeRole.Jegyző || r.Role == RepresentativeRole.HivataliVezető || r.Role == RepresentativeRole.Munkatars))
                                   .OrderBy(r => r.DisplayOrder).ThenBy(r => r.Name)
                                   .ToListAsync();


            var viewModel = new OfficePageViewModel // Create this ViewModel
            {
                OfficeDetails = officeInfo ?? new OfficeInfo(), // Provide default if null
                OfficeHours = officeHours,
                KeyStaffMembers = keyStaff
            };
            return View(viewModel);
        }

        // GET: /Municipality/Mayor
        public async Task<IActionResult> Mayor()
        {
            ViewData["Title"] = "Polgármester";
            var mayor = await _context.Representatives
                                .FirstOrDefaultAsync(r => r.Role == RepresentativeRole.Polgarmester && r.IsPublished);

            if (mayor == null)
            {
                // Handle case where mayor is not found, maybe return a specific view or redirect with error
                // For now, pass a new empty representative if null to prevent view errors
                // TempData["ErrorMessage"] = "Polgármesteri adatlap nem található.";
                // return RedirectToAction("Index", "Home");
                mayor = new Representative { Name = "Ismeretlen Polgármester", Biography = "Az adatok feltöltés alatt állnak." };
            }
            return View(mayor);
        }

        // GET: /Municipality/Representatives
        public async Task<IActionResult> Representatives()
        {
            ViewData["Title"] = "Képviselő-testület";
            var councilMembers = await _context.Representatives
                                         .Where(r => r.IsPublished &&
                                                (r.Role == RepresentativeRole.Polgarmester ||
                                                 r.Role == RepresentativeRole.Alpolgarmester ||
                                                 r.Role == RepresentativeRole.Kepviselo))
                                         .OrderBy(r => r.DisplayOrder)
                                         .ThenBy(r => r.Name)
                                         .ToListAsync();
            return View(councilMembers);
        }

        // --- NEW ACTION TO HANDLE THE 'ADD REPRESENTATIVE' MODAL FORM ---
        [HttpPost]
        [Authorize(Roles = "Administrator")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateRepresentative(CreateRepresentativeViewModel model)
        {
            if (ModelState.IsValid)
            {
                var newRep = new Representative
                {
                    Name = model.Name,
                    Role = model.Role,
                    CustomTitleOverride = model.CustomTitleOverride,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    ImageUrl = model.ImageUrl,
                    Biography = model.Biography,
                    DisplayOrder = model.DisplayOrder,
                    IsPublished = true // Default to published
                };

                _context.Representatives.Add(newRep);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Képviselő sikeresen hozzáadva!";
                return RedirectToAction(nameof(Representatives));
            }

            var errorList = ModelState.Values
                                .SelectMany(v => v.Errors)
                                .Select(e => e.ErrorMessage)
                                .ToList();
            TempData["ErrorMessage"] = "Hiba történt a mentés során: " + string.Join(" ", errorList);
            return RedirectToAction(nameof(Representatives));
        }
    }
}