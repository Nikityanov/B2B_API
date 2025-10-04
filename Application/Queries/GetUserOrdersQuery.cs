using FluentResults;
using B2B_API.Application.Interfaces;
using B2B_API.API.DTOs;

namespace B2B_API.Application.Queries
{
    /// <summary>
    /// Запрос для получения заказов пользователя
    /// </summary>
    public class GetUserOrdersQuery : IQuery<List<OrderDto>>
    {
        public required int CustomerId { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}