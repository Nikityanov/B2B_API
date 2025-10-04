using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using FluentResults;
using B2B_API.Application.Queries;
using B2B_API.Models.Enums;

namespace B2B_API.API.Controllers
{
    /// <summary>
    /// Контроллер для управления пользователями
    /// </summary>
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UsersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Получает список пользователей с пагинацией и фильтрацией
        /// </summary>
        /// <param name="page">Номер страницы (начиная с 1)</param>
        /// <param name="pageSize">Размер страницы</param>
        /// <param name="searchTerm">Поисковый термин для фильтрации по имени или email</param>
        /// <param name="userType">Фильтр по типу пользователя</param>
        /// <param name="userRole">Фильтр по роли пользователя</param>
        /// <param name="sortBy">Сортировка по полю</param>
        /// <param name="sortOrder">Порядок сортировки (asc/desc)</param>
        /// <returns>Список пользователей с информацией о пагинации</returns>
        [HttpGet]
        public async Task<IActionResult> GetUsers(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? searchTerm = null,
            [FromQuery] UserType? userType = null,
            [FromQuery] UserRole? userRole = null,
            [FromQuery] string? sortBy = null,
            [FromQuery] string? sortOrder = null)
        {
            var query = new GetUsersQuery
            {
                Page = page,
                PageSize = pageSize,
                SearchTerm = searchTerm,
                UserType = userType,
                UserRole = userRole,
                SortBy = sortBy,
                SortOrder = sortOrder
            };

            var result = await _mediator.Send(query);

            return result.IsSuccess ? Ok(result.Value) : BadRequest(result.ToResult());
        }

        /// <summary>
        /// Получает пользователя по ID
        /// </summary>
        /// <param name="id">Идентификатор пользователя</param>
        /// <returns>Информация о пользователе</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            var query = new GetUserByIdQuery { Id = id };
            var result = await _mediator.Send(query);

            if (result.IsFailed)
                return NotFound(result.ToResult());

            return Ok(result.Value);
        }
    }
}