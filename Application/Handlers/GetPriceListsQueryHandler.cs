using FluentResults;
using System.Linq.Expressions;
using B2B_API.Application.Queries;
using B2B_API.Domain.Entities;
using B2B_API.Domain.Interfaces;

namespace B2B_API.Application.Handlers
{
    /// <summary>
    /// Обработчик запроса получения списка прайс-листов с пагинацией и фильтрацией
    /// </summary>
    public class GetPriceListsQueryHandler : BasePagedQueryHandler<GetPriceListsQuery, PriceListListDto, List<PriceListListDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetPriceListsQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public override async Task<Result<(List<PriceListListDto> Items, int TotalCount)>> Handle(GetPriceListsQuery query, CancellationToken cancellationToken)
        {
            try
            {
                // Строим фильтры для запроса
                Expression<Func<PriceList, bool>>? filter = null;

                if (!string.IsNullOrEmpty(query.SearchTerm))
                {
                    filter = pl => pl.Name.Contains(query.SearchTerm) ||
                                   (pl.Description != null && pl.Description.Contains(query.SearchTerm));
                }

                if (!string.IsNullOrEmpty(query.Currency))
                {
                    var currencyFilter = filter;
                    filter = pl => (currencyFilter == null || currencyFilter.Compile()(pl)) && pl.Currency == query.Currency;
                }

                if (query.IsActive.HasValue)
                {
                    var activeFilter = filter;
                    filter = pl => (activeFilter == null || activeFilter.Compile()(pl)) && pl.IsActive == query.IsActive.Value;
                }

                if (query.SellerId.HasValue)
                {
                    var sellerFilter = filter;
                    filter = pl => (sellerFilter == null || sellerFilter.Compile()(pl)) && pl.SellerId == query.SellerId.Value;
                }

                // Получаем общее количество записей
                var totalCount = await _unitOfWork.PriceLists.CountAsync(filter, cancellationToken);

                // Получаем все прайс-листы и применяем фильтрацию вручную
                var allPriceLists = await _unitOfWork.PriceLists.GetAllAsync(cancellationToken);

                var filteredPriceLists = filter != null
                    ? allPriceLists.Where(filter.Compile())
                    : allPriceLists;

                // Применяем пагинацию
                var priceLists = filteredPriceLists
                    .OrderByDescending(pl => pl.CreatedAt)
                    .Skip((query.Page - 1) * query.PageSize)
                    .Take(query.PageSize)
                    .ToList();

                // Маппим в DTO и добавляем количество продуктов
                var priceListDtos = new List<PriceListListDto>();

                foreach (var priceList in priceLists)
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

                return Result.Ok((Items: priceListDtos, TotalCount: totalCount));
            }
            catch (Exception ex)
            {
                return Result.Fail<(List<PriceListListDto>, int)>($"Ошибка при получении списка прайс-листов: {ex.Message}");
            }
        }
    }
}