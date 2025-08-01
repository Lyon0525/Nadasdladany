using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // For Include, ToListAsync etc.
using Nadasdladany.Controllers;
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
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly UserManager<ApplicationUser> _userManager;

        public MunicipalityController(
            NadasdladanyDbContext context,
            IWebHostEnvironment webHostEnvironment,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _userManager = userManager;
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

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateRepresentative(CreateRepresentativeViewModel model)
        {
            if (ModelState.IsValid)
            {
                string imageUrl = await ProcessRepresentativeImage(model.ImageFile);
                var newRep = new Representative
                {
                    Name = model.Name,
                    Role = model.Role,
                    CustomTitleOverride = model.CustomTitleOverride,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    ImageUrl = imageUrl,
                    Biography = model.Biography,
                    DisplayOrder = model.DisplayOrder,
                    IsPublished = true
                };

                _context.Representatives.Add(newRep);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Képviselő sikeresen hozzáadva!";
                return RedirectToAction(nameof(Representatives));
            }

            var errorList = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            TempData["ErrorMessage"] = "Hiba történt a mentés során: " + string.Join(" ", errorList);
            return RedirectToAction(nameof(Representatives));
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")] 
        [ValidateAntiForgeryToken]          
        public async Task<IActionResult> EditOfficeDetails(OfficeDetailsViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Since there is only one office record, we find it (usually by a known ID, e.g., 1)
                var officeDetailsToUpdate = await _context.OfficeInfos.FindAsync(model.Id);

                if (officeDetailsToUpdate == null)
                {
                    // This case is unlikely but good to handle. Maybe create the record if it doesn't exist.
                    TempData["ErrorMessage"] = "A hivatali adatlap nem található az adatbázisban.";
                    return RedirectToAction(nameof(Office));
                }

                // Update the properties of the database entity from the view model
                officeDetailsToUpdate.OfficeName = model.OfficeName;
                officeDetailsToUpdate.AboutOffice = model.AboutOffice;
                officeDetailsToUpdate.Address = model.Address;
                officeDetailsToUpdate.PhoneNumber = model.PhoneNumber;
                officeDetailsToUpdate.Email = model.Email;
                officeDetailsToUpdate.WebsiteUrl = model.WebsiteUrl;
                officeDetailsToUpdate.GoogleMapsEmbedUrl = model.GoogleMapsEmbedUrl;

                try
                {
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "A hivatali adatok sikeresen frissítve lettek.";
                }
                catch (DbUpdateException ex)
                {
                    // Log the error for debugging
                    // _logger.LogError(ex, "Error updating office details.");
                    TempData["ErrorMessage"] = "Hiba történt mentés közben. Kérjük, próbálja újra.";
                }

                return RedirectToAction(nameof(Office));

            }

            // If the model state is not valid, collect the errors and return to the page
            var errorList = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();
            TempData["ErrorMessage"] = "Érvénytelen adatok: " + string.Join(" ", errorList);
            return RedirectToAction(nameof(Office));
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditRepresentative(EditRepresentativeViewModel model)
        {
            if (ModelState.IsValid)
            {
                var representativeToUpdate = await _context.Representatives.FindAsync(model.Id);
                if (representativeToUpdate == null)
                {
                    TempData["ErrorMessage"] = "A szerkesztendő személy nem található.";
                    return RedirectToAction(nameof(Representatives));
                }

                // --- HANDLE IMAGE UPLOAD LOGIC ---
                if (model.ImageFile != null)
                {
                    DeleteExistingImage(representativeToUpdate.ImageUrl);
                    representativeToUpdate.ImageUrl = await ProcessRepresentativeImage(model.ImageFile);
                }
                else if (model.RemoveCurrentImage)
                {
                    DeleteExistingImage(representativeToUpdate.ImageUrl);
                    representativeToUpdate.ImageUrl = null;
                }

                // Update the rest of the properties
                representativeToUpdate.Name = model.Name;
                representativeToUpdate.Role = model.Role;
                representativeToUpdate.CustomTitleOverride = model.CustomTitleOverride;
                representativeToUpdate.Email = model.Email;
                representativeToUpdate.PhoneNumber = model.PhoneNumber;
                representativeToUpdate.Biography = model.Biography;
                representativeToUpdate.DisplayOrder = model.DisplayOrder;

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "A képviselő adatai sikeresen frissítve lettek.";
                return RedirectToAction(nameof(Representatives));
            }

            var errorList = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            TempData["ErrorMessage"] = "Hiba történt a mentés során: " + string.Join(" ", errorList);
            return RedirectToAction(nameof(Representatives));
        }


        [HttpPost]
        [Authorize(Roles = "Administrator")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteRepresentative(int id)
        {
            var representativeToDelete = await _context.Representatives.FindAsync(id);
            if (representativeToDelete == null)
            {
                TempData["ErrorMessage"] = "A törlendő személy nem található, vagy már törölve lett.";
                return RedirectToAction(nameof(Representatives));
            }

            // Prevent accidental deletion of the Mayor from this general list page
            if (representativeToDelete.Role == RepresentativeRole.Polgarmester)
            {
                TempData["ErrorMessage"] = "A Polgármestert erről az oldalról nem lehet törölni. A Polgármesteri menüpontban végezhető el a módosítás.";
                return RedirectToAction(nameof(Representatives));
            }

            _context.Representatives.Remove(representativeToDelete);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"A(z) \"{representativeToDelete.Name}\" nevű személy sikeresen törölve lett.";

            return RedirectToAction(nameof(Representatives));
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditMayor(EditMayorViewModel model)
        {
            if (ModelState.IsValid)
            {
                var mayorToUpdate = await _context.Representatives
                    .FirstOrDefaultAsync(r => r.Id == model.Id && r.Role == RepresentativeRole.Polgarmester);

                if (mayorToUpdate == null)
                {
                    TempData["ErrorMessage"] = "A polgármesteri adatlap nem található.";
                    return RedirectToAction(nameof(Mayor));
                }

                if (model.ImageFile != null)
                {
                    DeleteExistingImage(mayorToUpdate.ImageUrl);
                    mayorToUpdate.ImageUrl = await ProcessRepresentativeImage(model.ImageFile);
                }
                else if (model.RemoveCurrentImage)
                {
                    DeleteExistingImage(mayorToUpdate.ImageUrl);
                    mayorToUpdate.ImageUrl = null;
                }

                mayorToUpdate.Name = model.Name;
                mayorToUpdate.CustomTitleOverride = model.CustomTitleOverride;
                mayorToUpdate.Email = model.Email;
                mayorToUpdate.PhoneNumber = model.PhoneNumber;
                mayorToUpdate.ReceptionHoursInfo = model.ReceptionHoursInfo;
                mayorToUpdate.Biography = model.Biography;

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "A polgármester adatai sikeresen frissítve lettek.";
                return RedirectToAction(nameof(Mayor));
            }

            var errorList = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            TempData["ErrorMessage"] = "Hiba történt a mentés során: " + string.Join(" ", errorList);
            return RedirectToAction(nameof(Mayor));
        }

        [HttpGet]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> ManageOfficeHours()
        {
            ViewData["Title"] = "Ügyfélfogadás Kezelése";
            var officeHours = await _context.OfficeHourEntries
                                            .OrderBy(oh => oh.DisplayOrder)
                                            .ThenBy(oh => oh.DayOfWeek)
                                            .ToListAsync();
            return View(officeHours);
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateOfficeHour(CreateOfficeHourViewModel model)
        {
            if (ModelState.IsValid)
            {
                var newEntry = new OfficeHourEntry
                {
                    DayOfWeek = model.DayOfWeek,
                    TimeDescription = model.TimeDescription,
                    DisplayOrder = model.DisplayOrder
                };

                _context.OfficeHourEntries.Add(newEntry);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Új ügyfélfogadási időpont sikeresen hozzáadva.";
                return RedirectToAction(nameof(ManageOfficeHours));
            }

            TempData["ErrorMessage"] = "Hiba történt a létrehozás során. Minden mező kitöltése kötelező.";
            return RedirectToAction(nameof(ManageOfficeHours));
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditOfficeHour(EditOfficeHourViewModel model)
        {
            if (ModelState.IsValid)
            {
                var entryToUpdate = await _context.OfficeHourEntries.FindAsync(model.Id);
                if (entryToUpdate == null)
                {
                    TempData["ErrorMessage"] = "A szerkesztendő időpont nem található.";
                    return RedirectToAction(nameof(ManageOfficeHours));
                }

                entryToUpdate.DayOfWeek = model.DayOfWeek;
                entryToUpdate.TimeDescription = model.TimeDescription;
                entryToUpdate.DisplayOrder = model.DisplayOrder;

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Az időpont sikeresen frissítve.";
                return RedirectToAction(nameof(ManageOfficeHours));
            }

            TempData["ErrorMessage"] = "Érvénytelen adatok. Minden mező kitöltése kötelező.";
            return RedirectToAction(nameof(ManageOfficeHours));
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteOfficeHour(int id)
        {
            var entryToDelete = await _context.OfficeHourEntries.FindAsync(id);
            if (entryToDelete == null)
            {
                TempData["ErrorMessage"] = "A törlendő időpont nem található.";
                return RedirectToAction(nameof(ManageOfficeHours));
            }

            _context.OfficeHourEntries.Remove(entryToDelete);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Az időpont sikeresen törölve.";

            return RedirectToAction(nameof(ManageOfficeHours));
        }


        #region Private Helper Methods

        private async Task<string> ProcessRepresentativeImage(IFormFile file)
        {
            if (file == null || file.Length == 0) return null;

            string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "img", "reps");
            Directory.CreateDirectory(uploadsFolder);
            string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(file.FileName);
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }
            return $"/img/reps/{uniqueFileName}";
        }

        private void DeleteExistingImage(string relativePath)
        {
            if (string.IsNullOrEmpty(relativePath)) return;
            var fullPath = Path.Combine(_webHostEnvironment.WebRootPath, relativePath.TrimStart('/'));
            if (System.IO.File.Exists(fullPath))
            {
                System.IO.File.Delete(fullPath);
            }
        }

        #endregion

    }
}