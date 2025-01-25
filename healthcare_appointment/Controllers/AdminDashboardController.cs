using healthcare_appointment.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore; // Required for ToListAsync
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;

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

        // GET: AdminDashboard/Index with Pagination
        public async Task<IActionResult> Index(int page = 1, string searchText = "", string specialization = "")
        {
            const int pageSize = 10; // Number of users per page

            // Fixed list of specializations
            var specializations = new List<string>
    {
        "Dentist",
        "Pediatrics",
        "Dermatology",
        "General Checkup",
        "Cardiology"
    };

            // Get the total number of users
            var query = _userManager.Users.AsQueryable();

            // Apply search filter if searchText is provided
            if (!string.IsNullOrEmpty(searchText))
            {
                query = query.Where(u => u.Email.Contains(searchText) ||
                                         u.FirstName.Contains(searchText) ||
                                         u.LastName.Contains(searchText));
            }

            // Apply specialization filter if selected
            if (!string.IsNullOrEmpty(specialization))
            {
                query = query.Where(u => u.ServiceSpecialization == specialization && u.IsDoctor);
            }

            var totalUsers = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalUsers / (double)pageSize);

            // Get the users for the current page
            var users = await query
                .Skip((page - 1) * pageSize) // Skip users from previous pages
                .Take(pageSize) // Take the users for the current page
                .ToListAsync();

            // Create a PaginatedList to hold the users and pagination data
            var model = new PaginatedList<ApplicationUser>(users, totalUsers, page, pageSize);

            // Pass the searchText, specialization, and specializations to the view for filtering
            ViewData["SearchText"] = searchText;
            ViewData["Specialization"] = specialization;
            ViewData["Specializations"] = specializations; // Pass the specializations list to the view

            return View(model); // Pass the paginated list to the view
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
