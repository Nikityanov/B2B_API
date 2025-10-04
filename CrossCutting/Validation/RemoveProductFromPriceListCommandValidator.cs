using FluentValidation;
using B2B_API.Application.Commands;

namespace B2B_API.CrossCutting.Validation
{
    /// <summary>
    /// Валидатор для команды удаления продукта из прайс-листа
    /// </summary>
    public class RemoveProductFromPriceListCommandValidator : AbstractValidator<RemoveProductFromPriceListCommand>
    {
        public RemoveProductFromPriceListCommandValidator()
        {
            RuleFor(x => x.PriceListId)
                .GreaterThan(0).WithMessage("ID прайс-листа должен быть положительным числом");

            RuleFor(x => x.ProductId)
                .GreaterThan(0).WithMessage("ID продукта должен быть положительным числом");
        }
    }
}