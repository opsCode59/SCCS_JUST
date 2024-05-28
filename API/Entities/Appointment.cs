namespace API.Entities
    {
    public class Appointment
        {
        public int Id { get; set; }
        public string userName { get; set; }
        public AppUser User { get; set; }
        public int AppUserId { get; set; }
        public string clinicUsername { get; set; }
        public bool IsApproved { get; set; }

        }
    }
