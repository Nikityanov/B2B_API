using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using FluentResults;
using B2B_API.Application.Commands;
using B2B_API.Application.Queries;
using B2B_API.API.DTOs;

namespace B2B_API.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public OrdersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Получает список всех заказов с пагинацией и фильтрацией
        /// </summary>
        /// <param name="page">Номер страницы (по умолчанию 1)</param>
        /// <param name="pageSize">Размер страницы (по умолчанию 10)</param>
        /// <param name="customerId">Фильтр по ID клиента</param>
        /// <param name="status">Фильтр по статусу заказа</param>
        /// <param name="startDate">Фильтр по дате начала периода</param>
        /// <param name="endDate">Фильтр по дате окончания периода</param>
        /// <returns>Список заказов с информацией о пагинации</returns>
        [HttpGet]
        public async Task<IActionResult> GetOrders(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] int? customerId = null,
            [FromQuery] Models.Enums.OrderStatus? status = null,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            var query = new GetOrdersQuery
            {
                Page = page,
                PageSize = pageSize,
                CustomerId = customerId,
                Status = status,
                StartDate = startDate,
                EndDate = endDate
            };

            var result = await _mediator.Send(query);

            return result.IsSuccess ? Ok(result.Value) : BadRequest(result.ToResult());
        }

        /// <summary>
        /// Получает заказ по ID
        /// </summary>
        /// <param name="id">ID заказа</param>
        /// <returns>Информация о заказе</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrder(int id)
        {
            var query = new GetOrderByIdQuery { Id = id };
            var result = await _mediator.Send(query);

            if (result.IsFailed)
                return NotFound(result.ToResult());

            return Ok(result.Value);
        }

        /// <summary>
        /// Получает заказы пользователя
        /// </summary>
        /// <param name="userId">ID пользователя</param>
        /// <param name="page">Номер страницы (по умолчанию 1)</param>
        /// <param name="pageSize">Размер страницы (по умолчанию 10)</param>
        /// <returns>Список заказов пользователя</returns>
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserOrders(int userId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var query = new GetUserOrdersQuery
            {
                CustomerId = userId,
                Page = page,
                PageSize = pageSize
            };

            var result = await _mediator.Send(query);

            return result.IsSuccess ? Ok(result.Value) : BadRequest(result.ToResult());
        }

        /// <summary>
        /// Создает новый заказ
        /// </summary>
        /// <param name="createOrderDto">Данные для создания заказа</param>
        /// <returns>Информация о созданном заказе</returns>
        [HttpPost]
        [Authorize(Roles = "Admin, Seller, Customer")]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDto createOrderDto)
        {
            var command = new CreateOrderCommand
            {
                CustomerId = createOrderDto.CustomerId,
                Status = createOrderDto.Status,
                Notes = createOrderDto.Notes,
                OrderItems = createOrderDto.OrderItems
            };

            var result = await _mediator.Send(command);

            if (result.IsFailed)
                return BadRequest(result.ToResult());

            return CreatedAtAction(
                nameof(GetOrder),
                new { id = result.Value },
                new { Id = result.Value });
        }

        /// <summary>
        /// Добавляет товар в заказ
        /// </summary>
        /// <param name="orderId">ID заказа</param>
        /// <param name="createOrderItemDto">Данные товара для добавления</param>
        /// <returns>Информация о добавленном товаре</returns>
        [HttpPost("{orderId}/items")]
        [Authorize(Roles = "Admin, Seller, Customer")]
        public async Task<IActionResult> AddOrderItem(int orderId, [FromBody] CreateOrderItemDto createOrderItemDto)
        {
            var command = new AddOrderItemCommand
            {
                OrderId = orderId,
                OrderItem = createOrderItemDto
            };

            var result = await _mediator.Send(command);

            if (result.IsFailed)
                return BadRequest(result.ToResult());

            return CreatedAtAction(
                nameof(GetOrder),
                new { id = orderId },
                new { Id = result.Value });
        }

        /// <summary>
        /// Обновляет заказ
        /// </summary>
        /// <param name="id">ID заказа</param>
        /// <param name="updateOrderDto">Данные для обновления заказа</param>
        /// <returns>Результат обновления</returns>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin, Seller")]
        public async Task<IActionResult> UpdateOrder(int id, [FromBody] UpdateOrderDto updateOrderDto)
        {
            var command = new UpdateOrderCommand
            {
                Id = id,
                CustomerId = updateOrderDto.CustomerId,
                Status = updateOrderDto.Status,
                Notes = updateOrderDto.Notes
            };

            var result = await _mediator.Send(command);

            return result.IsSuccess ? NoContent() : BadRequest(result.ToResult());
        }

        /// <summary>
        /// Удаляет заказ
        /// </summary>
        /// <param name="id">ID заказа</param>
        /// <returns>Результат удаления</returns>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var command = new DeleteOrderCommand { Id = id };
            var result = await _mediator.Send(command);

            return result.IsSuccess ? NoContent() : BadRequest(result.ToResult());
        }
    }
}