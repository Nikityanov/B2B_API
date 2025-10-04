using FluentValidation;
using B2B_API.Application.Commands;

namespace B2B_API.CrossCutting.Validation
{
    /// <summary>
    /// Валидатор для команды удаления прайс-листа
    /// </summary>
    public class DeletePriceListCommandValidator : AbstractValidator<DeletePriceListCommand>
    {
        public DeletePriceListCommandValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("ID прайс-листа должен быть положительным числом");
        }
    }
}