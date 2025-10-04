using Microsoft.AspNetCore.Mvc;
using MediatR;
using FluentResults;
using B2B_API.Application.Commands;
using B2B_API.API.DTOs;

namespace B2B_API.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Аутентификация пользователя
        /// </summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginDto)
        {
            // Валидация DTO
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = "Ошибка валидации данных",
                    Errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList()
                });
            }

            try
            {
                // Создаем команду на основе DTO
                var command = new LoginCommand
                {
                    LoginData = loginDto
                };

                var result = await _mediator.Send(command);

                if (result.IsFailed)
                {
                    return BadRequest(new
                    {
                        Success = false,
                        Message = result.Errors.First().Message
                    });
                }

                return Ok(new
                {
                    Success = true,
                    AccessToken = result.Value.AccessToken,
                    RefreshToken = result.Value.RefreshToken,
                    UserId = result.Value.UserId,
                    UserRole = result.Value.UserRole
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "Внутренняя ошибка сервера при аутентификации"
                });
            }
        }

        /// <summary>
        /// Регистрация нового пользователя
        /// </summary>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto registerDto)
        {
            // Валидация DTO
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = "Ошибка валидации данных",
                    Errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList()
                });
            }

            try
            {
                // Создаем команду на основе DTO
                var command = new CreateUserCommand
                {
                    Name = registerDto.Name,
                    Email = registerDto.Email,
                    Phone = registerDto.Phone,
                    Password = registerDto.Password,
                    UserType = registerDto.UserType,
                    UserRole = registerDto.UserRole
                };

                var result = await _mediator.Send(command);

                if (result.IsFailed)
                {
                    return BadRequest(new
                    {
                        Success = false,
                        Message = result.Errors.First().Message
                    });
                }

                return CreatedAtAction(
                    nameof(Login),
                    new { id = result.Value },
                    new
                    {
                        Success = true,
                        Message = "Пользователь успешно зарегистрирован",
                        UserId = result.Value
                    });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Success = false,
                    Message = "Внутренняя ошибка сервера при регистрации"
                });
            }
        }
    }
}
