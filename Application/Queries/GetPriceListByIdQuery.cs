using FluentResults;
using B2B_API.Application.Interfaces;
using B2B_API.API.DTOs;

namespace B2B_API.Application.Queries
{
    /// <summary>
    /// Запрос для получения прайс-листа по ID
    /// </summary>
    public class GetPriceListByIdQuery : IQuery<PriceListDto>
    {
        public int Id { get; set; }
    }
}