using FluentResults;
using B2B_API.Application.Interfaces;
using B2B_API.API.DTOs;

namespace B2B_API.Application.Queries
{
    /// <summary>
    /// Запрос для получения продуктов в прайс-листе
    /// </summary>
    public class GetPriceListProductsQuery : IQuery<(List<PriceListProductDto> Items, int TotalCount)>
    {
        public int PriceListId { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public bool? IsActive { get; set; }
        public string? SearchTerm { get; set; }
    }
}