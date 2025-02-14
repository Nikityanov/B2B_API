using System.ComponentModel.DataAnnotations;

namespace B2B_API.Models.DTO
{
    public class PriceListCreateDto
    {
        [Required]
        [MaxLength(200)]
        public required string Name { get; set; }
    }

    public class PriceListUpdateDto
    {
        [Required]
        [MaxLength(200)]
        public required string Name { get; set; }

        public bool IsActive { get; set; }
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