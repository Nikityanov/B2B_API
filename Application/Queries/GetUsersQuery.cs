using FluentResults;
using B2B_API.Application.Interfaces;
using B2B_API.Models.Enums;

namespace B2B_API.Application.Queries
{
    /// <summary>
    /// DTO для пользователя в ответе
    /// </summary>
    public class UserDto
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public UserType UserType { get; set; }
        public UserRole UserRole { get; set; }
        public required string UNP { get; set; }
        public string? OKPO { get; set; }
        public required string LegalAddress { get; set; }
        public required string ActualAddress { get; set; }
        public required string Email { get; set; }
        public required string Phone { get; set; }
        public string? BankName { get; set; }
        public string? BankAccount { get; set; }
        public string? BankBIK { get; set; }
        public string? INN { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
    }

    /// <summary>
    /// Запрос для получения списка пользователей с пагинацией и фильтрацией
    /// </summary>
    public class GetUsersQuery : IQuery<(List<UserDto> Items, int TotalCount)>
    {
        /// <summary>
        /// Номер страницы (начиная с 1)
        /// </summary>
        public int Page { get; set; } = 1;

        /// <summary>
        /// Размер страницы
        /// </summary>
        public int PageSize { get; set; } = 10;

        /// <summary>
        /// Поисковый термин для фильтрации по имени или email
        /// </summary>
        public string? SearchTerm { get; set; }

        /// <summary>
        /// Фильтр по типу пользователя
        /// </summary>
        public UserType? UserType { get; set; }

        /// <summary>
        /// Фильтр по роли пользователя
        /// </summary>
        public UserRole? UserRole { get; set; }

        /// <summary>
        /// Сортировка по полю
        /// </summary>
        public string? SortBy { get; set; }

        /// <summary>
        /// Порядок сортировки (asc/desc)
        /// </summary>
        public string? SortOrder { get; set; }
    }
}