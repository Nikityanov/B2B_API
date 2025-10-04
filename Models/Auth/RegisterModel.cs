using System.ComponentModel.DataAnnotations;

namespace B2B_API.Models.Auth
{
    /// <summary>
    /// Модель для регистрации пользователя
    /// </summary>
    public class RegisterModel
    {
        [Required(ErrorMessage = "Email обязателен")]
        [EmailAddress(ErrorMessage = "Некорректный формат email")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "Пароль обязателен")]
        [MinLength(6, ErrorMessage = "Пароль должен содержать минимум 6 символов")]
        public required string Password { get; set; }

        [Required(ErrorMessage = "Подтверждение пароля обязательно")]
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        public required string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Имя обязательно")]
        [MaxLength(100, ErrorMessage = "Имя не должно превышать 100 символов")]
        public required string FirstName { get; set; }

        [Required(ErrorMessage = "Фамилия обязательна")]
        [MaxLength(100, ErrorMessage = "Фамилия не должна превышать 100 символов")]
        public required string LastName { get; set; }

        [Phone(ErrorMessage = "Некорректный формат телефона")]
        public string? PhoneNumber { get; set; }

        [Required(ErrorMessage = "Тип пользователя обязателен")]
        [EnumDataType(typeof(UserType), ErrorMessage = "Некорректный тип пользователя")]
        public UserType UserType { get; set; } = UserType.Buyer;
    }

    /// <summary>
    /// Типы пользователей в системе
    /// </summary>
    public enum UserType
    {
        Buyer = 1,
        Seller = 2,
        Admin = 3
    }
}
