using FluentValidation;
using B2B_API.Application.Commands;
using B2B_API.Models.Enums;

namespace B2B_API.CrossCutting.Validation
{
    /// <summary>
    /// Валидатор для команды создания заказа
    /// </summary>
    public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
    {
        public CreateOrderCommandValidator()
        {
            RuleFor(x => x.CustomerId)
                .GreaterThan(0).WithMessage("ID клиента должен быть больше 0");

            RuleFor(x => x.Status)
                .IsInEnum().WithMessage("Некорректный статус заказа");

            RuleFor(x => x.Notes)
                .MaximumLength(1000).WithMessage("Заметки не должны превышать 1000 символов")
                .When(x => !string.IsNullOrEmpty(x.Notes));

            RuleFor(x => x.OrderItems)
                .NotEmpty().WithMessage("Заказ должен содержать хотя бы один товар")
                .Must(items => items.Count > 0).WithMessage("Заказ должен содержать хотя бы один товар");

            RuleForEach(x => x.OrderItems)
                .SetValidator(new CreateOrderItemDtoValidator());
        }
    }

    /// <summary>
    /// Валидатор для элементов заказа
    /// </summary>
    public class CreateOrderItemDtoValidator : AbstractValidator<API.DTOs.CreateOrderItemDto>
    {
        public CreateOrderItemDtoValidator()
        {
            RuleFor(x => x.ProductId)
                .GreaterThan(0).WithMessage("ID товара должен быть больше 0");

            RuleFor(x => x.Quantity)
                .GreaterThan(0).WithMessage("Количество товара должно быть больше 0")
                .LessThanOrEqualTo(1000).WithMessage("Количество товара не должно превышать 1000");

            RuleFor(x => x.UnitPrice)
                .GreaterThan(0).WithMessage("Цена товара должна быть больше 0")
                .LessThanOrEqualTo(999999.99m).WithMessage("Цена товара не должна превышать 999999.99");
        }
    }
}