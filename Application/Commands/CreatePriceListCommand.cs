using FluentResults;
using B2B_API.Application.Interfaces;

namespace B2B_API.Application.Commands
{
    /// <summary>
    /// Команда для создания нового прайс-листа
    /// </summary>
    public class CreatePriceListCommand : ICommand<int>
    {
        public required string Name { get; set; }
        public string? Description { get; set; }
        public required string Currency { get; set; }
        public int SellerId { get; set; }
    }
}