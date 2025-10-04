using System.ComponentModel.DataAnnotations;

namespace B2B_API.API.DTOs
{
    /// <summary>
    /// DTO для обновления информации о продукте
    /// </summary>
    public class UpdateProductDto
    {
        [MaxLength(200)]
        public string? Name { get; set; }

        [MaxLength(1000)]
        public string? Description { get; set; }

        public int? StockQuantity { get; set; }

        [Range(0.01, double.MaxValue)]
        public decimal? Price { get; set; }

        public string? SKU { get; set; }

        public string? Manufacturer { get; set; }
        public string? Unit { get; set; }
        public string? ImageUrl { get; set; }
        public IList<string>? ImageGallery { get; set; }
        public string? Characteristics { get; set; }

        public int? CategoryId { get; set; }
    }
}