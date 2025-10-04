using FluentResults;
using B2B_API.Application.Interfaces;
using B2B_API.API.DTOs;

namespace B2B_API.Application.Queries
{
    /// <summary>
    /// Запрос для получения заказа по ID
    /// </summary>
    public class GetOrderByIdQuery : IQuery<OrderDto>
    {
        public int Id { get; set; }
    }
}