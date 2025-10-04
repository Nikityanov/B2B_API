using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;
using B2B_API.Application.Queries;
using B2B_API.Domain.Entities;
using B2B_API.Domain.Interfaces;
using B2B_API.Data;

namespace B2B_API.Application.Handlers
{
    /// <summary>
    /// Обработчик запроса получения списка продуктов с пагинацией и фильтрацией
    /// </summary>
    public class GetProductsQueryHandler : BasePagedQueryHandler<GetProductsQuery, ProductDto, List<ProductDto>>
    {
        private readonly ApplicationDbContext _context;

        public GetProductsQueryHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public override async Task<Result<(List<ProductDto> Items, int TotalCount)>> Handle(
            GetProductsQuery query,
            CancellationToken cancellationToken)
        {
            try
            {
                // Строим запрос с включением категории
                var queryable = _context.Products
                    .Include(p => p.Category)
                    .AsQueryable();

                // Применяем фильтры
                if (query.CategoryId.HasValue)
                {
                    queryable = queryable.Where(p => p.CategoryId == query.CategoryId.Value);
                }

                if (!string.IsNullOrWhiteSpace(query.SearchTerm))
                {
                    var searchTerm = query.SearchTerm.ToLower();
                    queryable = queryable.Where(p =>
                        p.Name.ToLower().Contains(searchTerm) ||
                        (p.Description != null && p.Description.ToLower().Contains(searchTerm)) ||
                        p.SKU.ToLower().Contains(searchTerm) ||
                        (p.Manufacturer != null && p.Manufacturer.ToLower().Contains(searchTerm)));
                }

                // Получаем общее количество
                var totalCount = await queryable.CountAsync(cancellationToken);

                // Применяем пагинацию
                var products = await queryable
                    .Skip((query.Page - 1) * query.PageSize)
                    .Take(query.PageSize)
                    .ToListAsync(cancellationToken);

                var productDtos = products.Select(MapToDto).ToList();

                return Result.Ok((Items: productDtos, TotalCount: totalCount));
            }
            catch (Exception ex)
            {
                return Result.Fail($"Ошибка при получении списка продуктов: {ex.Message}");
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
