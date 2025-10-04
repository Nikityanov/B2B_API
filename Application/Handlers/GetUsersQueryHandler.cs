using FluentResults;
using B2B_API.Application.Queries;
using B2B_API.Domain.Entities;
using B2B_API.Domain.Interfaces;

namespace B2B_API.Application.Handlers
{
    /// <summary>
    /// Обработчик запроса получения списка пользователей с пагинацией и фильтрацией
    /// </summary>
    public class GetUsersQueryHandler : BasePagedQueryHandler<GetUsersQuery, UserDto, List<UserDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        /// <summary>
        /// Конструктор обработчика запроса пользователей
        /// </summary>
        /// <param name="unitOfWork">Unit of Work для работы с репозиториями</param>
        public GetUsersQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Обрабатывает запрос получения списка пользователей с пагинацией и фильтрацией
        /// </summary>
        /// <param name="query">Запрос с параметрами пагинации и фильтрации</param>
        /// <param name="cancellationToken">Токен отмены операции</param>
        /// <returns>Результат выполнения запроса с данными пользователей</returns>
        public override async Task<Result<(List<UserDto> Items, int TotalCount)>> Handle(
            GetUsersQuery query,
            CancellationToken cancellationToken)
        {
            try
            {
                // Получаем пользователей с применением фильтров
                var users = await _unitOfWork.Users.GetAllAsync(cancellationToken);

                // Применяем фильтры
                var filteredUsers = ApplyFilters(users, query);

                // Применяем сортировку
                var sortedUsers = ApplySorting(filteredUsers, query);

                // Получаем общее количество пользователей после фильтрации
                var totalCount = sortedUsers.Count();

                // Применяем пагинацию
                var pagedUsers = sortedUsers
                    .Skip((query.Page - 1) * query.PageSize)
                    .Take(query.PageSize)
                    .ToList();

                // Маппим в DTO
                var userDtos = pagedUsers.Select(MapToDto).ToList();

                return Result.Ok((Items: userDtos, TotalCount: totalCount));
            }
            catch (Exception ex)
            {
                return Result.Fail($"Ошибка при получении списка пользователей: {ex.Message}");
            }
        }

        /// <summary>
        /// Применяет фильтры к списку пользователей
        /// </summary>
        /// <param name="users">Исходный список пользователей</param>
        /// <param name="query">Запрос с параметрами фильтрации</param>
        /// <returns>Отфильтрованный список пользователей</returns>
        private static IQueryable<User> ApplyFilters(IEnumerable<User> users, GetUsersQuery query)
        {
            var queryableUsers = users.AsQueryable();

            // Фильтр по поисковому термину (имя или email)
            if (!string.IsNullOrWhiteSpace(query.SearchTerm))
            {
                var searchTerm = query.SearchTerm.ToLower();
                queryableUsers = queryableUsers.Where(u =>
                    u.Name.ToLower().Contains(searchTerm) ||
                    u.Email.ToLower().Contains(searchTerm));
            }

            // Фильтр по типу пользователя
            if (query.UserType.HasValue)
            {
                queryableUsers = queryableUsers.Where(u => u.UserType == query.UserType.Value);
            }

            // Фильтр по роли пользователя
            if (query.UserRole.HasValue)
            {
                queryableUsers = queryableUsers.Where(u => u.UserRole == query.UserRole.Value);
            }

            return queryableUsers;
        }

        /// <summary>
        /// Применяет сортировку к списку пользователей
        /// </summary>
        /// <param name="users">Исходный список пользователей</param>
        /// <param name="query">Запрос с параметрами сортировки</param>
        /// <returns>Отсортированный список пользователей</returns>
        private static IQueryable<User> ApplySorting(IQueryable<User> users, GetUsersQuery query)
        {
            if (string.IsNullOrWhiteSpace(query.SortBy))
            {
                // Сортировка по умолчанию - по дате создания (новые первыми)
                return users.OrderByDescending(u => u.CreatedAt);
            }

            var sortOrder = query.SortOrder?.ToLower() == "desc";

            return query.SortBy.ToLower() switch
            {
                "name" => sortOrder ? users.OrderByDescending(u => u.Name) : users.OrderBy(u => u.Name),
                "email" => sortOrder ? users.OrderByDescending(u => u.Email) : users.OrderBy(u => u.Email),
                "createdat" => sortOrder ? users.OrderByDescending(u => u.CreatedAt) : users.OrderBy(u => u.CreatedAt),
                "usertype" => sortOrder ? users.OrderByDescending(u => u.UserType) : users.OrderBy(u => u.UserType),
                "userrole" => sortOrder ? users.OrderByDescending(u => u.UserRole) : users.OrderBy(u => u.UserRole),
                _ => users.OrderByDescending(u => u.CreatedAt) // Сортировка по умолчанию
            };
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