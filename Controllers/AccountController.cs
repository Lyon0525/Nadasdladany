using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging; 
using Nadasdladany.Models;         
using Nadasdladany.ViewModels;  
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Nadasdladany.Controllers 
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<AccountController> _logger;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<AccountController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        // GET: /Account/Login
        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["Title"] = "Bejelentkezés";
            ViewData["ReturnUrl"] = returnUrl;
            if (User.Identity.IsAuthenticated) // If already logged in, redirect to home
            {
                return RedirectToAction("Index", "Home");
            }
            return View(); // Assumes Views/Account/Login.cshtml
        }

        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            ViewData["Title"] = "Bejelentkezés";
            ViewData["ReturnUrl"] = returnUrl; // Persist returnUrl

            if (User.Identity.IsAuthenticated) return RedirectToLocal(returnUrl);


            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "Érvénytelen bejelentkezési kísérlet.");
                    _logger.LogWarning("Login attempt failed for non-existent email: {Email}", model.Email);
                    return View(model);
                }

                // For simplicity, not checking lockout or EmailConfirmed here for admin
                // In production, you might want to: options.SignIn.RequireConfirmedAccount = true
                // and then handle if (!await _userManager.IsEmailConfirmedAsync(user))

                var result = await _signInManager.PasswordSignInAsync(user.UserName, model.Password, model.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User '{Email}' logged in.", model.Email);
                    return RedirectToLocal(returnUrl);
                }
                // if (result.RequiresTwoFactor) { /* Handle 2FA */ }
                // if (result.IsLockedOut) { /* Handle lockout */ }
                else
                {
                    ModelState.AddModelError(string.Empty, "Érvénytelen bejelentkezési kísérlet. Hibás e-mail cím vagy jelszó.");
                    _logger.LogWarning("Login attempt failed for email: {Email}. Invalid password.", model.Email);
                    return View(model);
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        // POST: /Account/Logout
        [HttpPost]
        [ValidateAntiForgeryToken] // Good practice even for logout
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync(); // Signs out the current user
            // await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme); // Alternative explicit way
            _logger.LogInformation("User logged out.");
            return RedirectToAction("Index", "Home"); // Redirect to homepage after logout
        }

        // GET: /Account/AccessDenied
        [HttpGet]
        public IActionResult AccessDenied()
        {
            ViewData["Title"] = "Hozzáférés Megtagadva";
            return View(); // Assumes Views/Account/AccessDenied.cshtml
        }


        private IActionResult RedirectToLocal(string? returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home"); // Default redirect
            }
        }
    }
}