using FluentResults;
using FluentValidation;
using B2B_API.Application.Commands;
using B2B_API.Domain.Entities;
using B2B_API.Domain.Interfaces;
using B2B_API.CrossCutting.Validation;

namespace B2B_API.Application.Handlers
{
    /// <summary>
    /// Обработчик команды добавления товара в заказ
    /// </summary>
    public class AddOrderItemCommandHandler : ValidatedCommandHandler<AddOrderItemCommand, int>
    {
        private readonly IUnitOfWork _unitOfWork;

        public AddOrderItemCommandHandler(
            IUnitOfWork unitOfWork,
            AddOrderItemCommandValidator validator) : base(validator)
        {
            _unitOfWork = unitOfWork;
        }

        protected override async Task<Result<int>> HandleValidated(AddOrderItemCommand command, CancellationToken cancellationToken)
        {
            try
            {
                // Получаем заказ
                var order = await _unitOfWork.Orders.FindAsync(o => o.Id == command.OrderId)
                    .ContinueWith(t => t.Result.FirstOrDefault());

                if (order == null)
                {
                    return Result.Fail<int>($"Заказ с ID {command.OrderId} не найден");
                }

                // Проверяем, что в заказ можно добавлять товары
                if (order.Status != Models.Enums.OrderStatus.Pending)
                {
                    return Result.Fail<int>($"Нельзя добавлять товары в заказ со статусом '{order.Status}'");
                }

                // Получаем товар
                var product = await _unitOfWork.Products.FindAsync(p => p.Id == command.OrderItem.ProductId)
                    .ContinueWith(t => t.Result.FirstOrDefault());
                if (product == null)
                {
                    return Result.Fail<int>($"Товар с ID {command.OrderItem.ProductId} не найден");
                }

                // Проверяем, что товар уже не добавлен в заказ
                var existingOrderItem = await _unitOfWork.OrderItems.FindAsync(
                    oi => oi.OrderId == command.OrderId && oi.ProductId == command.OrderItem.ProductId)
                    .ContinueWith(t => t.Result.FirstOrDefault());

                if (existingOrderItem != null)
                {
                    return Result.Fail<int>($"Товар уже добавлен в заказ");
                }

                // Создаем элемент заказа
                var orderItem = new OrderItem
                {
                    OrderId = command.OrderId,
                    ProductId = command.OrderItem.ProductId,
                    Quantity = command.OrderItem.Quantity,
                    UnitPrice = command.OrderItem.UnitPrice,
                    TotalPrice = command.OrderItem.Quantity * command.OrderItem.UnitPrice
                };

                // Начинаем транзакцию
                await _unitOfWork.BeginTransactionAsync(cancellationToken);

                try
                {
                    // Добавляем элемент заказа
                    var createdOrderItem = await _unitOfWork.OrderItems.AddAsync(orderItem, cancellationToken);

                    // Обновляем общую сумму заказа
                    order.TotalAmount += orderItem.TotalPrice;
                    order.UpdateModifiedDate();

                    await _unitOfWork.Orders.UpdateAsync(order, cancellationToken);

                    // Сохраняем изменения
                    await _unitOfWork.SaveChangesAsync(cancellationToken);

                    // Подтверждаем транзакцию
                    await _unitOfWork.CommitTransactionAsync(cancellationToken);

                    return Result.Ok(createdOrderItem.Id);
                }
                catch (Exception ex)
                {
                    // Откатываем транзакцию в случае ошибки
                    await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                    return Result.Fail<int>($"Ошибка при добавлении товара в заказ: {ex.Message}");
                }
            }
            catch (Exception ex)
            {
                return Result.Fail<int>($"Ошибка при добавлении товара в заказ: {ex.Message}");
            }
        }
    }
}