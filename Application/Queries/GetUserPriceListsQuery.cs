using FluentResults;
using B2B_API.Application.Interfaces;

namespace B2B_API.Application.Queries
{
    /// <summary>
    /// Запрос для получения прайс-листов пользователя
    /// </summary>
    public class GetUserPriceListsQuery : IQuery<List<PriceListListDto>>
    {
        public int UserId { get; set; }
        public bool IncludeInactive { get; set; } = false;
    }
}