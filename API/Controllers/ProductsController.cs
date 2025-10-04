using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using FluentResults;
using B2B_API.Application.Commands;
using B2B_API.Application.Queries;

namespace B2B_API.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProductsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Получает список продуктов с пагинацией и фильтрацией
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetProducts(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] int? categoryId = null,
            [FromQuery] string? searchTerm = null)
        {
            var query = new GetProductsQuery
            {
                Page = page,
                PageSize = pageSize,
                CategoryId = categoryId,
                SearchTerm = searchTerm
            };

            var result = await _mediator.Send(query);

            if (result.IsFailed)
                return BadRequest(result.ToResult());

            return Ok(new
            {
                Products = result.Value.Items,
                TotalCount = result.Value.TotalCount,
                PageSize = pageSize,
                CurrentPage = page
            });
        }

        /// <summary>
        /// Получает продукт по ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct(int id)
        {
            var query = new GetProductByIdQuery { Id = id };
            var result = await _mediator.Send(query);

            if (result.IsFailed)
                return NotFound(result.ToResult());

            return Ok(result.Value);
        }

        /// <summary>
        /// Создает новый продукт
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Seller")]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductCommand command)
        {
            var result = await _mediator.Send(command);

            if (result.IsFailed)
                return BadRequest(result.ToResult());

            return CreatedAtAction(
                nameof(GetProduct),
                new { id = result.Value },
                new { Id = result.Value });
        }

        /// <summary>
        /// Обновляет продукт
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Seller")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] UpdateProductCommand command)
        {
            command.Id = id;
            var result = await _mediator.Send(command);

            return result.IsSuccess ? NoContent() : BadRequest(result.ToResult());
        }

        /// <summary>
        /// Удаляет продукт
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Seller")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var command = new DeleteProductCommand { Id = id };
            var result = await _mediator.Send(command);

            return result.IsSuccess ? NoContent() : BadRequest(result.ToResult());
        }
    }
}
