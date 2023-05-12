using System.ComponentModel.DataAnnotations;

namespace BetMe.Models
{
    public class UserRegistrationDto
    {
        [Required, MaxLength(64), MinLength(3)]
        public string Name { get; set; } = string.Empty;
        [Required, MaxLength(128), EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Required, MaxLength(64), MinLength(8)]
        public string Password { get; set; } = string.Empty;
    }
}