using GPURental.Models;
using GPURental.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace GPURental.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        // Constructor to inject the Identity services
        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // Add these methods INSIDE the AccountController class

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken] // Recommended for all POST actions
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new User
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FullName = model.FullName,
                    CreatedAt = DateTime.UtcNow,
                    BalanceInCents = 0,
                    Timezone = model.Timezone
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    // --- UPDATED ROLE ASSIGNMENT LOGIC ---
                    // Always assign the "Renter" role by default.
                    await _userManager.AddToRoleAsync(user, "Renter");

                    // If the user checked the box, also assign the "Provider" role.
                    if (model.IsProvider)
                    {
                        await _userManager.AddToRoleAsync(user, "Provider");
                    }
                    // ------------------------------------

                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(model);
        }


        // Add these methods INSIDE the AccountController class

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [AllowAnonymous] // Allow anyone (even anonymous users, though unlikely to hit this) to see this page
        public IActionResult AccessDenied()
        {
            return View();
        }

        // Add these methods INSIDE your existing AccountController.cs

        [Authorize] // Protect this action
        [HttpGet]
        public async Task<IActionResult> Manage()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            var model = new ManageAccountViewModel
            {
                Email = user.Email,
                FullName = user.FullName,
                Timezone = user.Timezone
            };

            return View(model);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Manage(ManageAccountViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            // --- Repopulate the email in case of any error ---
            model.Email = user.Email;
            // ------------------------------------------------

            // Update profile information first
            user.FullName = model.FullName;
            user.Timezone = model.Timezone;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(model); // Return view with profile update errors
            }

            // Handle Password Change if fields are provided
            if (!string.IsNullOrEmpty(model.OldPassword) && !string.IsNullOrEmpty(model.NewPassword))
            {
                var changePasswordResult = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
                if (!changePasswordResult.Succeeded)
                {
                    foreach (var error in changePasswordResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View(model); // Return view with password change errors
                }

                await _signInManager.RefreshSignInAsync(user);
            }

            TempData["SuccessMessage"] = "Your profile has been updated.";
            return RedirectToAction("Manage");
        }

        // Add this method INSIDE your AccountController.cs

        [Authorize] // User must be logged in
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BecomeProvider()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            // Check if the user is already a provider to avoid doing extra work
            if (!await _userManager.IsInRoleAsync(user, "Provider"))
            {
                // Add the user to the "Provider" role
                await _userManager.AddToRoleAsync(user, "Provider");

                // Re-sign the user in so their new role claim is included in their cookie
                await _signInManager.RefreshSignInAsync(user);

                TempData["SuccessMessage"] = "Congratulations! You are now a provider and can create listings.";
            }

            return RedirectToAction("Manage");
        }
    }
}