using FluentResults;
using B2B_API.Application.Interfaces;

namespace B2B_API.Application.Queries
{
    /// <summary>
    /// DTO для прайс-листа в ответе
    /// </summary>
    public class PriceListListDto
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public required string Currency { get; set; }
        public int SellerId { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public int ProductsCount { get; set; }
    }

    /// <summary>
    /// Запрос для получения списка прайс-листов с пагинацией и фильтрацией
    /// </summary>
    public class GetPriceListsQuery : IQuery<(List<PriceListListDto> Items, int TotalCount)>
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SearchTerm { get; set; }
        public string? Currency { get; set; }
        public bool? IsActive { get; set; }
        public int? SellerId { get; set; }
    }
}