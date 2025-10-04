using FluentResults;
using System.Linq.Expressions;
using B2B_API.Application.Queries;
using B2B_API.Domain.Entities;
using B2B_API.Domain.Interfaces;
using B2B_API.API.DTOs;

namespace B2B_API.Application.Handlers
{
    /// <summary>
    /// Обработчик запроса получения продуктов в прайс-листе
    /// </summary>
    public class GetPriceListProductsQueryHandler : BasePagedQueryHandler<GetPriceListProductsQuery, PriceListProductDto, List<PriceListProductDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetPriceListProductsQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public override async Task<Result<(List<PriceListProductDto> Items, int TotalCount)>> Handle(GetPriceListProductsQuery query, CancellationToken cancellationToken)
        {
            try
            {
                // Проверяем существование прайс-листа
                var priceList = await _unitOfWork.PriceLists.GetByIdAsync(query.PriceListId, cancellationToken);
                if (priceList == null)
                    return Result.Fail<(List<PriceListProductDto>, int)>($"Прайс-лист с ID {query.PriceListId} не найден");

                // Получаем продукты прайс-листа
                var allPriceListProducts = priceList.PriceListProducts?
                    .Where(plp => plp.PriceListId == query.PriceListId) ?? new List<PriceListProduct>();

                // Применяем фильтры
                if (query.IsActive.HasValue)
                {
                    allPriceListProducts = allPriceListProducts.Where(plp => plp.IsActive == query.IsActive.Value);
                }

                // Получаем общее количество
                var totalCount = allPriceListProducts.Count();

                // Применяем пагинацию
                var priceListProducts = allPriceListProducts
                    .Skip((query.Page - 1) * query.PageSize)
                    .Take(query.PageSize)
                    .ToList();

                // Маппим в DTO
                var priceListProductDtos = new List<PriceListProductDto>();

                foreach (var priceListProduct in priceListProducts)
                {
                    priceListProductDtos.Add(new PriceListProductDto
                    {
                        Id = priceListProduct.Id,
                        PriceListId = priceListProduct.PriceListId,
                        ProductId = priceListProduct.ProductId,
                        SpecialPrice = priceListProduct.SpecialPrice,
                        IsActive = priceListProduct.IsActive,
                        CreatedAt = priceListProduct.CreatedAt,
                        ModifiedAt = priceListProduct.ModifiedAt
                    });
                }

                return Result.Ok((Items: priceListProductDtos, TotalCount: totalCount));
            }
            catch (Exception ex)
            {
                return Result.Fail<(List<PriceListProductDto>, int)>($"Ошибка при получении продуктов прайс-листа: {ex.Message}");
            }
        }
    }
}