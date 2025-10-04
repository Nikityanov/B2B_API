using FluentResults;
using MediatR;
using B2B_API.Application.Commands;
using B2B_API.Domain.Entities;
using B2B_API.Domain.Interfaces;

namespace B2B_API.Application.Handlers
{
    /// <summary>
    /// Обработчик команды удаления категории
    /// </summary>
    public class DeleteCategoryCommandHandler : BaseCommandHandler<DeleteCategoryCommand>
    {
        private readonly IRepository<Category> _categoryRepository;

        public DeleteCategoryCommandHandler(IRepository<Category> categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public override async Task<Result> Handle(DeleteCategoryCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var category = await _categoryRepository.GetByIdAsync(command.Id, cancellationToken);

                if (category == null)
                    return Result.Fail($"Категория с ID {command.Id} не найдена");

                // Проверяем, есть ли продукты в этой категории
                var productsInCategory = await _categoryRepository
                    .FindAsync(c => c.Id == command.Id)
                    .ContinueWith(t => t.Result.FirstOrDefault());

                if (productsInCategory != null)
                    return Result.Fail("Невозможно удалить категорию, содержащую продукты");

                await _categoryRepository.DeleteAsync(category, cancellationToken);

                return Result.Ok();
            }
            catch (Exception ex)
            {
                return Result.Fail($"Ошибка при удалении категории: {ex.Message}");
            }
        }
    }
}
