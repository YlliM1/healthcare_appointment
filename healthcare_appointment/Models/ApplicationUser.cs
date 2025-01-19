using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace healthcare_appointment.Models // Ensure this namespace matches your project's structure
{
    public class ApplicationUser : IdentityUser
    {
        [Required, MaxLength(50)]
        public string FirstName { get; set; }

        [Required, MaxLength(50)]
        public string LastName { get; set; }

        public bool IsDoctor { get; set; }

        [MaxLength(100)]
        public string? ServiceSpecialization { get; set; }
    }
}
