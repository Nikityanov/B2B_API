using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using B2B_API.Application.Queries;
using B2B_API.Domain.Entities;
using B2B_API.Data;

namespace B2B_API.Application.Handlers
{
    /// <summary>
    /// Обработчик запроса получения продукта по ID
    /// </summary>
    public class GetProductByIdQueryHandler : BaseQueryHandler<GetProductByIdQuery, ProductDto>
    {
        private readonly ApplicationDbContext _context;

        public GetProductByIdQueryHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public override async Task<Result<ProductDto>> Handle(GetProductByIdQuery query, CancellationToken cancellationToken)
        {
            try
            {
                var product = await _context.Products
                    .Include(p => p.Category)
                    .FirstOrDefaultAsync(p => p.Id == query.Id, cancellationToken);

                if (product == null)
                    return Result.Fail($"Продукт с ID {query.Id} не найден");

                var productDto = MapToDto(product);
                return Result.Ok(productDto);
            }
            catch (Exception ex)
            {
                return Result.Fail($"Ошибка при получении продукта: {ex.Message}");
            }
        }

        private static ProductDto MapToDto(Product product)
        {
            return new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                StockQuantity = product.StockQuantity,
                Price = product.Price,
                SKU = product.SKU,
                Manufacturer = product.Manufacturer,
                Unit = product.Unit,
                ImageUrl = product.ImageUrl,
                ImageGallery = product.ImageGallery.ToList(),
                Characteristics = product.Characteristics,
                CategoryId = product.CategoryId,
                CategoryName = product.Category?.Name
            };
        }
    }
}
