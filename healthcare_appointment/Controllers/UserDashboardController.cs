﻿using healthcare_appointment.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;




namespace healthcare_appointment.Controllers

{
    [Authorize] // Ensure only authenticated users can access
    public class UserDashboardController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<UserDashboardController> _logger;

        public UserDashboardController(UserManager<ApplicationUser> userManager, ILogger<UserDashboardController> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }


        // Add this to UserDashboardController

        // GET: UserDashboard/EditProfile
        public async Task<IActionResult> EditProfile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            return View(user); // Pass the user data to the EditProfile view
        }

        // POST: UserDashboard/EditProfile
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(ApplicationUser updatedUser)
        {
            if (!ModelState.IsValid) return View(updatedUser);

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            // Update user details
            user.FirstName = updatedUser.FirstName;
            user.LastName = updatedUser.LastName;
            user.UserName = updatedUser.UserName;
            user.Email = updatedUser.Email;

            // Save changes
            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                _logger.LogInformation("User profile updated successfully.");
                TempData["SuccessMessage"] = "Profile updated successfully!";
                return RedirectToAction(nameof(Index));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(updatedUser);
        }




        // GET: UserDashboard/Index
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            return View(user);
        }

        // GET: UserDashboard/ChangePassword
        public IActionResult ChangePassword()
        {
            return View();
        }

        // POST: UserDashboard/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
            if (result.Succeeded)
            {
                _logger.LogInformation("User changed their password successfully.");
                TempData["SuccessMessage"] = "Password changed successfully!";
                return RedirectToAction(nameof(Index));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }
    }
}
