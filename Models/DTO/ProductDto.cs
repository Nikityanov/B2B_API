using System.ComponentModel.DataAnnotations;

namespace B2B_API.Models.DTO
{
    public class ProductCreateDto
    {
        [Required]
        [MaxLength(200)]
        public required string Name { get; set; }

        [MaxLength(1000)]
        public required string Description { get; set; }

        [Required]
        public int StockQuantity { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Price { get; set; }

        [Required]
        public required string SKU { get; set; }

        public string? Manufacturer { get; set; }
        public string? Unit { get; set; }

        public string? ImageUrl { get; set; }
        public ICollection<string>? ImageGallery { get; set; }
        public string? Characteristics { get; set; }
        public int? CategoryId { get; set; }
    }

    public class ProductUpdateDto
    {
        [Required]
        [MaxLength(200)]
        public required string Name { get; set; }

        [MaxLength(1000)]
        public required string Description { get; set; }

        [Required]
        public int StockQuantity { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Price { get; set; }

        public string? Manufacturer { get; set; }
        public string? Unit { get; set; }

        public string? ImageUrl { get; set; }
        public ICollection<string>? ImageGallery { get; set; }
        public string? Characteristics { get; set; }
        public int? CategoryId { get; set; }
    }

    public class ProductResponseDto
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public int StockQuantity { get; set; }
        public decimal Price { get; set; }
        public required string SKU { get; set; }
        public string? Manufacturer { get; set; }
        public string? Unit { get; set; }

        public string? ImageUrl { get; set; }
        public ICollection<string>? ImageGallery { get; set; }
        public string? Characteristics { get; set; }
        public int? CategoryId { get; set; }
    }
}