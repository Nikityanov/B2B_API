using FluentResults;
using B2B_API.Application.Interfaces;

namespace B2B_API.Application.Commands
{
    /// <summary>
    /// Команда для удаления продукта из прайс-листа
    /// </summary>
    public class RemoveProductFromPriceListCommand : ICommand
    {
        public int PriceListId { get; set; }
        public int ProductId { get; set; }
    }
}