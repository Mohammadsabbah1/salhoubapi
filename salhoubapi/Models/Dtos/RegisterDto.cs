using System.ComponentModel.DataAnnotations;

namespace salhoubapi.Models.Dtos
{
    public class RegisterDto
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string PhoneNumber { get; set; } = string.Empty;
        [Required]
        public float Percantage { get; set; } = 0;
        [Required]
        public string Role { get; set; }
    }
}
