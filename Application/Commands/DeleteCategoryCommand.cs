using FluentResults;
using B2B_API.Application.Interfaces;

namespace B2B_API.Application.Commands
{
    /// <summary>
    /// Команда для удаления категории
    /// </summary>
    public class DeleteCategoryCommand : ICommand
    {
        public int Id { get; set; }
    }
}
