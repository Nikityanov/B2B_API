using FluentResults;
using B2B_API.Application.Interfaces;

namespace B2B_API.Application.Commands
{
    /// <summary>
    /// Команда для добавления продукта в прайс-лист
    /// </summary>
    public class AddProductToPriceListCommand : ICommand<int>
    {
        public int PriceListId { get; set; }
        public int ProductId { get; set; }
        public decimal SpecialPrice { get; set; }
    }
}