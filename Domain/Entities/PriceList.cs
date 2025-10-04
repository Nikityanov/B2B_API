using System.ComponentModel.DataAnnotations;

namespace B2B_API.Domain.Entities
{
    /// <summary>
    /// Доменная сущность прайс-листа
    /// </summary>
    public class PriceList : BaseEntity
    {
        [Required]
        [MaxLength(200)]
        public required string Name { get; set; }

        [MaxLength(1000)]
        public string? Description { get; set; }

        [Required]
        public required string Currency { get; set; }

        [Required]
        public int SellerId { get; set; }

        public bool IsActive { get; set; } = true;

        // Навигационные свойства
        public virtual User Seller { get; set; } = null!;
        public virtual ICollection<User> AllowedBuyers { get; set; } = new List<User>();
        public virtual ICollection<PriceListProduct> PriceListProducts { get; set; } = new List<PriceListProduct>();
    }
}
