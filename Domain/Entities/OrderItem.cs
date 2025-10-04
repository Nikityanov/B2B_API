using System.ComponentModel.DataAnnotations;

namespace B2B_API.Domain.Entities
{
    /// <summary>
    /// Доменная сущность элемента заказа
    /// </summary>
    public class OrderItem : BaseEntity
    {
        [Required]
        public int OrderId { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        public decimal UnitPrice { get; set; }

        [Required]
        public decimal TotalPrice { get; set; }

        // Навигационные свойства
        public virtual Order Order { get; set; } = null!;
        public virtual Product Product { get; set; } = null!;
    }
}
