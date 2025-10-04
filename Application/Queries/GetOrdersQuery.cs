using FluentResults;
using B2B_API.Application.Interfaces;
using B2B_API.Models.Enums;
using B2B_API.API.DTOs;

namespace B2B_API.Application.Queries
{
    /// <summary>
    /// Запрос для получения списка всех заказов с пагинацией и фильтрацией
    /// </summary>
    public class GetOrdersQuery : IQuery<(List<OrderDto> Items, int TotalCount)>
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int? CustomerId { get; set; }
        public OrderStatus? Status { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}