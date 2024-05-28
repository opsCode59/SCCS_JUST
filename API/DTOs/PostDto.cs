namespace API.DTOs
    {
    public class PostDto
        {
        public int Id { get; set; }
        public string userName { get; set; }
        public DateTime created { get; set; } = DateTime.UtcNow;
        public string description { get; set; }
        public string senderUserName { get; set; }

        }
    }
