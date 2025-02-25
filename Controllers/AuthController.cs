using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using B2B_API.Models;
using B2B_API.Models.Auth;
using B2B_API.Services;
using B2B_API.Data;
using B2B_API.Models.Enums;

namespace B2B_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly JwtService _jwtService;

        public AuthController(ApplicationDbContext context, JwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<User>> Register(RegisterModel model)
        {
            if (_context.Users != null && await _context.Users.AnyAsync(u => u.Email == model.Email))
            {
                return BadRequest("Email already exists");
            }

            // Создаем нового пользователя с минимальным набором данных
            var user = new User
            {
                Email = model.Email,
                UserRole = model.UserType == UserType.Individual ? UserRole.Seller : UserRole.Buyer,
                UserType = model.UserType,
                Name = "Не указано",  // Временное значение
                INN = "000000000000", // Временное значение
                LegalAddress = "Не указано", // Временное значение
                ActualAddress = "Не указано", // Временное значение
                Phone = "Не указано", // Временное значение
                UNP = "000000000",
                PasswordHash = Array.Empty<byte>() // Временное значение, будет перезаписано ниже
            };
            // Хешируем пароль
            // Генерация соли и хеша с использованием PBKDF2
            const int iterations = 600000;
            using var pbkdf2 = new Rfc2898DeriveBytes(
                model.Password,
                32, // Размер соли
                iterations,
                HashAlgorithmName.SHA512);
            
            user.PasswordSalt = pbkdf2.Salt;
            user.PasswordHash = pbkdf2.GetBytes(64); // 64 байта для хеша

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Генерируем токен для нового пользователя
            var token = _jwtService.GenerateToken(user);

            return Ok(new {
                token,
                userId = user.Id,
                message = "Пользователь успешно зарегистрирован. Пожалуйста, заполните оставшиеся данные в профиле."
            });
        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(LoginModel loginModel)
        {
            var user = await _context.Users?.FirstOrDefaultAsync(u => u.Email == loginModel.Email);
            if (user == null)
            {
                return Unauthorized("Неверный email или пароль");
            }

            // Проверяем пароль
            var saltBytes = user.PasswordSalt;
            using var pbkdf2 = new Rfc2898DeriveBytes(
                loginModel.Password,
                saltBytes,
                600000,
                HashAlgorithmName.SHA512);
            
            var computedHash = pbkdf2.GetBytes(64);

            if (!computedHash.SequenceEqual(user.PasswordHash))
            {
                return Unauthorized("Неверный email или пароль");
            }

            // Генерируем JWT токен
            var token = _jwtService.GenerateToken(user);
            
            return Ok(new { 
                token,
                userId = user.Id,
                isProfileComplete = IsProfileComplete(user)
            });
        }

        private bool IsProfileComplete(User user)
        {
            return !string.IsNullOrEmpty(user.Name) &&
                   user.Name != "Не указано" &&
                   !string.IsNullOrEmpty(user.INN) &&
                   user.INN != "000000000000" &&
                   !string.IsNullOrEmpty(user.LegalAddress) &&
                   user.LegalAddress != "Не указано" &&
                   !string.IsNullOrEmpty(user.ActualAddress) &&
                   user.ActualAddress != "Не указано" &&
                   !string.IsNullOrEmpty(user.Phone) &&
                   user.Phone != "Не указано";
        }
    }
}
