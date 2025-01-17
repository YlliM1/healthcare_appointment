using System.Diagnostics;
using healthcare_appointment.Models;
using Microsoft.AspNetCore.Mvc;

namespace healthcare_appointment.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult Appointments()
        {
            if (!User.Identity.IsAuthenticated)
            {
                // Redirect to login page if not logged in
                return RedirectToAction("Login", "Account", new { area = "Identity", returnUrl = "/Home/Appointments" });
            }

            // Otherwise, show the appointments page
            return View();
        }








    }
}
