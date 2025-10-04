using FluentValidation;
using B2B_API.Application.Commands;

namespace B2B_API.CrossCutting.Validation
{
    /// <summary>
    /// Валидатор для команды создания прайс-листа
    /// </summary>
    public class CreatePriceListCommandValidator : AbstractValidator<CreatePriceListCommand>
    {
        public CreatePriceListCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Название прайс-листа обязательно для заполнения")
                .MaximumLength(200).WithMessage("Название прайс-листа не должно превышать 200 символов")
                .Matches(@"^[a-zA-Zа-яА-Я0-9\s\-_()]+$").WithMessage("Название прайс-листа содержит недопустимые символы");

            RuleFor(x => x.Description)
                .MaximumLength(1000).WithMessage("Описание прайс-листа не должно превышать 1000 символов")
                .When(x => !string.IsNullOrEmpty(x.Description));

            RuleFor(x => x.Currency)
                .NotEmpty().WithMessage("Валюта обязательна для заполнения")
                .MaximumLength(3).WithMessage("Код валюты не должен превышать 3 символа")
                .Matches(@"^[A-Z]{3}$").WithMessage("Код валюты должен состоять из 3 заглавных букв");

            RuleFor(x => x.SellerId)
                .GreaterThan(0).WithMessage("ID продавца должен быть положительным числом");
        }
    }
}