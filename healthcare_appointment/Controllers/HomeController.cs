using System.Diagnostics;
using healthcare_appointment.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace healthcare_appointment.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
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
                return RedirectToAction("Login", "Account", new { area = "Identity", returnUrl = "/Home/Appointments" });
            }

            return View();
        }

        [HttpGet]
        public IActionResult Dentist()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account", new { area = "Identity", returnUrl = "/Home/Dentist" });
            }

            var doctors = _context.Users
                .Where(u => u.IsDoctor && u.ServiceSpecialization == "Dentist")
                .ToList();

            ViewBag.Doctors = doctors;

            return View();
        }

        [HttpPost]
        public IActionResult BookDentist(AppointmentViewModel model)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account", new { area = "Identity", returnUrl = "/Home/Dentist" });
            }

            if (ModelState.IsValid)
            {
                var appointment = new Appointment
                {
                    FullName = model.FullName,
                    ContactNumber = model.ContactNumber,
                    AppointmentDate = model.AppointmentDate,
                    AppointmentTime = model.AppointmentTime,
                    ServiceType = model.ServiceType,
                    DoctorId = model.DoctorId
                };

                _context.Appointments.Add(appointment);
                _context.SaveChanges();

                TempData["AppointmentSuccess"] = "Your appointment has been booked successfully!";
                return RedirectToAction("Appointments", "Home");
            }

            var doctors = _context.Users
                .Where(u => u.IsDoctor && u.ServiceSpecialization == "Dentist")
                .ToList();
            ViewBag.Doctors = doctors;

            return View("Dentist", model);
        }

        [HttpGet]
        public IActionResult GeneralCheckup()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account", new { area = "Identity", returnUrl = "/Home/GeneralCheckup" });
            }

            var doctors = _context.Users
                .Where(u => u.IsDoctor && u.ServiceSpecialization == "General Checkup")
                .ToList();

            ViewBag.Doctors = doctors;

            return View();
        }

        [HttpPost]
        public IActionResult BookGeneralCheckup(AppointmentViewModel model)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account", new { area = "Identity", returnUrl = "/Home/GeneralCheckup" });
            }

            if (ModelState.IsValid)
            {
                var appointment = new Appointment
                {
                    FullName = model.FullName,
                    ContactNumber = model.ContactNumber,
                    AppointmentDate = model.AppointmentDate,
                    AppointmentTime = model.AppointmentTime,
                    ServiceType = model.ServiceType,
                    DoctorId = model.DoctorId
                };

                _context.Appointments.Add(appointment);
                _context.SaveChanges();

                TempData["AppointmentSuccess"] = "Your general checkup appointment has been booked successfully!";
                return RedirectToAction("Appointments", "Home");
            }

            var doctors = _context.Users
                .Where(u => u.IsDoctor && u.ServiceSpecialization == "General Checkup")
                .ToList();
            ViewBag.Doctors = doctors;

            return View("GeneralCheckup", model);
        }

        [HttpGet]
        public IActionResult Pediatrics()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account", new { area = "Identity", returnUrl = "/Home/Pediatrics" });
            }

            var doctors = _context.Users
                .Where(u => u.IsDoctor && u.ServiceSpecialization == "Pediatrics")
                .ToList();

            ViewBag.Doctors = doctors;

            return View();
        }

        [HttpPost]
        public IActionResult BookPediatrics(AppointmentViewModel model)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account", new { area = "Identity", returnUrl = "/Home/Pediatrics" });
            }

            if (ModelState.IsValid)
            {
                var appointment = new Appointment
                {
                    FullName = model.FullName,
                    ContactNumber = model.ContactNumber,
                    AppointmentDate = model.AppointmentDate,
                    AppointmentTime = model.AppointmentTime,
                    ServiceType = model.ServiceType,
                    DoctorId = model.DoctorId
                };

                _context.Appointments.Add(appointment);
                _context.SaveChanges();

                TempData["AppointmentSuccess"] = "Your pediatrics appointment has been booked successfully!";
                return RedirectToAction("Appointments", "Home");
            }

            var doctors = _context.Users
                .Where(u => u.IsDoctor && u.ServiceSpecialization == "Pediatrics")
                .ToList();
            ViewBag.Doctors = doctors;

            return View("Pediatrics", model);
        }

        [HttpGet]
        public IActionResult Dermatology()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account", new { area = "Identity", returnUrl = "/Home/Dermatology" });
            }

            var doctors = _context.Users
                .Where(u => u.IsDoctor && u.ServiceSpecialization == "Dermatology")
                .ToList();

            ViewBag.Doctors = doctors;

            return View();
        }

        [HttpPost]
        public IActionResult BookDermatology(AppointmentViewModel model)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account", new { area = "Identity", returnUrl = "/Home/Dermatology" });
            }

            if (ModelState.IsValid)
            {
                var appointment = new Appointment
                {
                    FullName = model.FullName,
                    ContactNumber = model.ContactNumber,
                    AppointmentDate = model.AppointmentDate,
                    AppointmentTime = model.AppointmentTime,
                    ServiceType = model.ServiceType,
                    DoctorId = model.DoctorId
                };

                _context.Appointments.Add(appointment);
                _context.SaveChanges();

                TempData["AppointmentSuccess"] = "Your dermatology appointment has been booked successfully!";
                return RedirectToAction("Appointments", "Home");
            }

            var doctors = _context.Users
                .Where(u => u.IsDoctor && u.ServiceSpecialization == "Dermatology")
                .ToList();
            ViewBag.Doctors = doctors;

            return View("Dermatology", model);
        }

      


        [HttpGet]
        public IActionResult Cardiology()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account", new { area = "Identity", returnUrl = "/Home/Cardiology" });
            }

            var doctors = _context.Users
                .Where(u => u.IsDoctor && u.ServiceSpecialization == "Cardiology")
                .ToList();

            ViewBag.Doctors = doctors;

            return View();
        }

        [HttpPost]
        public IActionResult BookCardiology(AppointmentViewModel model)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account", new { area = "Identity", returnUrl = "/Home/Cardiology" });
            }

            if (ModelState.IsValid)
            {
                var appointment = new Appointment
                {
                    FullName = model.FullName,
                    ContactNumber = model.ContactNumber,
                    AppointmentDate = model.AppointmentDate,
                    AppointmentTime = model.AppointmentTime,
                    ServiceType = model.ServiceType,
                    DoctorId = model.DoctorId
                };

                _context.Appointments.Add(appointment);
                _context.SaveChanges();

                TempData["AppointmentSuccess"] = "Your cardiology appointment has been booked successfully!";
                return RedirectToAction("Appointments", "Home");
            }

            var doctors = _context.Users
                .Where(u => u.IsDoctor && u.ServiceSpecialization == "Cardiology")
                .ToList();
            ViewBag.Doctors = doctors;

            return View("Cardiology", model);
        }

        public IActionResult AboutUs()
        {
            return View();
        }

    }
}
