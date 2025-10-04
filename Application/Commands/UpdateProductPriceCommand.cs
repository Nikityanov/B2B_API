using FluentResults;
using B2B_API.Application.Interfaces;

namespace B2B_API.Application.Commands
{
    /// <summary>
    /// Команда для обновления цены продукта в прайс-листе
    /// </summary>
    public class UpdateProductPriceCommand : ICommand
    {
        public int PriceListId { get; set; }
        public int ProductId { get; set; }
        public decimal? SpecialPrice { get; set; }
        public bool? IsActive { get; set; }
    }
}