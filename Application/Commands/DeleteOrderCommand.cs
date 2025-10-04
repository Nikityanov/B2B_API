using FluentResults;
using B2B_API.Application.Interfaces;

namespace B2B_API.Application.Commands
{
    /// <summary>
    /// Команда для удаления заказа
    /// </summary>
    public class DeleteOrderCommand : ICommand
    {
        public int Id { get; set; }
    }
}