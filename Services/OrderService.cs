using B2B_API.Models;
using B2B_API.Models.DTO;
using B2B_API.Interfaces;
using B2B_API.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace B2B_API.Services
{
    public class OrderService
    {
        private readonly IGenericRepository<Order> _repository;
        private readonly ApplicationDbContext _context;

        public OrderService(IGenericRepository<Order> repository, ApplicationDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        public OrderResponseDto ToDto(Order entity)
        {
            return new OrderResponseDto
            {
                Id = entity.Id,
                OrderDate = entity.OrderDate,
                Status = entity.Status,
                CustomerId = entity.CustomerId,
                CustomerName = entity.Customer?.Name ?? "Unknown",
                OrderItems = entity.OrderItems?.Select(item => new OrderItemResponseDto
                {
                    ProductId = item.ProductId,
                    ProductName = item.Product?.Name ?? "Unknown",
                    ProductSKU = item.Product?.SKU ?? "Unknown",
                    Quantity = item.Quantity,
                    Price = item.Price
                }).ToList() ?? new List<OrderItemResponseDto>()
            };
        }

        public async Task<Order> CreateOrderAsync(OrderCreateDto createDto, int userId)
        {
            var order = new Order
            {
                CustomerId = userId,
                OrderItems = createDto.OrderItems.Select(itemDto => new OrderItem
                {
                    ProductId = itemDto.ProductId,
                    Quantity = itemDto.Quantity
                }).ToList()
            };

            await _repository.AddAsync(order);
            await _repository.SaveChangesAsync();

            var createdOrder = await GetOrderAsync(order.Id); // Return order with related entities
            if (createdOrder == null)
            {
                throw new InvalidOperationException("Failed to retrieve created order.");
            }
            return createdOrder;
        }

        public async Task UpdateOrderAsync(Order order, OrderUpdateDto updateDto)
        {
            var originalStatus = order.Status; // Store original status for comparison

            if (updateDto.Status.HasValue)
            {
                order.Status = updateDto.Status.Value;
            }

            _repository.Update(order);
            await _repository.SaveChangesAsync();

            if (updateDto.Status.HasValue && updateDto.Status.Value != originalStatus)
            {
                // Можно добавить логирование смены статуса заказа или отправку уведомлений
            }
        }

        public async Task DeleteOrderAsync(Order order)
        {
            _repository.Remove(order);
            await _repository.SaveChangesAsync();
        }

        public async Task<Order?> GetOrderAsync(int id)
        {
            return await _repository.GetAsync(id,
                o => o.Customer,
                o => o.OrderItems,
                o => o.OrderItems!.Select(item => item.Product));
        }

        public async Task<(IEnumerable<Order>, int)> GetOrdersAsync(int page, int pageSize)
        {
            var (orders, totalCount) = await _repository.GetPagedAsync(page, pageSize);

            // Eagerly load related entities
            orders = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderItems)
                    .ThenInclude(item => item.Product)
                .Where(o => orders.Contains(o)) // Filter to only include paged orders
                .ToListAsync();

            return (orders, totalCount);
        }
    }
}
