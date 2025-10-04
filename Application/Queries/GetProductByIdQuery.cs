using FluentResults;
using B2B_API.Application.Interfaces;

namespace B2B_API.Application.Queries
{
    /// <summary>
    /// Запрос для получения продукта по ID
    /// </summary>
    public class GetProductByIdQuery : IQuery<ProductDto>
    {
        public int Id { get; set; }
    }
}
