using Microsoft.AspNetCore.Mvc;

namespace healthcare_appointment.Controllers
{
    public class UserDashboardController : Controller
    {
        // GET: UserDashboard
        public IActionResult Index()
        {
            return View();
        }
    }
}
