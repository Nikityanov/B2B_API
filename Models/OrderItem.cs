using System.ComponentModel.DataAnnotations.Schema;

namespace B2B_API.Models
{
    public class OrderItem
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public Order Order { get; set; } = null!; // Required navigation property

        public int ProductId { get; set; }
        public Product Product { get; set; } = null!; // Required navigation property

        public int Quantity { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Price { get; set; } // Price at the time of order
    }
}