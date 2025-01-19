using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace healthcare_appointment.Models
{
    public class Appointment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string FullName { get; set; }

        [Required]
        [Phone]
        public string ContactNumber { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime AppointmentDate { get; set; }

        [Required]
        [DataType(DataType.Time)]
        public TimeSpan AppointmentTime { get; set; }

        [Required]
        public string ServiceType { get; set; } // E.g., "Dentist", "Cardiology", etc.

        [Required]
        public string DoctorId { get; set; } // Reference to the doctor

        [ForeignKey("DoctorId")]
        public ApplicationUser Doctor { get; set; } // Navigation property for the doctor
    }
}
