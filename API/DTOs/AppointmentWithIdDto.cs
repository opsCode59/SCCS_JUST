namespace API.DTOs
    {
    public class AppointmentWithIdDto
        {
        public int Id { get; set; }
        public string userName { get; set; }
        public string clinicUsername { get; set; }
        public int? appUserId { get; set; }
        public bool isApproved { get; set; }
        }
    }
