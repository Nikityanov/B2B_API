using System.ComponentModel.DataAnnotations;
using B2B_API.Models.Enums;

namespace B2B_API.Domain.Entities
{
    /// <summary>
    /// Доменная сущность заказа
    /// </summary>
    public class Order : BaseEntity
    {
        [Required]
        public int CustomerId { get; set; }

        [Required]
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        [Required]
        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        [Required]
        public decimal TotalAmount { get; set; }

        public string? Notes { get; set; }

        // Навигационные свойства
        public virtual User Customer { get; set; } = null!;
        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
