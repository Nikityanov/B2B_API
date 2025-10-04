using FluentResults;
using B2B_API.Application.Interfaces;

namespace B2B_API.Application.Commands
{
    /// <summary>
    /// Команда для удаления продукта
    /// </summary>
    public class DeleteProductCommand : ICommand
    {
        public int Id { get; set; }
    }
}
