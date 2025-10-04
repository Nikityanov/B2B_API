using System.ComponentModel.DataAnnotations;

namespace B2B_API.API.DTOs
{
    /// <summary>
    /// DTO для ответа API с полной информацией о продукте
    /// </summary>
    public class ProductDto
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public required string Name { get; set; }

        [MaxLength(1000)]
        public string? Description { get; set; }

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
        public IList<string> ImageGallery { get; set; } = new List<string>();
        public string? Characteristics { get; set; }

        public int? CategoryId { get; set; }
        public CategoryDto? Category { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
    }
}