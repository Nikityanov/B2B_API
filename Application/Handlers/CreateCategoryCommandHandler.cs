using FluentResults;
using FluentValidation;
using B2B_API.Application.Commands;
using B2B_API.Domain.Entities;
using B2B_API.Domain.Interfaces;
using B2B_API.CrossCutting.Validation;

namespace B2B_API.Application.Handlers
{
    /// <summary>
    /// Обработчик команды создания категории
    /// </summary>
    public class CreateCategoryCommandHandler : ValidatedCommandHandler<CreateCategoryCommand, int>
    {
        private readonly IRepository<Category> _categoryRepository;

        public CreateCategoryCommandHandler(
            IRepository<Category> categoryRepository,
            CreateCategoryCommandValidator validator) : base(validator)
        {
            _categoryRepository = categoryRepository;
        }

        protected override async Task<Result<int>> HandleValidated(CreateCategoryCommand command, CancellationToken cancellationToken)
        {
            try
            {
                // Проверяем уникальность названия категории (бизнес-валидация)
                var existingCategory = await _categoryRepository
                    .FindAsync(c => c.Name.ToLower() == command.Name.ToLower())
                    .ContinueWith(t => t.Result.FirstOrDefault());

                if (existingCategory != null)
                    return Result.Fail<int>("Категория с таким названием уже существует");

                // Создаем новую категорию
                var category = new Category
                {
                    Name = command.Name,
                    Description = command.Description,
                    ImageUrl = command.ImageUrl
                };

                // Сохраняем в репозиторий
                var createdCategory = await _categoryRepository.AddAsync(category, cancellationToken);

                return Result.Ok(createdCategory.Id);
            }
            catch (Exception ex)
            {
                return Result.Fail<int>($"Ошибка при создании категории: {ex.Message}");
            }
        }
    }
}
