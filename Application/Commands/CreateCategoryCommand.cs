using FluentResults;
using B2B_API.Application.Interfaces;

namespace B2B_API.Application.Commands
{
    /// <summary>
    /// Команда для создания категории
    /// </summary>
    public class CreateCategoryCommand : ICommand<int>
    {
        public required string Name { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
    }
}
