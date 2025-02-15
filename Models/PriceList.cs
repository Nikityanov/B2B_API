using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace B2B_API.Models
{
    public class PriceList
    {
        public PriceList()
        {
            AllowedBuyers = new HashSet<User>();
            Products = new HashSet<PriceListProduct>();
        }

        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public required string Name { get; set; }

        [Required]
        public int SellerId { get; set; }

        [ForeignKey("SellerId")]
        public virtual required User Seller { get; set; }

        public virtual ICollection<User> AllowedBuyers { get; set; }
        public virtual ICollection<PriceListProduct> Products { get; set; }

        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        [MaxLength(1000)]
        public string? Description { get; set; }

        public string? PriceListType { get; set; } // Тип прайс-листа (например, "Оптовый", "Розница", "Специальный")
        public string? Currency { get; set; } = "BYN"; // Валюта прайс-листа, по умолчанию - BYN
    }
    
    public class PriceListProduct
    {
        public int PriceListId { get; set; }
        public int ProductId { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal SpecialPrice { get; set; }

        public virtual required PriceList PriceList { get; set; }
        public virtual required Product Product { get; set; }
    }
}