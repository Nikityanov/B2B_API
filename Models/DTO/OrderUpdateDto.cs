using B2B_API.Models.Enums;

namespace B2B_API.Models.DTO
{
    public class OrderUpdateDto
    {
        public OrderStatus? Status { get; set; }
    }
}