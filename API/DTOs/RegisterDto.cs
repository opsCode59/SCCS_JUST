using System.ComponentModel.DataAnnotations;

namespace API.DTOs;
    public class RegisterDto
        {
    [Required]
    public string username { get; set; }
    
    [Required]
    public string  knownAs { get; set; }
   
    public string? gender { get; set; }
    [Required]
    public DateTime dateOfBirth { get; set; }

    [Required]
    public string country { get; set; }
    
    [Required]
    public string city { get; set; }
   

    [Required]
    [StringLength(20,MinimumLength =6)]
    public string password { get; set; }
        }

