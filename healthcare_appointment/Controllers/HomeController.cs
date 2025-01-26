using System.Diagnostics;
using System;
using System.Linq;
using healthcare_appointment.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

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

        public IActionResult Index(string searchTerm)
        {
            var doctorsQuery = _context.Users
                .Where(u => u.IsDoctor);

            if (!string.IsNullOrEmpty(searchTerm))
            {
                doctorsQuery = doctorsQuery.Where(u => u.FirstName.Contains(searchTerm) || u.LastName.Contains(searchTerm));
            }

            var doctors = doctorsQuery.ToList();
            ViewBag.Doctors = doctors;

            return View();
        }



        public IActionResult AppointmentOverview(int page = 1, int pageSize = 10)
        {
            // Ensure the user is authenticated
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account", new { area = "Identity", returnUrl = "/Home/AppointmentOverview" });
            }

            // Retrieve appointments with pagination
            var totalAppointments = _context.Appointments.Count();
            var appointments = _context.Appointments
                .OrderBy(a => a.AppointmentDate)
                .ThenBy(a => a.AppointmentTime)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // Map DoctorId to DoctorName
            var doctorNames = _context.Users
                .Where(u => u.IsDoctor)
                .ToDictionary(u => u.Id, u => $"{u.FirstName} {u.LastName}");
            ViewBag.DoctorNames = doctorNames;

            // Pass pagination data to the view
            ViewBag.Pagination = new AppointmentsPagination
            {
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling((double)totalAppointments / pageSize)
            };

            return View(appointments.Select(a => new AppointmentViewModel
            {
                FullName = a.FullName,
                ContactNumber = a.ContactNumber,
                AppointmentDate = a.AppointmentDate,
                AppointmentTime = a.AppointmentTime,
                ServiceType = a.ServiceType,
                DoctorId = a.DoctorId
            }).ToList());
        }


        public IActionResult DeleteAppointment(string fullName, string contactNumber)
        {
            if (string.IsNullOrEmpty(fullName) || string.IsNullOrEmpty(contactNumber))
            {
                return Json(new { success = false, message = "Full name and contact number are required." });
            }

            try
            {
                // Search for the appointment based on fullName and contactNumber
                var appointment = _context.Appointments
                    .FirstOrDefault(a => a.FullName == fullName && a.ContactNumber == contactNumber);

                if (appointment == null)
                {
                    return Json(new { success = false, message = "Appointment not found." });
                }

                // Remove the appointment from the database
                _context.Appointments.Remove(appointment);
                _context.SaveChanges();

                return Json(new { success = true, message = "Appointment successfully deleted." });
            }
            catch (Exception ex)
            {
                // Log the error for debugging purposes
                _logger.LogError(ex, "Error deleting appointment.");
                return Json(new { success = false, message = "An error occurred while deleting the appointment." });
            }
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

        // Dentist Appointment
        [HttpGet]
        public IActionResult Dentist(string specialization)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account", new { area = "Identity", returnUrl = "/Home/Dentist" });
            }

            var doctorsQuery = _context.Users
                .Where(u => u.IsDoctor && u.ServiceSpecialization == "Dentist");

            if (!string.IsNullOrEmpty(specialization))
            {
                doctorsQuery = doctorsQuery.Where(u => u.ServiceSpecialization.Contains(specialization));
            }

            var doctors = doctorsQuery.ToList();
            ViewBag.Specializations = new List<string> { "Dentist", "General Checkup", "Cardiology", "Dermatology", "Pediatrics" };
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

        // General Check-Up Appointment
        [HttpGet]
        public IActionResult GeneralCheckup(string specialization)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account", new { area = "Identity", returnUrl = "/Home/GeneralCheckup" });
            }

            var doctorsQuery = _context.Users
                .Where(u => u.IsDoctor && u.ServiceSpecialization == "General Checkup");

            if (!string.IsNullOrEmpty(specialization))
            {
                doctorsQuery = doctorsQuery.Where(u => u.ServiceSpecialization.Contains(specialization));
            }

            var doctors = doctorsQuery.ToList();
            ViewBag.Specializations = new List<string> { "Dentist", "General Checkup", "Cardiology", "Dermatology", "Pediatrics" };
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

                TempData["AppointmentSuccess"] = "Your appointment has been booked successfully!";
                return RedirectToAction("Appointments", "Home");
            }

            var doctors = _context.Users
                .Where(u => u.IsDoctor && u.ServiceSpecialization == "General Check-Up")
                .ToList();
            ViewBag.Doctors = doctors;

            return View("GeneralCheckup", model);
        }

        // Cardiology Appointment
        [HttpGet]
        public IActionResult Cardiology(string specialization)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account", new { area = "Identity", returnUrl = "/Home/Cardiology" });
            }

            var doctorsQuery = _context.Users
                .Where(u => u.IsDoctor && u.ServiceSpecialization == "Cardiology");

            if (!string.IsNullOrEmpty(specialization))
            {
                doctorsQuery = doctorsQuery.Where(u => u.ServiceSpecialization.Contains(specialization));
            }

            var doctors = doctorsQuery.ToList();
            ViewBag.Specializations = new List<string> { "Dentist", "General Checkup", "Cardiology", "Dermatology", "Pediatrics" };
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

                TempData["AppointmentSuccess"] = "Your appointment has been booked successfully!";
                return RedirectToAction("Appointments", "Home");
            }

            var doctors = _context.Users
                .Where(u => u.IsDoctor && u.ServiceSpecialization == "Cardiology")
                .ToList();
            ViewBag.Doctors = doctors;

            return View("Cardiology", model);
        }

        // Dermatology Appointment
        [HttpGet]
        public IActionResult Dermatology(string specialization)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account", new { area = "Identity", returnUrl = "/Home/Dermatology" });
            }

            var doctorsQuery = _context.Users
                .Where(u => u.IsDoctor && u.ServiceSpecialization == "Dermatology");

            if (!string.IsNullOrEmpty(specialization))
            {
                doctorsQuery = doctorsQuery.Where(u => u.ServiceSpecialization.Contains(specialization));
            }

            var doctors = doctorsQuery.ToList();
            ViewBag.Specializations = new List<string> { "Dentist", "General Checkup", "Cardiology", "Dermatology", "Pediatrics" };
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

                TempData["AppointmentSuccess"] = "Your appointment has been booked successfully!";
                return RedirectToAction("Appointments", "Home");
            }

            var doctors = _context.Users
                .Where(u => u.IsDoctor && u.ServiceSpecialization == "Dermatology")
                .ToList();
            ViewBag.Doctors = doctors;

            return View("Dermatology", model);
        }

        // Pediatrics Appointment
        [HttpGet]
        public IActionResult Pediatrics(string specialization)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account", new { area = "Identity", returnUrl = "/Home/Pediatrics" });
            }

            var doctorsQuery = _context.Users
                .Where(u => u.IsDoctor && u.ServiceSpecialization == "Pediatrics");

            if (!string.IsNullOrEmpty(specialization))
            {
                doctorsQuery = doctorsQuery.Where(u => u.ServiceSpecialization.Contains(specialization));
            }

            var doctors = doctorsQuery.ToList();
            ViewBag.Specializations = new List<string> { "Dentist", "General Checkup", "Cardiology", "Dermatology", "Pediatrics" };
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

                TempData["AppointmentSuccess"] = "Your appointment has been booked successfully!";
                return RedirectToAction("Appointments", "Home");
            }

            var doctors = _context.Users
                .Where(u => u.IsDoctor && u.ServiceSpecialization == "Pediatrics")
                .ToList();
            ViewBag.Doctors = doctors;

            return View("Pediatrics", model);
        }

        [HttpGet]
        public IActionResult ContactUs()
        {
            TempData.Remove("SuccessMessage");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ContactUs(Contact model)
        {
            if (ModelState.IsValid)
            {
                var contact = new Contact
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    PhoneNumber = model.PhoneNumber,
                    Description = model.Description,
                    CreatedAt = DateTime.Now
                };

                _context.Contacts.Add(contact);
                _context.SaveChanges();

                TempData["ContactSuccessMessage"] = "Thank you for contacting us. We will get back to you shortly.";
                return RedirectToAction("ContactUs");
            }

            return View(model);
        }

        public IActionResult AboutUs()
        {
            return View();
        }

        public IActionResult Services()
        {
            return View();
        }
    }
}
