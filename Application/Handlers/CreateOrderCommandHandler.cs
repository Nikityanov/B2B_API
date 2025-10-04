using FluentResults;
using FluentValidation;
using B2B_API.Application.Commands;
using B2B_API.Domain.Entities;
using B2B_API.Domain.Interfaces;
using B2B_API.CrossCutting.Validation;

namespace B2B_API.Application.Handlers
{
    /// <summary>
    /// Обработчик команды создания заказа
    /// </summary>
    public class CreateOrderCommandHandler : ValidatedCommandHandler<CreateOrderCommand, int>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateOrderCommandHandler(
            IUnitOfWork unitOfWork,
            CreateOrderCommandValidator validator) : base(validator)
        {
            _unitOfWork = unitOfWork;
        }

        protected override async Task<Result<int>> HandleValidated(CreateOrderCommand command, CancellationToken cancellationToken)
        {
            try
            {
                // Проверяем существование клиента
                var customer = await _unitOfWork.Users.FindAsync(u => u.Id == command.CustomerId)
                    .ContinueWith(t => t.Result.FirstOrDefault());
                if (customer == null)
                {
                    return Result.Fail<int>($"Клиент с ID {command.CustomerId} не найден");
                }

                // Проверяем существование товаров и их доступность
                // Проверяем существование товаров и их доступность
                foreach (var orderItemDto in command.OrderItems)
                {
                    var product = await _unitOfWork.Products.FindAsync(p => p.Id == orderItemDto.ProductId)
                        .ContinueWith(t => t.Result.FirstOrDefault());
                    if (product == null)
                    {
                        return Result.Fail<int>($"Товар с ID {orderItemDto.ProductId} не найден");
                    }

                    // Проверяем, что цена товара не изменилась (если требуется строгая валидация цены)
                    // В реальном приложении здесь может быть логика проверки актуальности цены
                }

                // Создаем заказ
                var order = new Order
                {
                    CustomerId = command.CustomerId,
                    Status = command.Status,
                    Notes = command.Notes,
                    OrderDate = DateTime.UtcNow,
                    TotalAmount = 0 // Будет рассчитана ниже
                };

                // Создаем элементы заказа
                var orderItems = new List<OrderItem>();
                decimal totalAmount = 0;

                foreach (var orderItemDto in command.OrderItems)
                {
                    var product = await _unitOfWork.Products.FindAsync(p => p.Id == orderItemDto.ProductId)
                        .ContinueWith(t => t.Result.FirstOrDefault());
                    if (product == null) continue;

                    var orderItem = new OrderItem
                    {
                        ProductId = orderItemDto.ProductId,
                        Quantity = orderItemDto.Quantity,
                        UnitPrice = orderItemDto.UnitPrice,
                        TotalPrice = orderItemDto.Quantity * orderItemDto.UnitPrice
                    };

                    orderItems.Add(orderItem);
                    totalAmount += orderItem.TotalPrice;
                }

                order.TotalAmount = totalAmount;
                order.OrderItems = orderItems;

                // Начинаем транзакцию
                await _unitOfWork.BeginTransactionAsync(cancellationToken);

                try
                {
                    // Сохраняем заказ
                    var createdOrder = await _unitOfWork.Orders.AddAsync(order, cancellationToken);

                    // Сохраняем элементы заказа
                    foreach (var orderItem in orderItems)
                    {
                        orderItem.OrderId = createdOrder.Id;
                        await _unitOfWork.OrderItems.AddAsync(orderItem, cancellationToken);
                    }

                    // Сохраняем изменения
                    await _unitOfWork.SaveChangesAsync(cancellationToken);

                    // Подтверждаем транзакцию
                    await _unitOfWork.CommitTransactionAsync(cancellationToken);

                    return Result.Ok(createdOrder.Id);
                }
                catch (Exception ex)
                {
                    // Откатываем транзакцию в случае ошибки
                    await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                    return Result.Fail<int>($"Ошибка при создании заказа: {ex.Message}");
                }
            }
            catch (Exception ex)
            {
                return Result.Fail<int>($"Ошибка при создании заказа: {ex.Message}");
            }
        }
    }
}