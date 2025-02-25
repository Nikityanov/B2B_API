using System.ComponentModel.DataAnnotations;

namespace B2B_API.Models
{
    public class Product
    {
        public Product()
        {
            PriceLists = new HashSet<PriceList>();
        }

        [Key]
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
        public required string SKU { get; set; }  // Артикул

        public string? Manufacturer { get; set; }

        public string? Unit { get; set; }  // Единица измерения

        public virtual ICollection<PriceList> PriceLists { get; set; }

        public string? ImageUrl { get; set; }
        public virtual IList<string> ImageGallery { get; set; } = new List<string>();

        public string? Characteristics { get; set; } // JSON or XML for product characteristics

        public int? CategoryId { get; set; } // Foreign key for Category
        public virtual Category? Category { get; set; } // Navigation property for Category
    }
}