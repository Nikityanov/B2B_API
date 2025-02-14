using System.ComponentModel.DataAnnotations;

namespace B2B_API.Models.Auth
{
    public class LoginModel
    {
        [Required]
        [EmailAddress]
        public required string Email { get; set; }

        [Required]
        public required string Password { get; set; }
    }
} 