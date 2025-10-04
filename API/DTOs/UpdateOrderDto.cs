using System.ComponentModel.DataAnnotations;
using B2B_API.Models.Enums;

namespace B2B_API.API.DTOs
{
    /// <summary>
    /// DTO для обновления заказа
    /// </summary>
    public class UpdateOrderDto
    {
        public int Id { get; set; }

        [Required]
        public int CustomerId { get; set; }

        [Required]
        public OrderStatus Status { get; set; }

        public string? Notes { get; set; }
    }
}