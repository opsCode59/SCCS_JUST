using API.Entities;
using System.ComponentModel.DataAnnotations;

namespace API.DTOs
    {
    public class CreateScheduleDto
        {
        [Required]

        public string userName { get; set; }
        [Required]

        public DateTime scheduleDate { get; set; }
        [Required]

        public string clinicUserName { get; set; }

        }
    }
