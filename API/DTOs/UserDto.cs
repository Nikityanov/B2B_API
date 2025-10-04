using System.ComponentModel.DataAnnotations;
using B2B_API.Models.Enums;

namespace B2B_API.API.DTOs
{
    /// <summary>
    /// DTO для ответа API с полной информацией о пользователе
    /// </summary>
    public class UserDto
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public required string Name { get; set; }

        [Required]
        public UserType UserType { get; set; }

        [Required]
        public UserRole UserRole { get; set; }

        [Required]
        [StringLength(9, MinimumLength = 9)]
        [RegularExpression(@"^\d+$")]
        public required string UNP { get; set; }

        [StringLength(13, MinimumLength = 13)]
        [RegularExpression(@"^\d+$")]
        public string? OKPO { get; set; }

        [Required]
        [MaxLength(500)]
        public required string LegalAddress { get; set; }

        [Required]
        [MaxLength(500)]
        public required string ActualAddress { get; set; }

        [Required]
        [MaxLength(100)]
        [EmailAddress]
        public required string Email { get; set; }

        [Required]
        [MaxLength(20)]
        [Phone]
        public required string Phone { get; set; }

        [MaxLength(50)]
        public string? BankName { get; set; }

        [MaxLength(20)]
        public string? BankAccount { get; set; }

        [MaxLength(9)]
        public string? BankBIK { get; set; }

        [MaxLength(12)]
        public string? INN { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }

        public ICollection<OrderDto> Orders { get; set; } = new List<OrderDto>();
    }
}