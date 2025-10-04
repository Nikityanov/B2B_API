using FluentValidation;
using B2B_API.Application.Commands;

namespace B2B_API.CrossCutting.Validation
{
    /// <summary>
    /// Валидатор для команды добавления товара в заказ
    /// </summary>
    public class AddOrderItemCommandValidator : AbstractValidator<AddOrderItemCommand>
    {
        public AddOrderItemCommandValidator()
        {
            RuleFor(x => x.OrderId)
                .GreaterThan(0).WithMessage("ID заказа должен быть больше 0");

            RuleFor(x => x.OrderItem)
                .NotNull().WithMessage("Элемент заказа не может быть пустым");

            RuleFor(x => x.OrderItem.ProductId)
                .GreaterThan(0).WithMessage("ID товара должен быть больше 0");

            RuleFor(x => x.OrderItem.Quantity)
                .GreaterThan(0).WithMessage("Количество товара должно быть больше 0")
                .LessThanOrEqualTo(1000).WithMessage("Количество товара не должно превышать 1000");

            RuleFor(x => x.OrderItem.UnitPrice)
                .GreaterThan(0).WithMessage("Цена товара должна быть больше 0")
                .LessThanOrEqualTo(999999.99m).WithMessage("Цена товара не должна превышать 999999.99");
        }
    }
}