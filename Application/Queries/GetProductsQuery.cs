using FluentResults;
using B2B_API.Application.Interfaces;

namespace B2B_API.Application.Queries
{
    /// <summary>
    /// DTO для продукта в ответе
    /// </summary>
    public class ProductDto
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public int StockQuantity { get; set; }
        public decimal Price { get; set; }
        public required string SKU { get; set; }
        public string? Manufacturer { get; set; }
        public string? Unit { get; set; }
        public string? ImageUrl { get; set; }
        public List<string>? ImageGallery { get; set; }
        public string? Characteristics { get; set; }
        public int? CategoryId { get; set; }
        public string? CategoryName { get; set; }
    }

    /// <summary>
    /// Запрос для получения списка продуктов с пагинацией и фильтрацией
    /// </summary>
    public class GetProductsQuery : IQuery<(List<ProductDto> Items, int TotalCount)>
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int? CategoryId { get; set; }
        public string? SearchTerm { get; set; }
    }
}
