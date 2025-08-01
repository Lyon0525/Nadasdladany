using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nadasdladany.Data;
using Nadasdladany.Models;
using Nadasdladany.ViewModels;
using System.Linq;
using System.Threading.Tasks;

namespace Nadasdladany.Controllers
{
    [Authorize(Roles = "Administrator")] // VERY IMPORTANT: Secures the entire controller
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly NadasdladanyDbContext _context;

        public AdminController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, NadasdladanyDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        // GET: /Admin/Users
        public async Task<IActionResult> Users()
        {
            // Get all users who are in the "Administrator" role
            var admins = await _userManager.GetUsersInRoleAsync("Administrator");

            // Map them to the view model for display
            var adminViewModels = admins.Select(user => new AdminUserViewModel
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email
            }).ToList();

            return View(adminViewModels);
        }

        [HttpGet]
        public async Task<IActionResult> Messages(int page = 1)
        {
            const int pageSize = 10;
            ViewData["Title"] = "Felkeresések Megtekintése";

            var query = _context.ContactSubmissions.OrderByDescending(s => s.SubmittedDate);

            var totalMessages = await query.CountAsync();
            var messages = await query
                                .Skip((page - 1) * pageSize)
                                .Take(pageSize)
                                .ToListAsync();

            var viewModel = new AdminMessagesViewModel
            {
                Messages = messages,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling(totalMessages / (double)pageSize)
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAdmin(CreateAdminViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    // Ensure the "Administrator" role exists
                    if (!await _roleManager.RoleExistsAsync("Administrator"))
                    {
                        await _roleManager.CreateAsync(new IdentityRole("Administrator"));
                    }
                    // Add the new user to the "Administrator" role
                    await _userManager.AddToRoleAsync(user, "Administrator");

                    TempData["SuccessMessage"] = "Adminisztrátor sikeresen létrehozva.";
                    return RedirectToAction("Users");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            TempData["ErrorMessage"] = "Hiba történt a létrehozás során. " + string.Join(" ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
            return RedirectToAction("Users");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAdmin(EditAdminViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(model.Id);
                if (user == null)
                {
                    TempData["ErrorMessage"] = "A felhasználó nem található.";
                    return RedirectToAction("Users");
                }

                user.Email = model.Email;
                user.UserName = model.Email; // Keep UserName and Email in sync

                var updateResult = await _userManager.UpdateAsync(user);

                if (!updateResult.Succeeded)
                {
                    TempData["ErrorMessage"] = "Hiba az adatok frissítése során.";
                    return RedirectToAction("Users");
                }

                // Handle optional password reset
                if (!string.IsNullOrEmpty(model.NewPassword))
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    var passwordResult = await _userManager.ResetPasswordAsync(user, token, model.NewPassword);
                    if (!passwordResult.Succeeded)
                    {
                        TempData["ErrorMessage"] = "Jelszó frissítése sikertelen.";
                        return RedirectToAction("Users");
                    }
                }

                TempData["SuccessMessage"] = "Adminisztrátor adatai sikeresen frissítve.";
                return RedirectToAction("Users");
            }

            TempData["ErrorMessage"] = "Érvénytelen adatok.";
            return RedirectToAction("Users");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAdmin(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                TempData["ErrorMessage"] = "A felhasználó nem található.";
                return RedirectToAction("Users");
            }

            // CRITICAL: Prevent a user from deleting their own account
            var currentUserId = _userManager.GetUserId(User);
            if (user.Id == currentUserId)
            {
                TempData["ErrorMessage"] = "Figyelem: Saját magát nem törölheti!";
                return RedirectToAction("Users");
            }

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Adminisztrátor sikeresen törölve.";
            }
            else
            {
                TempData["ErrorMessage"] = "Hiba a törlés során.";
            }

            return RedirectToAction("Users");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleMessageReadStatus(int id)
        {
            var message = await _context.ContactSubmissions.FindAsync(id);
            if (message != null)
            {
                message.IsRead = !message.IsRead; // Flip the boolean value
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Üzenet állapota sikeresen módosítva.";
            }
            else
            {
                TempData["ErrorMessage"] = "Az üzenet nem található.";
            }

            // Redirect back to the messages page
            return RedirectToAction(nameof(Messages));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteMessage(int id)
        {
            var message = await _context.ContactSubmissions.FindAsync(id);
            if (message != null)
            {
                _context.ContactSubmissions.Remove(message);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Üzenet sikeresen törölve.";
            }
            else
            {
                TempData["ErrorMessage"] = "A törlendő üzenet nem található.";
            }

            // Redirect back to the main messages list
            return RedirectToAction(nameof(Messages));
        }
    }
}