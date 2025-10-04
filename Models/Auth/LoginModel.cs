using System.ComponentModel.DataAnnotations;

namespace B2B_API.Models.Auth
{
    /// <summary>
    /// Модель для входа в систему
    /// </summary>
    public class LoginModel
    {
        [Required(ErrorMessage = "Email обязателен")]
        [EmailAddress(ErrorMessage = "Некорректный формат email")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "Пароль обязателен")]
        [MinLength(6, ErrorMessage = "Пароль должен содержать минимум 6 символов")]
        public required string Password { get; set; }
    }
}
