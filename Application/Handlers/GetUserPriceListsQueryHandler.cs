using FluentResults;
using B2B_API.Application.Queries;
using B2B_API.Domain.Entities;
using B2B_API.Domain.Interfaces;

namespace B2B_API.Application.Handlers
{
    /// <summary>
    /// Обработчик запроса получения прайс-листов пользователя
    /// </summary>
    public class GetUserPriceListsQueryHandler : BaseQueryHandler<GetUserPriceListsQuery, List<PriceListListDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetUserPriceListsQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public override async Task<Result<List<PriceListListDto>>> Handle(GetUserPriceListsQuery query, CancellationToken cancellationToken)
        {
            try
            {
                // Проверяем существование пользователя
                var user = await _unitOfWork.Users.GetByIdAsync(query.UserId, cancellationToken);
                if (user == null)
                    return Result.Fail<List<PriceListListDto>>($"Пользователь с ID {query.UserId} не найден");

                // Получаем прайс-листы пользователя
                var allPriceLists = await _unitOfWork.PriceLists.GetAllAsync(cancellationToken);

                var userPriceLists = allPriceLists
                    .Where(pl => pl.SellerId == query.UserId)
                    .Where(pl => query.IncludeInactive || pl.IsActive)
                    .OrderByDescending(pl => pl.CreatedAt)
                    .ToList();

                // Маппим в DTO и добавляем количество продуктов
                var priceListDtos = new List<PriceListListDto>();

                foreach (var priceList in userPriceLists)
                {
                    var productsCount = priceList.PriceListProducts?.Count(plp => plp.IsActive) ?? 0;

                    priceListDtos.Add(new PriceListListDto
                    {
                        Id = priceList.Id,
                        Name = priceList.Name,
                        Description = priceList.Description,
                        Currency = priceList.Currency,
                        SellerId = priceList.SellerId,
                        IsActive = priceList.IsActive,
                        CreatedAt = priceList.CreatedAt,
                        ModifiedAt = priceList.ModifiedAt,
                        ProductsCount = productsCount
                    });
                }

                return Result.Ok(priceListDtos);
            }
            catch (Exception ex)
            {
                return Result.Fail<List<PriceListListDto>>($"Ошибка при получении прайс-листов пользователя: {ex.Message}");
            }
        }
    }
}