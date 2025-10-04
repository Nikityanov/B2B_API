using FluentResults;
using B2B_API.Application.Interfaces;

namespace B2B_API.Application.Queries
{
    /// <summary>
    /// Запрос для получения пользователя по ID
    /// </summary>
    public class GetUserByIdQuery : IQuery<UserDto>
    {
        /// <summary>
        /// Идентификатор пользователя
        /// </summary>
        public int Id { get; set; }
    }
}