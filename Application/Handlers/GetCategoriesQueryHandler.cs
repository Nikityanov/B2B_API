using FluentResults;
using MediatR;
using B2B_API.Application.Queries;
using B2B_API.Domain.Entities;
using B2B_API.Domain.Interfaces;

namespace B2B_API.Application.Handlers
{
    /// <summary>
    /// Обработчик запроса получения списка категорий с пагинацией
    /// </summary>
    public class GetCategoriesQueryHandler : BasePagedQueryHandler<GetCategoriesQuery, CategoryDto, List<CategoryDto>>
    {
        private readonly IRepository<Category> _categoryRepository;

        public GetCategoriesQueryHandler(IRepository<Category> categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

    public override async Task<Result<(List<CategoryDto> Items, int TotalCount)>> Handle(
            GetCategoriesQuery query,
            CancellationToken cancellationToken)
        {
            try
            {
                // Получаем общее количество категорий
                var totalCount = await _categoryRepository.CountAsync(cancellationToken: cancellationToken);

                // Получаем категории с пагинацией
                var categories = await _categoryRepository.GetAllAsync(cancellationToken);

                var categoryDtos = categories
                    .Skip((query.Page - 1) * query.PageSize)
                    .Take(query.PageSize)
                    .Select(MapToDto)
                    .ToList();

                return Result.Ok((Items: categoryDtos, TotalCount: totalCount));
            }
            catch (Exception ex)
            {
                return Result.Fail($"Ошибка при получении списка категорий: {ex.Message}");
            }
        }

        private static CategoryDto MapToDto(Category category)
        {
            return new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                ImageUrl = category.ImageUrl
            };
        }
    }
}
