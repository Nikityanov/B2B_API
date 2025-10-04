using FluentResults;
using MediatR;
using B2B_API.Application.Queries;
using B2B_API.Domain.Entities;
using B2B_API.Domain.Interfaces;

namespace B2B_API.Application.Handlers
{
    /// <summary>
    /// Обработчик запроса получения категории по ID
    /// </summary>
    public class GetCategoryByIdQueryHandler : BaseQueryHandler<GetCategoryByIdQuery, CategoryDto>
    {
        private readonly IRepository<Category> _categoryRepository;

        public GetCategoryByIdQueryHandler(IRepository<Category> categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public override async Task<Result<CategoryDto>> Handle(GetCategoryByIdQuery query, CancellationToken cancellationToken)
        {
            try
            {
                var category = await _categoryRepository.GetByIdAsync(query.Id, cancellationToken);

                if (category == null)
                    return Result.Fail($"Категория с ID {query.Id} не найдена");

                var categoryDto = MapToDto(category);
                return Result.Ok(categoryDto);
            }
            catch (Exception ex)
            {
                return Result.Fail($"Ошибка при получении категории: {ex.Message}");
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
