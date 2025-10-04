using FluentResults;
using B2B_API.Application.Interfaces;
using B2B_API.Models.Enums;
using B2B_API.API.DTOs;

namespace B2B_API.Application.Commands
{
    /// <summary>
    /// Команда для создания нового заказа
    /// </summary>
    public class CreateOrderCommand : ICommand<int>
    {
        public required int CustomerId { get; set; }
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        public string? Notes { get; set; }
        public required ICollection<CreateOrderItemDto> OrderItems { get; set; } = new List<CreateOrderItemDto>();
    }
}