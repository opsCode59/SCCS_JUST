using API.Entities;
using System.ComponentModel.DataAnnotations;

namespace API.DTOs
    {
    public class CreateScheduleDto
        {
        [Required]

        public string userName { get; set; }
        [Required]

        public DateTime dateOfBirth { get; set; }
        [Required]

        public string clinicUserName { get; set; }

        }
    }
