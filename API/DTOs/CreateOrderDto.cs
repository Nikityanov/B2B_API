using System.ComponentModel.DataAnnotations;
using B2B_API.Models.Enums;

namespace B2B_API.API.DTOs
{
    /// <summary>
    /// DTO для создания нового заказа
    /// </summary>
    public class CreateOrderDto
    {
        [Required]
        public int CustomerId { get; set; }

        [Required]
        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        public string? Notes { get; set; }

        [Required]
        [MinLength(1)]
        public ICollection<CreateOrderItemDto> OrderItems { get; set; } = new List<CreateOrderItemDto>();
    }
}