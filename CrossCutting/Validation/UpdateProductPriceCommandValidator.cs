using FluentValidation;
using B2B_API.Application.Commands;

namespace B2B_API.CrossCutting.Validation
{
    /// <summary>
    /// Валидатор для команды обновления цены продукта в прайс-листе
    /// </summary>
    public class UpdateProductPriceCommandValidator : AbstractValidator<UpdateProductPriceCommand>
    {
        public UpdateProductPriceCommandValidator()
        {
            RuleFor(x => x.PriceListId)
                .GreaterThan(0).WithMessage("ID прайс-листа должен быть положительным числом");

            RuleFor(x => x.ProductId)
                .GreaterThan(0).WithMessage("ID продукта должен быть положительным числом");

            RuleFor(x => x.SpecialPrice)
                .GreaterThan(0).WithMessage("Цена продукта должна быть положительной")
                .PrecisionScale(10, 2, true).WithMessage("Цена не должна содержать более 2 знаков после запятой")
                .When(x => x.SpecialPrice.HasValue);
        }
    }
}