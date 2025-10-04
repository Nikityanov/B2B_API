using System.ComponentModel.DataAnnotations;

namespace B2B_API.Models.Auth
{
    /// <summary>
    /// Настройки JWT токенов
    /// </summary>
    public class JwtSettings
    {
        public const string SectionName = "JwtSettings";

        [Required]
        [MinLength(32, ErrorMessage = "SecretKey должен быть минимум 32 символа")]
        public required string SecretKey { get; set; }

        [Required]
        public required string Issuer { get; set; }

        [Required]
        public required string Audience { get; set; }

        [Range(1, 1440, ErrorMessage = "ExpirationInMinutes должен быть от 1 до 1440 минут")]
        public int ExpirationInMinutes { get; set; } = 60;
    }
}
