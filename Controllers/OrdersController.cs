using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using B2B_API.Models;
using B2B_API.Models.DTO;
using B2B_API.Interfaces;
using B2B_API.Services;
using B2B_API.Data; // Добавлено using для ApplicationDbContext
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace B2B_API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IGenericRepository<Order> _repository;
        private readonly OrderService _orderService;
        private readonly ApplicationDbContext _context; // Add context for Include queries

        public OrdersController(IGenericRepository<Order> repository, OrderService orderService, ApplicationDbContext context)
        {
            _repository = repository;
            _orderService = orderService;
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderResponseDto>>> GetOrders([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            if (page < 1 || pageSize < 1)
            {
                return BadRequest("Page and PageSize must be greater than 0.");
            }

            var (orders, totalCount) = await _repository.GetPagedAsync(page, pageSize);
            var orderDtos = orders.Select(order => _orderService.ToDto(order));

            var response = new
            {
                TotalCount = totalCount,
                PageSize = pageSize,
                CurrentPage = page,
                Orders = orderDtos
            };

            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<OrderResponseDto>> GetOrder(int id)
        {
            if (_context.Orders == null)
            {
                return Problem("Entity set '_context.Orders' is null.", statusCode: 500); // Или другой подходящий код ошибки
            }

            var order = await _context.Orders // Use _context for Include
                .Include(o => o.Customer)
                .Include(o => o.OrderItems)
                    .ThenInclude(item => item.Product)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            return Ok(_orderService.ToDto(order));
        }

        [HttpPost]
        public async Task<ActionResult<OrderResponseDto>> CreateOrder(OrderCreateDto createDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return Forbid(); // Или BadRequest("Invalid user ID");
            }

            var order = await _orderService.CreateOrderAsync(createDto, userId);
            return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, _orderService.ToDto(order));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrder(int id, OrderUpdateDto updateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var order = await _repository.GetByIdAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            await _orderService.UpdateOrderAsync(order, updateDto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var order = await _repository.GetByIdAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            await _orderService.DeleteOrderAsync(order);
            return NoContent();
        }
    }
}