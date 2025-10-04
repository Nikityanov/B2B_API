using FluentResults;
using B2B_API.Application.Queries;
using B2B_API.Domain.Entities;
using B2B_API.Domain.Interfaces;
using B2B_API.API.DTOs;

namespace B2B_API.Application.Handlers
{
    /// <summary>
    /// Обработчик запроса получения заказов пользователя
    /// </summary>
    public class GetUserOrdersQueryHandler : BaseQueryHandler<GetUserOrdersQuery, List<OrderDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetUserOrdersQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public override async Task<Result<List<OrderDto>>> Handle(GetUserOrdersQuery query, CancellationToken cancellationToken)
        {
            try
            {
                // Проверяем существование пользователя
                var customer = await _unitOfWork.Users.FindAsync(u => u.Id == query.CustomerId)
                    .ContinueWith(t => t.Result.FirstOrDefault());

                if (customer == null)
                {
                    return Result.Fail<List<OrderDto>>($"Клиент с ID {query.CustomerId} не найден");
                }

                // Получаем заказы пользователя
                var orders = await _unitOfWork.Orders.FindAsync(o => o.CustomerId == query.CustomerId)
                    .ContinueWith(t => t.Result.ToList());

                // Применяем пагинацию
                var pagedOrders = orders
                    .OrderByDescending(o => o.OrderDate) // Сортируем по дате заказа (новые первыми)
                    .Skip((query.Page - 1) * query.PageSize)
                    .Take(query.PageSize)
                    .ToList();

                // Маппим заказы в DTO
                var orderDtos = new List<OrderDto>();

                foreach (var order in pagedOrders)
                {
                    var orderDto = await MapToDto(order);
                    orderDtos.Add(orderDto);
                }

                return Result.Ok(orderDtos);
            }
            catch (Exception ex)
            {
                return Result.Fail<List<OrderDto>>($"Ошибка при получении заказов пользователя: {ex.Message}");
            }
        }

        private async Task<OrderDto> MapToDto(Order order)
        {
            // Получаем элементы заказа
            var orderItems = await _unitOfWork.OrderItems.FindAsync(oi => oi.OrderId == order.Id)
                .ContinueWith(t => t.Result.ToList());

            // Маппим элементы заказа
            var orderItemDtos = new List<OrderItemDto>();
            foreach (var orderItem in orderItems)
            {
                var product = await _unitOfWork.Products.FindAsync(p => p.Id == orderItem.ProductId)
                    .ContinueWith(t => t.Result.FirstOrDefault());

                var orderItemDto = new OrderItemDto
                {
                    Id = orderItem.Id,
                    OrderId = orderItem.OrderId,
                    ProductId = orderItem.ProductId,
                    ProductName = product?.Name,
                    Quantity = orderItem.Quantity,
                    UnitPrice = orderItem.UnitPrice,
                    TotalPrice = orderItem.TotalPrice,
                    CreatedAt = orderItem.CreatedAt,
                    ModifiedAt = orderItem.ModifiedAt
                };
                orderItemDtos.Add(orderItemDto);
            }

            // Генерируем номер заказа (если поле отсутствует в доменной модели)
            var orderNumber = $"ORD-{order.Id:D8}";

            return new OrderDto
            {
                Id = order.Id,
                OrderNumber = orderNumber,
                CustomerId = order.CustomerId,
                CustomerName = null, // Не включаем имя клиента в ответ для заказов пользователя
                OrderDate = order.OrderDate,
                Status = order.Status,
                TotalAmount = order.TotalAmount,
                Notes = order.Notes,
                CreatedAt = order.CreatedAt,
                ModifiedAt = order.ModifiedAt,
                OrderItems = orderItemDtos
            };
        }
    }
}