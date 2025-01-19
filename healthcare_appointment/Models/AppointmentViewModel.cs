namespace healthcare_appointment.Models
{
    public class AppointmentViewModel
    {
        public string FullName { get; set; }
        public string ContactNumber { get; set; }
        public DateTime AppointmentDate { get; set; }
        public TimeSpan AppointmentTime { get; set; }
        public string ServiceType { get; set; }
        public string DoctorId { get; set; }  // This will hold the selected doctor's ID
    }

}
