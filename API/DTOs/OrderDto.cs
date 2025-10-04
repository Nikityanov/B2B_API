using System.ComponentModel.DataAnnotations;
using B2B_API.Models.Enums;

namespace B2B_API.API.DTOs
{
    /// <summary>
    /// DTO для ответа API с полной информацией о заказе
    /// </summary>
    public class OrderDto
    {
        public int Id { get; set; }

        [Required]
        public string OrderNumber { get; set; } = null!;

        [Required]
        public int CustomerId { get; set; }

        public string? CustomerName { get; set; }

        [Required]
        public DateTime OrderDate { get; set; }

        [Required]
        public OrderStatus Status { get; set; }

        [Required]
        public decimal TotalAmount { get; set; }

        public string? Notes { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? ModifiedAt { get; set; }

        public ICollection<OrderItemDto> OrderItems { get; set; } = new List<OrderItemDto>();
    }
}