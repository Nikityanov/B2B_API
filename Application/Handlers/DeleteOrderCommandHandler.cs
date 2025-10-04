using FluentResults;
using FluentValidation;
using B2B_API.Application.Commands;
using B2B_API.Domain.Entities;
using B2B_API.Domain.Interfaces;
using B2B_API.CrossCutting.Validation;

namespace B2B_API.Application.Handlers
{
    /// <summary>
    /// Обработчик команды удаления заказа
    /// </summary>
    public class DeleteOrderCommandHandler : ValidatedCommandHandler<DeleteOrderCommand>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteOrderCommandHandler(
            IUnitOfWork unitOfWork,
            DeleteOrderCommandValidator validator) : base(validator)
        {
            _unitOfWork = unitOfWork;
        }

        protected override async Task<Result> HandleValidated(DeleteOrderCommand command, CancellationToken cancellationToken)
        {
            try
            {
                // Получаем заказ для удаления
                var order = await _unitOfWork.Orders.FindAsync(o => o.Id == command.Id)
                    .ContinueWith(t => t.Result.FirstOrDefault());

                if (order == null)
                {
                    return Result.Fail($"Заказ с ID {command.Id} не найден");
                }

                // Проверяем, что заказ можно удалить (например, не в финальном статусе)
                if (order.Status == Models.Enums.OrderStatus.Shipped || order.Status == Models.Enums.OrderStatus.Delivered)
                {
                    return Result.Fail($"Нельзя удалять заказ в статусе '{order.Status}'");
                }

                // Проверяем, есть ли элементы заказа
                var orderItems = await _unitOfWork.OrderItems.FindAsync(oi => oi.OrderId == command.Id);
                var hasOrderItems = orderItems.Any();

                // Начинаем транзакцию
                await _unitOfWork.BeginTransactionAsync(cancellationToken);

                try
                {
                    // Если есть элементы заказа, удаляем их сначала
                    if (hasOrderItems)
                    {
                        foreach (var orderItem in orderItems)
                        {
                            await _unitOfWork.OrderItems.DeleteAsync(orderItem, cancellationToken);
                        }
                    }

                    // Удаляем заказ
                    await _unitOfWork.Orders.DeleteAsync(order, cancellationToken);

                    // Сохраняем изменения
                    await _unitOfWork.SaveChangesAsync(cancellationToken);

                    // Подтверждаем транзакцию
                    await _unitOfWork.CommitTransactionAsync(cancellationToken);

                    return Result.Ok();
                }
                catch (Exception ex)
                {
                    // Откатываем транзакцию в случае ошибки
                    await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                    return Result.Fail($"Ошибка при удалении заказа: {ex.Message}");
                }
            }
            catch (Exception ex)
            {
                return Result.Fail($"Ошибка при удалении заказа: {ex.Message}");
            }
        }
    }
}