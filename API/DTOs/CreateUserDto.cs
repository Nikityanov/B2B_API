using System.ComponentModel.DataAnnotations;
using B2B_API.Models.Enums;

namespace B2B_API.API.DTOs
{
    /// <summary>
    /// DTO для создания нового пользователя
    /// </summary>
    public class CreateUserDto
    {
        [Required(ErrorMessage = "Имя обязательно для заполнения")]
        [MaxLength(200, ErrorMessage = "Имя не должно превышать 200 символов")]
        public required string Name { get; set; }

        [Required(ErrorMessage = "Тип пользователя обязателен")]
        public UserType UserType { get; set; }

        [Required(ErrorMessage = "Роль пользователя обязательна")]
        public UserRole UserRole { get; set; }

        [Required(ErrorMessage = "Email обязателен для заполнения")]
        [MaxLength(100, ErrorMessage = "Email не должен превышать 100 символов")]
        [EmailAddress(ErrorMessage = "Некорректный формат email")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "Телефон обязателен для заполнения")]
        [MaxLength(20, ErrorMessage = "Телефон не должен превышать 20 символов")]
        [Phone(ErrorMessage = "Некорректный формат телефона")]
        public required string Phone { get; set; }

        [Required(ErrorMessage = "Пароль обязателен для заполнения")]
        [MinLength(6, ErrorMessage = "Пароль должен содержать минимум 6 символов")]
        public required string Password { get; set; }
    }
}