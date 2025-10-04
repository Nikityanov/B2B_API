using FluentResults;
using B2B_API.Application.Queries;
using B2B_API.Domain.Entities;
using B2B_API.Domain.Interfaces;
using B2B_API.API.DTOs;

namespace B2B_API.Application.Handlers
{
    /// <summary>
    /// Обработчик запроса получения списка заказов с пагинацией и фильтрацией
    /// </summary>
    public class GetOrdersQueryHandler : BasePagedQueryHandler<GetOrdersQuery, OrderDto, List<OrderDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetOrdersQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public override async Task<Result<(List<OrderDto> Items, int TotalCount)>> Handle(
            GetOrdersQuery query,
            CancellationToken cancellationToken)
        {
            try
            {
                // Строим фильтр для заказов
                var ordersQuery = _unitOfWork.Orders.GetAllAsync(cancellationToken);

                // Применяем фильтры
                if (query.CustomerId.HasValue)
                {
                    ordersQuery = _unitOfWork.Orders.FindAsync(o => o.CustomerId == query.CustomerId.Value, cancellationToken);
                }

                if (query.Status.HasValue)
                {
                    ordersQuery = _unitOfWork.Orders.FindAsync(o => o.Status == query.Status.Value, cancellationToken);
                }

                if (query.StartDate.HasValue)
                {
                    ordersQuery = _unitOfWork.Orders.FindAsync(o => o.OrderDate >= query.StartDate.Value, cancellationToken);
                }

                if (query.EndDate.HasValue)
                {
                    ordersQuery = _unitOfWork.Orders.FindAsync(o => o.OrderDate <= query.EndDate.Value, cancellationToken);
                }

                // Получаем заказы асинхронно
                var ordersTask = ordersQuery.ContinueWith(t => t.Result.ToList());
                var orders = await ordersTask;

                // Получаем общее количество заказов (до пагинации)
                var totalCount = orders.Count;

                // Применяем пагинацию
                var pagedOrders = orders
                    .Skip((query.Page - 1) * query.PageSize)
                    .Take(query.PageSize)
                    .ToList();

                // Получаем элементы заказов для каждого заказа
                var orderDtos = new List<OrderDto>();

                foreach (var order in pagedOrders)
                {
                    var orderDto = await MapToDto(order);
                    orderDtos.Add(orderDto);
                }

                return Result.Ok((Items: orderDtos, TotalCount: totalCount));
            }
            catch (Exception ex)
            {
                return Result.Fail($"Ошибка при получении списка заказов: {ex.Message}");
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