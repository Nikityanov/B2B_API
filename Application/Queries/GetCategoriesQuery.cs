using FluentResults;
using B2B_API.Application.Interfaces;

namespace B2B_API.Application.Queries
{
    /// <summary>
    /// DTO для категории в ответе
    /// </summary>
    public class CategoryDto
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
    }

    /// <summary>
    /// Запрос для получения списка категорий с пагинацией
    /// </summary>
    public class GetCategoriesQuery : IQuery<(List<CategoryDto> Items, int TotalCount)>
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
