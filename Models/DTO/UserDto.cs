using System.ComponentModel.DataAnnotations;
using B2B_API.Models.Enums;

namespace B2B_API.Models.DTO
{
    public class UserShortInfoDto
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Email { get; set; }
        public UserType UserType { get; set; }
        public UserRole UserRole { get; set; }
        public required string INN { get; set; }
    }

    public class UserProfileUpdateDto
    {
        [Required]
        [MaxLength(200)]
        public required string Name { get; set; }

        [Required]
        [MaxLength(12)]
        public required string INN { get; set; }


        [Required]
        [MaxLength(200)]
        public required string LegalAddress { get; set; }

        [Required]
        [MaxLength(200)]
        public required string ActualAddress { get; set; }

        [Required]
        [MaxLength(20)]
        public required string Phone { get; set; }

        [MaxLength(50)]
        public string? BankName { get; set; }

        [MaxLength(20)]
        public string? BankAccount { get; set; }

        [MaxLength(100)]
        [EmailAddress]
        public required string Email { get; set; }

        [MaxLength(9)]
        public string? BankBIK { get; set; }
    }

    public class UserProfileResponseDto
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public UserType UserType { get; set; }
        public UserRole UserRole { get; set; }
        public required string INN { get; set; }
        public required string LegalAddress { get; set; }
        public required string ActualAddress { get; set; }
        public required string Email { get; set; }
        public required string Phone { get; set; }
        public string? BankName { get; set; }
        public string? BankAccount { get; set; }
        public string? BankBIK { get; set; }
        public string? OKPO { get; set; }
        public required string UNP { get; set; }
        public bool IsProfileComplete { get; set; }
    }
}