using System.Text;
using FluentResults;
using MediatR;
using B2B_API.Application.Commands;
using B2B_API.Infrastructure.External;
using B2B_API.Domain.Entities;
using B2B_API.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace B2B_API.Application.Handlers
{
    /// <summary>
    /// Обработчик команды аутентификации пользователя
    /// </summary>
    public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<(string AccessToken, string RefreshToken, int UserId, string UserRole)>>
    {
        private readonly IRepository<User> _userRepository;
        private readonly JwtService _jwtService;

        public LoginCommandHandler(
            IRepository<User> userRepository,
            JwtService jwtService)
        {
            _userRepository = userRepository;
            _jwtService = jwtService;
        }

        public async Task<Result<(string AccessToken, string RefreshToken, int UserId, string UserRole)>> Handle(
            LoginCommand command,
            CancellationToken cancellationToken)
        {
            try
            {
                // Находим пользователя по email
                var user = await _userRepository
                    .FindAsync(u => u.Email.ToLower() == command.LoginData.Email.ToLower())
                    .ContinueWith(t => t.Result.FirstOrDefault());

                if (user == null)
                    return Result.Fail<(string AccessToken, string RefreshToken, int UserId, string UserRole)>("Неверный email или пароль");

                // Проверяем пароль с использованием PasswordHasher
                var passwordHasher = new PasswordHasher<string>();
                var passwordVerificationResult = passwordHasher.VerifyHashedPassword(
                    "temp",
                    user.PasswordHash,
                    command.LoginData.Password
                );

                if (passwordVerificationResult != PasswordVerificationResult.Success)
                    return Result.Fail<(string AccessToken, string RefreshToken, int UserId, string UserRole)>("Неверный email или пароль");

                // Генерируем токены для доменной модели
                var accessToken = _jwtService.GenerateToken(user);
                var refreshToken = Guid.NewGuid().ToString(); // Временный refresh token

                return Result.Ok((AccessToken: accessToken, RefreshToken: refreshToken, UserId: user.Id, UserRole: user.UserRole.ToString()));
            }
            catch (Exception ex)
            {
                return Result.Fail<(string AccessToken, string RefreshToken, int UserId, string UserRole)>($"Ошибка при аутентификации: {ex.Message}");
            }
        }

    }
}
