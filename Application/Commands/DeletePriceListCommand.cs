using FluentResults;
using B2B_API.Application.Interfaces;

namespace B2B_API.Application.Commands
{
    /// <summary>
    /// Команда для удаления прайс-листа
    /// </summary>
    public class DeletePriceListCommand : ICommand
    {
        public int Id { get; set; }
    }
}