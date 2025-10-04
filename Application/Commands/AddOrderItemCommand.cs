using FluentResults;
using B2B_API.Application.Interfaces;
using B2B_API.API.DTOs;

namespace B2B_API.Application.Commands
{
    /// <summary>
    /// Команда для добавления товара в заказ
    /// </summary>
    public class AddOrderItemCommand : ICommand<int>
    {
        public required int OrderId { get; set; }
        public required CreateOrderItemDto OrderItem { get; set; } = new CreateOrderItemDto();
    }
}