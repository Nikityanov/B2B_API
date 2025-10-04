using System.ComponentModel.DataAnnotations;

namespace B2B_API.API.DTOs
{
    /// <summary>
    /// DTO для входа пользователя в систему
    /// </summary>
    public class LoginRequestDto
    {
        [Required(ErrorMessage = "Email обязателен для заполнения")]
        [MaxLength(100, ErrorMessage = "Email не должен превышать 100 символов")]
        [EmailAddress(ErrorMessage = "Некорректный формат email")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "Пароль обязателен для заполнения")]
        public required string Password { get; set; }
    }
}