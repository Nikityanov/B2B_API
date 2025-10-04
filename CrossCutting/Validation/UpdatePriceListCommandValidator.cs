using FluentValidation;
using B2B_API.Application.Commands;

namespace B2B_API.CrossCutting.Validation
{
    /// <summary>
    /// Валидатор для команды обновления прайс-листа
    /// </summary>
    public class UpdatePriceListCommandValidator : AbstractValidator<UpdatePriceListCommand>
    {
        public UpdatePriceListCommandValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("ID прайс-листа должен быть положительным числом");

            RuleFor(x => x.Name)
                .MaximumLength(200).WithMessage("Название прайс-листа не должно превышать 200 символов")
                .Matches(@"^[a-zA-Zа-яА-Я0-9\s\-_()]+$").WithMessage("Название прайс-листа содержит недопустимые символы")
                .When(x => !string.IsNullOrEmpty(x.Name));

            RuleFor(x => x.Description)
                .MaximumLength(1000).WithMessage("Описание прайс-листа не должно превышать 1000 символов")
                .When(x => !string.IsNullOrEmpty(x.Description));

            RuleFor(x => x.Currency)
                .MaximumLength(3).WithMessage("Код валюты не должен превышать 3 символа")
                .Matches(@"^[A-Z]{3}$").WithMessage("Код валюты должен состоять из 3 заглавных букв")
                .When(x => !string.IsNullOrEmpty(x.Currency));
        }
    }
}