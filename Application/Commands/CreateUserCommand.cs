using FluentResults;
using B2B_API.Application.Interfaces;
using B2B_API.Models.Enums;

namespace B2B_API.Application.Commands
{
    /// <summary>
    /// Команда для создания пользователя
    /// </summary>
    public class CreateUserCommand : ICommand<int>
    {
        public required string Name { get; set; }
        public required string Email { get; set; }
        public string? Phone { get; set; }
        public required string Password { get; set; }
        public UserType UserType { get; set; }
        public UserRole UserRole { get; set; }
    }
}
