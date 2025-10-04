using FluentResults;
using B2B_API.Application.Interfaces;

namespace B2B_API.Application.Queries
{
    /// <summary>
    /// Запрос для получения категории по ID
    /// </summary>
    public class GetCategoryByIdQuery : IQuery<CategoryDto>
    {
        public int Id { get; set; }
    }
}
