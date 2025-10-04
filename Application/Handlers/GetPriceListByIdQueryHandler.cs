using FluentResults;
using B2B_API.Application.Queries;
using B2B_API.Domain.Entities;
using B2B_API.Domain.Interfaces;
using B2B_API.API.DTOs;

namespace B2B_API.Application.Handlers
{
    /// <summary>
    /// Обработчик запроса получения прайс-листа по ID
    /// </summary>
    public class GetPriceListByIdQueryHandler : BaseQueryHandler<GetPriceListByIdQuery, PriceListDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetPriceListByIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public override async Task<Result<PriceListDto>> Handle(GetPriceListByIdQuery query, CancellationToken cancellationToken)
        {
            try
            {
                var priceList = await _unitOfWork.PriceLists.GetByIdAsync(query.Id, cancellationToken);

                if (priceList == null)
                    return Result.Fail<PriceListDto>($"Прайс-лист с ID {query.Id} не найден");

                var priceListDto = MapToDto(priceList);
                return Result.Ok(priceListDto);
            }
            catch (Exception ex)
            {
                return Result.Fail<PriceListDto>($"Ошибка при получении прайс-листа: {ex.Message}");
            }
        }

        private static PriceListDto MapToDto(PriceList priceList)
        {
            var priceListProducts = priceList.PriceListProducts?
                .Where(plp => plp.IsActive)
                .Select(MapPriceListProductToDto)
                .ToList();

            return new PriceListDto
            {
                Id = priceList.Id,
                Name = priceList.Name,
                Description = priceList.Description,
                Currency = priceList.Currency,
                SellerId = priceList.SellerId,
                IsActive = priceList.IsActive,
                CreatedAt = priceList.CreatedAt,
                ModifiedAt = priceList.ModifiedAt,
                PriceListProducts = priceListProducts
            };
        }

        private static PriceListProductDto MapPriceListProductToDto(PriceListProduct priceListProduct)
        {
            return new PriceListProductDto
            {
                Id = priceListProduct.Id,
                PriceListId = priceListProduct.PriceListId,
                ProductId = priceListProduct.ProductId,
                SpecialPrice = priceListProduct.SpecialPrice,
                IsActive = priceListProduct.IsActive,
                CreatedAt = priceListProduct.CreatedAt,
                ModifiedAt = priceListProduct.ModifiedAt
            };
        }
    }
}