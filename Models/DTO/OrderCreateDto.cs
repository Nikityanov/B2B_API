using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace B2B_API.Models.DTO
{
    public class OrderCreateDto
    {
        [Required]
        public int CustomerId { get; set; }

        public List<OrderItemCreateDto> OrderItems { get; set; } = new List<OrderItemCreateDto>();
    }

    public class OrderItemCreateDto
    {
        [Required]
        public int ProductId { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }
    }
}