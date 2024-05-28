using API.Entities;

namespace API.DTOs
    {
    public class FeedbackDto
        {
        public int Id { get; set; }
        public int AppUserId { get; set; }
        public string userName { get; set; }
        public string content { get; set; }
        }
    }
