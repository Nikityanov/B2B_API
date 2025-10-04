using FluentValidation;
using B2B_API.Application.Commands;

namespace B2B_API.CrossCutting.Validation
{
    /// <summary>
    /// Валидатор для команды создания пользователя
    /// </summary>
    public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
    {
        public CreateUserCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Имя пользователя обязательно для заполнения")
                .MaximumLength(200).WithMessage("Имя пользователя не должно превышать 200 символов")
                .Matches(@"^[a-zA-Zа-яА-Я\s\-]+$").WithMessage("Имя пользователя содержит недопустимые символы");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email обязателен для заполнения")
                .MaximumLength(100).WithMessage("Email не должен превышать 100 символов")
                .EmailAddress().WithMessage("Некорректный формат email");

            RuleFor(x => x.Phone)
                .MaximumLength(20).WithMessage("Телефон не должен превышать 20 символов")
                .Matches(@"^\+?[0-9\s\-\(\)]+$").WithMessage("Некорректный формат телефона")
                .When(x => !string.IsNullOrEmpty(x.Phone));

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Пароль обязателен для заполнения")
                .MinimumLength(6).WithMessage("Пароль должен содержать минимум 6 символов")
                .MaximumLength(100).WithMessage("Пароль не должен превышать 100 символов");

            RuleFor(x => x.UserType)
                .IsInEnum().WithMessage("Некорректный тип пользователя");

            RuleFor(x => x.UserRole)
                .IsInEnum().WithMessage("Некорректная роль пользователя");
        }
    }
}
