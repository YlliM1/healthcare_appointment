using healthcare_appointment.Models; // Ensure this is included
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore; // Required for ToListAsync
using System.Threading.Tasks;

namespace healthcare_appointment.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminDashboardController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<AdminDashboardController> _logger;

        public AdminDashboardController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, ILogger<AdminDashboardController> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        // GET: AdminDashboard/Index
        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync(); // Materialize the query to a list
            return View(users); // Pass the list to the view
        }

        // GET: AdminDashboard/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: AdminDashboard/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ApplicationUser user, string password)
        {
            if (ModelState.IsValid)
            {
                var result = await _userManager.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User created successfully.");
                    return RedirectToAction(nameof(Index));
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(user);
        }

        // GET: AdminDashboard/Edit/{id}
        public async Task<IActionResult> Edit(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();
            return View(user);
        }

        // POST: AdminDashboard/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, ApplicationUser user)
        {
            if (id != user.Id) return NotFound();

            if (ModelState.IsValid)
            {
                var existingUser = await _userManager.FindByIdAsync(id);
                if (existingUser == null) return NotFound();

                existingUser.FirstName = user.FirstName;
                existingUser.LastName = user.LastName;
                existingUser.IsDoctor = user.IsDoctor;
                existingUser.ServiceSpecialization = user.ServiceSpecialization;
                existingUser.UserName = user.UserName;
                existingUser.Email = user.Email;

                var result = await _userManager.UpdateAsync(existingUser);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User updated successfully.");
                    return RedirectToAction(nameof(Index));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(user);
        }

        // DELETE: AdminDashboard/Delete/{id}
        // DELETE: AdminDashboard/Delete/{id}
        [HttpDelete]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return Json(new { success = false, error = "User not found" });

            // Perform deletion
            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                _logger.LogInformation("User deleted successfully.");
                return Json(new { success = true });
            }

            // Return errors if deletion failed
            var errorMessage = string.Join(", ", result.Errors.Select(e => e.Description));
            return Json(new { success = false, error = errorMessage });
        }

    }
}
