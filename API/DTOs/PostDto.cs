namespace API.DTOs
    {
    public class PostDto
        {
        public int Id { get; set; }
        public DateTime created { get; set; } = DateTime.UtcNow;
        public string description { get; set; }
        public string? url { get; set; }
        public string senderUserName { get; set; }

        }
    }
