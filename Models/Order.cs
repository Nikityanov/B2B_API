using System;
using System.Collections.Generic;
using B2B_API.Models.Enums;

namespace B2B_API.Models
{
    public class Order
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        public int CustomerId { get; set; }
        public User Customer { get; set; } = null!; // Required navigation property

        public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}