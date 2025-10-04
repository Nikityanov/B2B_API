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
    public class PriceListsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PriceListsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Получает список прайс-листов с пагинацией и фильтрацией
        /// </summary>
        /// <param name="page">Номер страницы (по умолчанию 1)</param>
        /// <param name="pageSize">Размер страницы (по умолчанию 10)</param>
        /// <param name="searchTerm">Поисковый термин для названия прайс-листа</param>
        /// <param name="currency">Фильтр по валюте</param>
        /// <param name="isActive">Фильтр по статусу активности</param>
        /// <param name="sellerId">Фильтр по ID продавца</param>
        /// <returns>Список прайс-листов с пагинацией</returns>
        [HttpGet]
        public async Task<IActionResult> GetPriceLists(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? searchTerm = null,
            [FromQuery] string? currency = null,
            [FromQuery] bool? isActive = null,
            [FromQuery] int? sellerId = null)
        {
            var query = new GetPriceListsQuery
            {
                Page = page,
                PageSize = pageSize,
                SearchTerm = searchTerm,
                Currency = currency,
                IsActive = isActive,
                SellerId = sellerId
            };

            var result = await _mediator.Send(query);

            return result.IsSuccess ? Ok(result.Value) : BadRequest(result.ToResult());
        }

        /// <summary>
        /// Получает прайс-лист по ID
        /// </summary>
        /// <param name="id">ID прайс-листа</param>
        /// <returns>Информация о прайс-листе</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPriceList(int id)
        {
            var query = new GetPriceListByIdQuery { Id = id };
            var result = await _mediator.Send(query);

            if (result.IsFailed)
                return NotFound(result.ToResult());

            return Ok(result.Value);
        }

        /// <summary>
        /// Получает продукты в прайс-листе
        /// </summary>
        /// <param name="id">ID прайс-листа</param>
        /// <param name="page">Номер страницы (по умолчанию 1)</param>
        /// <param name="pageSize">Размер страницы (по умолчанию 10)</param>
        /// <param name="isActive">Фильтр по статусу активности продукта</param>
        /// <param name="searchTerm">Поисковый термин для названия продукта</param>
        /// <returns>Список продуктов в прайс-листе</returns>
        [HttpGet("{id}/products")]
        public async Task<IActionResult> GetPriceListProducts(
            int id,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] bool? isActive = null,
            [FromQuery] string? searchTerm = null)
        {
            var query = new GetPriceListProductsQuery
            {
                PriceListId = id,
                Page = page,
                PageSize = pageSize,
                IsActive = isActive,
                SearchTerm = searchTerm
            };

            var result = await _mediator.Send(query);

            return result.IsSuccess ? Ok(result.Value) : BadRequest(result.ToResult());
        }

        /// <summary>
        /// Получает прайс-листы пользователя
        /// </summary>
        /// <param name="userId">ID пользователя</param>
        /// <param name="includeInactive">Включить неактивные прайс-листы</param>
        /// <returns>Список прайс-листов пользователя</returns>
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserPriceLists(int userId, [FromQuery] bool includeInactive = false)
        {
            var query = new GetUserPriceListsQuery
            {
                UserId = userId,
                IncludeInactive = includeInactive
            };

            var result = await _mediator.Send(query);

            return result.IsSuccess ? Ok(result.Value) : BadRequest(result.ToResult());
        }

        /// <summary>
        /// Создает новый прайс-лист
        /// </summary>
        /// <param name="dto">Данные для создания прайс-листа</param>
        /// <returns>Информация о созданном прайс-листе</returns>
        [HttpPost]
        [Authorize(Roles = "Admin, Seller")]
        public async Task<IActionResult> CreatePriceList([FromBody] CreatePriceListDto dto)
        {
            var command = new CreatePriceListCommand
            {
                Name = dto.Name,
                Description = dto.Description,
                Currency = dto.Currency,
                SellerId = dto.SellerId
            };

            var result = await _mediator.Send(command);

            if (result.IsFailed)
                return BadRequest(result.ToResult());

            return CreatedAtAction(
                nameof(GetPriceList),
                new { id = result.Value },
                new { Id = result.Value });
        }

        /// <summary>
        /// Добавляет продукт в прайс-лист
        /// </summary>
        /// <param name="pricelistId">ID прайс-листа</param>
        /// <param name="dto">Данные продукта для добавления</param>
        /// <returns>Информация о добавленном продукте</returns>
        [HttpPost("{pricelistId}/products")]
        [Authorize(Roles = "Admin, Seller")]
        public async Task<IActionResult> AddProductToPriceList(int pricelistId, [FromBody] CreatePriceListProductDto dto)
        {
            var command = new AddProductToPriceListCommand
            {
                PriceListId = pricelistId,
                ProductId = dto.ProductId,
                SpecialPrice = dto.SpecialPrice
            };

            var result = await _mediator.Send(command);

            if (result.IsFailed)
                return BadRequest(result.ToResult());

            return CreatedAtAction(
                nameof(GetPriceListProducts),
                new { id = pricelistId },
                new { Id = result.Value });
        }

        /// <summary>
        /// Обновляет прайс-лист
        /// </summary>
        /// <param name="id">ID прайс-листа</param>
        /// <param name="dto">Данные для обновления прайс-листа</param>
        /// <returns>Результат обновления</returns>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin, Seller")]
        public async Task<IActionResult> UpdatePriceList(int id, [FromBody] UpdatePriceListDto dto)
        {
            var command = new UpdatePriceListCommand
            {
                Id = id,
                Name = dto.Name,
                Description = dto.Description,
                Currency = dto.Currency,
                IsActive = dto.IsActive
            };

            var result = await _mediator.Send(command);

            return result.IsSuccess ? NoContent() : BadRequest(result.ToResult());
        }

        /// <summary>
        /// Обновляет цену продукта в прайс-листе
        /// </summary>
        /// <param name="pricelistId">ID прайс-листа</param>
        /// <param name="productId">ID продукта</param>
        /// <param name="dto">Данные для обновления цены продукта</param>
        /// <returns>Результат обновления</returns>
        [HttpPut("{pricelistId}/products/{productId}")]
        [Authorize(Roles = "Admin, Seller")]
        public async Task<IActionResult> UpdateProductPrice(int pricelistId, int productId, [FromBody] UpdatePriceListProductDto dto)
        {
            var command = new UpdateProductPriceCommand
            {
                PriceListId = pricelistId,
                ProductId = productId,
                SpecialPrice = dto.SpecialPrice,
                IsActive = dto.IsActive
            };

            var result = await _mediator.Send(command);

            return result.IsSuccess ? NoContent() : BadRequest(result.ToResult());
        }

        /// <summary>
        /// Удаляет прайс-лист
        /// </summary>
        /// <param name="id">ID прайс-листа</param>
        /// <returns>Результат удаления</returns>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin, Seller")]
        public async Task<IActionResult> DeletePriceList(int id)
        {
            var command = new DeletePriceListCommand { Id = id };
            var result = await _mediator.Send(command);

            return result.IsSuccess ? NoContent() : BadRequest(result.ToResult());
        }

        /// <summary>
        /// Удаляет продукт из прайс-листа
        /// </summary>
        /// <param name="pricelistId">ID прайс-листа</param>
        /// <param name="productId">ID продукта</param>
        /// <returns>Результат удаления</returns>
        [HttpDelete("{pricelistId}/products/{productId}")]
        [Authorize(Roles = "Admin, Seller")]
        public async Task<IActionResult> RemoveProductFromPriceList(int pricelistId, int productId)
        {
            var command = new RemoveProductFromPriceListCommand
            {
                PriceListId = pricelistId,
                ProductId = productId
            };

            var result = await _mediator.Send(command);

            return result.IsSuccess ? NoContent() : BadRequest(result.ToResult());
        }
    }
}