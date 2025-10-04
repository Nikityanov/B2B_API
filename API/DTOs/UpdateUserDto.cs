using System.ComponentModel.DataAnnotations;
using B2B_API.Models.Enums;

namespace B2B_API.API.DTOs
{
    /// <summary>
    /// DTO для обновления информации о пользователе
    /// </summary>
    public class UpdateUserDto
    {
        [MaxLength(200)]
        public string? Name { get; set; }

        public UserType? UserType { get; set; }

        public UserRole? UserRole { get; set; }

        [StringLength(9, MinimumLength = 9)]
        [RegularExpression(@"^\d+$")]
        public string? UNP { get; set; }

        [StringLength(13, MinimumLength = 13)]
        [RegularExpression(@"^\d+$")]
        public string? OKPO { get; set; }

        [MaxLength(500)]
        public string? LegalAddress { get; set; }

        [MaxLength(500)]
        public string? ActualAddress { get; set; }

        [MaxLength(100)]
        [EmailAddress]
        public string? Email { get; set; }

        [MaxLength(20)]
        [Phone]
        public string? Phone { get; set; }

        [MinLength(6)]
        public string? Password { get; set; }

        [MaxLength(50)]
        public string? BankName { get; set; }

        [MaxLength(20)]
        public string? BankAccount { get; set; }

        [MaxLength(9)]
        public string? BankBIK { get; set; }

        [MaxLength(12)]
        public string? INN { get; set; }
    }
}