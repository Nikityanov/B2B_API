using FluentResults;
using B2B_API.Application.Queries;
using B2B_API.Domain.Entities;
using B2B_API.Domain.Interfaces;
using B2B_API.API.DTOs;

namespace B2B_API.Application.Handlers
{
    /// <summary>
    /// Обработчик запроса получения заказа по ID
    /// </summary>
    public class GetOrderByIdQueryHandler : BaseQueryHandler<GetOrderByIdQuery, OrderDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetOrderByIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public override async Task<Result<OrderDto>> Handle(GetOrderByIdQuery query, CancellationToken cancellationToken)
        {
            try
            {
                // Получаем заказ по ID
                var order = await _unitOfWork.Orders.FindAsync(o => o.Id == query.Id)
                    .ContinueWith(t => t.Result.FirstOrDefault());

                if (order == null)
                {
                    return Result.Fail<OrderDto>($"Заказ с ID {query.Id} не найден");
                }

                // Маппим заказ в DTO
                var orderDto = await MapToDto(order);

                return Result.Ok(orderDto);
            }
            catch (Exception ex)
            {
                return Result.Fail<OrderDto>($"Ошибка при получении заказа: {ex.Message}");
            }
        }

        private async Task<OrderDto> MapToDto(Order order)
        {
            // Получаем клиента
            var customer = await _unitOfWork.Users.FindAsync(u => u.Id == order.CustomerId)
                .ContinueWith(t => t.Result.FirstOrDefault());

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
                CustomerName = customer?.Name,
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