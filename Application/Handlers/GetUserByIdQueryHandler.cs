using FluentResults;
using B2B_API.Application.Queries;
using B2B_API.Domain.Entities;
using B2B_API.Domain.Interfaces;

namespace B2B_API.Application.Handlers
{
    /// <summary>
    /// Обработчик запроса получения пользователя по ID
    /// </summary>
    public class GetUserByIdQueryHandler : BaseQueryHandler<GetUserByIdQuery, UserDto>
    {
        private readonly IUnitOfWork _unitOfWork;

        /// <summary>
        /// Конструктор обработчика запроса пользователя по ID
        /// </summary>
        /// <param name="unitOfWork">Unit of Work для работы с репозиториями</param>
        public GetUserByIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Обрабатывает запрос получения пользователя по ID
        /// </summary>
        /// <param name="query">Запрос с идентификатором пользователя</param>
        /// <param name="cancellationToken">Токен отмены операции</param>
        /// <returns>Результат выполнения запроса с данными пользователя</returns>
        public override async Task<Result<UserDto>> Handle(GetUserByIdQuery query, CancellationToken cancellationToken)
        {
            try
            {
                // Получаем пользователя по ID
                var user = await _unitOfWork.Users.GetByIdAsync(query.Id, cancellationToken);

                // Проверяем, что пользователь найден
                if (user == null)
                    return Result.Fail($"Пользователь с ID {query.Id} не найден");

                // Маппим в DTO
                var userDto = MapToDto(user);
                return Result.Ok(userDto);
            }
            catch (Exception ex)
            {
                return Result.Fail($"Ошибка при получении пользователя: {ex.Message}");
            }
        }

        /// <summary>
        /// Маппит доменную сущность User в DTO модель
        /// </summary>
        /// <param name="user">Доменная сущность пользователя</param>
        /// <returns>DTO модель пользователя</returns>
        private static UserDto MapToDto(User user)
        {
            return new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                UserType = user.UserType,
                UserRole = user.UserRole,
                UNP = user.UNP,
                OKPO = user.OKPO,
                LegalAddress = user.LegalAddress,
                ActualAddress = user.ActualAddress,
                Email = user.Email,
                Phone = user.Phone,
                BankName = user.BankName,
                BankAccount = user.BankAccount,
                BankBIK = user.BankBIK,
                INN = user.INN,
                CreatedAt = user.CreatedAt,
                ModifiedAt = user.ModifiedAt
            };
        }
    }
}