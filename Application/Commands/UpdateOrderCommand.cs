using FluentResults;
using B2B_API.Application.Interfaces;
using B2B_API.Models.Enums;

namespace B2B_API.Application.Commands
{
    /// <summary>
    /// Команда для обновления заказа
    /// </summary>
    public class UpdateOrderCommand : ICommand
    {
        public int Id { get; set; }
        public required int CustomerId { get; set; }
        public OrderStatus Status { get; set; }
        public string? Notes { get; set; }
    }
}