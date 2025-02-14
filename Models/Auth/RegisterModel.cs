using System.ComponentModel.DataAnnotations;
using B2B_API.Models.Enums;

namespace B2B_API.Models.Auth
{
    public class RegisterModel
    {
        [Required]
        [EmailAddress]
        public required string Email { get; set; }

        [Required]
        [MinLength(6)]
        public required string Password { get; set; }

        [Required]
        public UserRole UserRole { get; set; }

        [Required]
        public UserType UserType { get; set; }
    }
} 