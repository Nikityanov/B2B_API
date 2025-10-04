using FluentResults;
using FluentValidation;
using B2B_API.Application.Commands;
using B2B_API.Domain.Entities;
using B2B_API.Domain.Interfaces;
using B2B_API.CrossCutting.Validation;

namespace B2B_API.Application.Handlers
{
    /// <summary>
    /// Обработчик команды обновления заказа
    /// </summary>
    public class UpdateOrderCommandHandler : ValidatedCommandHandler<UpdateOrderCommand>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateOrderCommandHandler(
            IUnitOfWork unitOfWork,
            UpdateOrderCommandValidator validator) : base(validator)
        {
            _unitOfWork = unitOfWork;
        }

        protected override async Task<Result> HandleValidated(UpdateOrderCommand command, CancellationToken cancellationToken)
        {
            try
            {
                // Получаем заказ для обновления
                var order = await _unitOfWork.Orders.FindAsync(o => o.Id == command.Id)
                    .ContinueWith(t => t.Result.FirstOrDefault());

                if (order == null)
                {
                    return Result.Fail($"Заказ с ID {command.Id} не найден");
                }

                // Проверяем существование клиента
                var customer = await _unitOfWork.Users.FindAsync(u => u.Id == command.CustomerId)
                    .ContinueWith(t => t.Result.FirstOrDefault());
                if (customer == null)
                {
                    return Result.Fail($"Клиент с ID {command.CustomerId} не найден");
                }

                // Проверяем, что заказ можно обновить (например, не в финальном статусе)
                if (order.Status == Models.Enums.OrderStatus.Shipped || order.Status == Models.Enums.OrderStatus.Delivered)
                {
                    return Result.Fail($"Нельзя обновлять заказ в статусе '{order.Status}'");
                }

                // Обновляем свойства заказа
                order.CustomerId = command.CustomerId;
                order.Status = command.Status;
                order.Notes = command.Notes;
                order.UpdateModifiedDate();

                // Начинаем транзакцию
                await _unitOfWork.BeginTransactionAsync(cancellationToken);

                try
                {
                    // Обновляем заказ в репозитории
                    await _unitOfWork.Orders.UpdateAsync(order, cancellationToken);

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
                    return Result.Fail($"Ошибка при обновлении заказа: {ex.Message}");
                }
            }
            catch (Exception ex)
            {
                return Result.Fail($"Ошибка при обновлении заказа: {ex.Message}");
            }
        }
    }
}