using System.ComponentModel.DataAnnotations;

namespace B2B_API.Domain.Entities
{
    /// <summary>
    /// Промежуточная сущность для связи многие-ко-многим между PriceList и Product
    /// </summary>
    public class PriceListProduct : BaseEntity
    {
        [Required]
        public int PriceListId { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Required]
        public decimal SpecialPrice { get; set; }

        public bool IsActive { get; set; } = true;

        // Навигационные свойства
        public virtual PriceList PriceList { get; set; } = null!;
        public virtual Product Product { get; set; } = null!;
    }
}
