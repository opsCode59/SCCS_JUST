using API.Entities;
using System.ComponentModel.DataAnnotations;

namespace API.DTOs
    {
    public class AppointmentDto
        {
        public string userName { get; set; }
        public string clinicUsername { get; set; }
        public int? appUserId { get; set; }
        public bool isApproved { get; set; }
        }
    }
