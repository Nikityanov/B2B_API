using System.ComponentModel.DataAnnotations;

namespace B2B_API.Domain.Entities
{
    /// <summary>
    /// Доменная сущность продукта
    /// </summary>
    public class Product : BaseEntity
    {
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
        public virtual IList<string> ImageGallery { get; set; } = new List<string>();
        public string? Characteristics { get; set; }

        public int? CategoryId { get; set; }
        public virtual Category? Category { get; set; }

        // Навигационные свойства для связи с прайс-листами
        public virtual ICollection<PriceListProduct> PriceListProducts { get; set; } = new List<PriceListProduct>();
    }
}
