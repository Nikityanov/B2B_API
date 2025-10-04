using FluentResults;
using B2B_API.Application.Interfaces;
using B2B_API.API.DTOs;

namespace B2B_API.Application.Commands
{
    /// <summary>
    /// Команда для аутентификации пользователя
    /// </summary>
    public class LoginCommand : ICommand<(string AccessToken, string RefreshToken, int UserId, string UserRole)>
    {
        public required LoginRequestDto LoginData { get; set; }
    }
}
