namespace API.Entities
    {
    public class Feedback
        {
        public int Id { get; set; }
        public AppUser AppUser { get; set; }
        public int AppUserId { get; set; }
        public string userName { get; set; }
        public string content { get; set; }
        }
    }
