using FluentResults;
using B2B_API.Application.Interfaces;

namespace B2B_API.Application.Commands
{
    /// <summary>
    /// Команда для обновления прайс-листа
    /// </summary>
    public class UpdatePriceListCommand : ICommand
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Currency { get; set; }
        public bool? IsActive { get; set; }
    }
}