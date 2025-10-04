using FluentResults;
using MediatR;
using B2B_API.Application.Commands;
using B2B_API.Domain.Entities;
using B2B_API.Domain.Interfaces;

namespace B2B_API.Application.Handlers
{
    /// <summary>
    /// Обработчик команды обновления категории
    /// </summary>
    public class UpdateCategoryCommandHandler : BaseCommandHandler<UpdateCategoryCommand>
    {
        private readonly IRepository<Category> _categoryRepository;

        public UpdateCategoryCommandHandler(IRepository<Category> categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public override async Task<Result> Handle(UpdateCategoryCommand command, CancellationToken cancellationToken)
        {
            // Валидация команды
            var validationResult = await ValidateCommand(command);
            if (validationResult.IsFailed)
                return validationResult;

            try
            {
                var category = await _categoryRepository.GetByIdAsync(command.Id, cancellationToken);

                if (category == null)
                    return Result.Fail($"Категория с ID {command.Id} не найдена");

                // Обновляем свойства категории
                category.Name = command.Name;
                category.Description = command.Description;
                category.ImageUrl = command.ImageUrl;
                category.UpdateModifiedDate();

                await _categoryRepository.UpdateAsync(category, cancellationToken);

                return Result.Ok();
            }
            catch (Exception ex)
            {
                return Result.Fail($"Ошибка при обновлении категории: {ex.Message}");
            }
        }

        protected override async Task<Result> ValidateCommand(UpdateCategoryCommand command)
        {
            if (string.IsNullOrWhiteSpace(command.Name))
                return Result.Fail("Название категории обязательно для заполнения");

            if (command.Name.Length > 100)
                return Result.Fail("Название категории не должно превышать 100 символов");

            // Проверяем уникальность названия категории (исключая текущую категорию)
            var existingCategory = await _categoryRepository
                .FindAsync(c => c.Name.ToLower() == command.Name.ToLower() && c.Id != command.Id)
                .ContinueWith(t => t.Result.FirstOrDefault());

            if (existingCategory != null)
                return Result.Fail("Категория с таким названием уже существует");

            return Result.Ok();
        }
    }
}
