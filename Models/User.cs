using System.ComponentModel.DataAnnotations;
using B2B_API.Models.Enums;

namespace B2B_API.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public required string Name { get; set; }

        [Required]
        public UserType UserType { get; set; }

        [Required]
        public UserRole UserRole { get; set; }

        // Для Республики Беларусь:
        [Required]
        [StringLength(9, MinimumLength = 9, ErrorMessage = "INN must contain exactly 9 digits")]
        [RegularExpression(@"^\d+$", ErrorMessage = "INN must consist only of digits")]
        public required string UNP { get; set; }

        [StringLength(13, MinimumLength = 13, ErrorMessage = "OKPO must contain exactly 13 digits")]
        [RegularExpression(@"^\d+$", ErrorMessage = "OKPO must consist only of digits")]
        public string? OKPO { get; set; }  // Общегосударственный классификатор предприятий

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
        [Phone(ErrorMessage = "Некорректный формат номера телефона")]
        public required string Phone { get; set; }

        [MaxLength(50)]
        public string? BankName { get; set; }

        [MaxLength(20)]
        public string? BankAccount { get; set; }

        [MaxLength(9)]
        public string? BankBIK { get; set; }

        [Required]
        public byte[] PasswordHash { get; set; } = Array.Empty<byte>();
        public byte[] PasswordSalt { get; set; } = Array.Empty<byte>();

        public virtual ICollection<PriceList> OwnedPriceLists { get; set; } = new List<PriceList>();
        public virtual ICollection<PriceList> AccessiblePriceLists { get; set; } = new List<PriceList>();

        [MaxLength(12)]
        public string? INN { get; set; }
    }
}