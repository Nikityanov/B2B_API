using System.Text;
using FluentResults;
using FluentValidation;
using B2B_API.Application.Commands;
using B2B_API.Domain.Entities;
using B2B_API.Domain.Interfaces;
using B2B_API.CrossCutting.Validation;
using Microsoft.AspNetCore.Identity;

namespace B2B_API.Application.Handlers
{
    /// <summary>
    /// Обработчик команды создания пользователя
    /// </summary>
    public class CreateUserCommandHandler : ValidatedCommandHandler<CreateUserCommand, int>
    {
        private readonly IRepository<User> _userRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateUserCommandHandler(
            IRepository<User> userRepository,
            IUnitOfWork unitOfWork,
            CreateUserCommandValidator validator) : base(validator)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }

        protected override async Task<Result<int>> HandleValidated(CreateUserCommand command, CancellationToken cancellationToken)
        {
            try
            {
                // Проверяем уникальность email (бизнес-валидация)
                var existingUser = await _userRepository
                    .FindAsync(u => u.Email.ToLower() == command.Email.ToLower())
                    .ContinueWith(t => t.Result.FirstOrDefault());

                if (existingUser != null)
                    return Result.Fail<int>("Пользователь с таким email уже существует");

                // Хешируем пароль с использованием PasswordHasher
                var passwordHasher = new PasswordHasher<string>();
                var hashedPassword = passwordHasher.HashPassword("temp", command.Password);

                // Создаем нового пользователя
                var user = new User
                {
                    Name = command.Name,
                    Email = command.Email,
                    Phone = command.Phone,
                    PasswordHash = hashedPassword,
                    UserType = command.UserType,
                    UserRole = command.UserRole,
                    UNP = "000000000", // Временное значение для аутентификации
                    LegalAddress = "Не указан", // Временное значение для аутентификации
                    ActualAddress = "Не указан" // Временное значение для аутентификации
                };

                // Сохраняем в репозиторий
                var createdUser = await _userRepository.AddAsync(user, cancellationToken);

                // Сохраняем изменения в базе данных
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return Result.Ok(createdUser.Id);
            }
            catch (Exception ex)
            {
                return Result.Fail<int>($"Ошибка при создании пользователя: {ex.Message}");
            }
        }

    }
}
