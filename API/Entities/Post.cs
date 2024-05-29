namespace API.Entities
    {
    public class Post
        {
        public int Id { get; set; }
        public string SenderUsername { get; set; }
        public DateTime created { get; set; } = DateTime.UtcNow;
        public string description { get; set; }

        }
    }
