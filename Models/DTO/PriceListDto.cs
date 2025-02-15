using System.ComponentModel.DataAnnotations;

namespace B2B_API.Models.DTO
{
    public class PriceListCreateDto
    {
        [Required]
        [MaxLength(200)]
        public required string Name { get; set; }

        [MaxLength(1000)]
        public string? Description { get; set; }

        public string? PriceListType { get; set; }
        public string? Currency { get; set; } = "BYN"; // Default value
    }

    public class PriceListUpdateDto
    {
        [Required]
        [MaxLength(200)]
        public required string Name { get; set; }

        public bool IsActive { get; set; }

        [MaxLength(1000)]
        public string? Description { get; set; }

        [RegularExpression(@"^[a-zA-Z0-9\s-]+$", ErrorMessage = "Price list type can contain only letters, numbers, spaces and hyphens")]
        public string? PriceListType { get; set; }
        [RegularExpression(@"^[A-Z]{3}$", ErrorMessage = "Currency must be a 3-letter ISO 4217 currency code")]
        public string? Currency { get; set; }
    }

    public class PriceListResponseDto
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public int SellerId { get; set; }
        public required string SellerName { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public required ICollection<PriceListProductResponseDto> Products { get; set; }
        public required ICollection<UserShortInfoDto> AllowedBuyers { get; set; }

        public string? Description { get; set; }
        public string? PriceListType { get; set; }
        public string? Currency { get; set; }
    }

    public class PriceListProductCreateDto
    {
        [Required]
        public int ProductId { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal SpecialPrice { get; set; }
    }

    public class PriceListProductResponseDto
    {
        public int ProductId { get; set; }
        public required string ProductName { get; set; }
        public required string ProductSKU { get; set; }
        public decimal SpecialPrice { get; set; }
        public decimal RegularPrice { get; set; }
    }
}