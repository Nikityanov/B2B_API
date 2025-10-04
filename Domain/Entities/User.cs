using System.ComponentModel.DataAnnotations;
using B2B_API.Models.Enums;

namespace B2B_API.Domain.Entities
{
    /// <summary>
    /// Доменная сущность пользователя
    /// </summary>
    public class User : BaseEntity
    {
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

        [Required]
        public required string PasswordHash { get; set; }

        [MaxLength(12)]
        public string? INN { get; set; }

        // Навигационные свойства
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
        public virtual ICollection<PriceList> OwnedPriceLists { get; set; } = new List<PriceList>();
        public virtual ICollection<PriceList> AccessiblePriceLists { get; set; } = new List<PriceList>();
    }
}
