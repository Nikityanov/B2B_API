using FluentValidation;
using B2B_API.Application.Commands;
using B2B_API.Models.Enums;

namespace B2B_API.CrossCutting.Validation
{
    /// <summary>
    /// Валидатор для команды обновления заказа
    /// </summary>
    public class UpdateOrderCommandValidator : AbstractValidator<UpdateOrderCommand>
    {
        public UpdateOrderCommandValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("ID заказа должен быть больше 0");

            RuleFor(x => x.CustomerId)
                .GreaterThan(0).WithMessage("ID клиента должен быть больше 0");

            RuleFor(x => x.Status)
                .IsInEnum().WithMessage("Некорректный статус заказа");

            RuleFor(x => x.Notes)
                .MaximumLength(1000).WithMessage("Заметки не должны превышать 1000 символов")
                .When(x => !string.IsNullOrEmpty(x.Notes));
        }
    }
}