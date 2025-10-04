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
    public class CategoriesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CategoriesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Получает список категорий с пагинацией
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetCategories([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var query = new GetCategoriesQuery { Page = page, PageSize = pageSize };
            var result = await _mediator.Send(query);

            return result.IsSuccess ? Ok(result.Value) : BadRequest(result.ToResult());
        }

        /// <summary>
        /// Получает категорию по ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategory(int id)
        {
            var query = new GetCategoryByIdQuery { Id = id };
            var result = await _mediator.Send(query);

            if (result.IsFailed)
                return NotFound(result.ToResult());

            return Ok(result.Value);
        }

        /// <summary>
        /// Создает новую категорию
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin, Seller")]
        public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryCommand command)
        {
            var result = await _mediator.Send(command);

            if (result.IsFailed)
                return BadRequest(result.ToResult());

            return CreatedAtAction(
                nameof(GetCategory),
                new { id = result.Value },
                new { Id = result.Value });
        }

        /// <summary>
        /// Обновляет категорию
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] UpdateCategoryCommand command)
        {
            command.Id = id;
            var result = await _mediator.Send(command);

            return result.IsSuccess ? NoContent() : BadRequest(result.ToResult());
        }

        /// <summary>
        /// Удаляет категорию
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var command = new DeleteCategoryCommand { Id = id };
            var result = await _mediator.Send(command);

            return result.IsSuccess ? NoContent() : BadRequest(result.ToResult());
        }
    }
}
