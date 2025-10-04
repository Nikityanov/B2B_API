using FluentResults;
using MediatR;
using B2B_API.Application.Commands;
using B2B_API.Domain.Entities;
using B2B_API.Domain.Interfaces;

namespace B2B_API.Application.Handlers
{
    /// <summary>
    /// Обработчик команды обновления продукта
    /// </summary>
    public class UpdateProductCommandHandler : BaseCommandHandler<UpdateProductCommand>
    {
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<Category> _categoryRepository;

        public UpdateProductCommandHandler(
            IRepository<Product> productRepository,
            IRepository<Category> categoryRepository)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
        }

        public override async Task<Result> Handle(UpdateProductCommand command, CancellationToken cancellationToken)
        {
            // Валидация команды
            var validationResult = await ValidateCommand(command);
            if (validationResult.IsFailed)
                return validationResult;

            try
            {
                var product = await _productRepository.GetByIdAsync(command.Id, cancellationToken);

                if (product == null)
                    return Result.Fail($"Продукт с ID {command.Id} не найден");

                // Обновляем свойства продукта
                product.Name = command.Name;
                product.Description = command.Description;
                product.StockQuantity = command.StockQuantity;
                product.Price = command.Price;
                product.SKU = command.SKU;
                product.Manufacturer = command.Manufacturer;
                product.Unit = command.Unit;
                product.ImageUrl = command.ImageUrl;
                product.ImageGallery = command.ImageGallery ?? new List<string>();
                product.Characteristics = command.Characteristics;
                product.CategoryId = command.CategoryId;
                product.UpdateModifiedDate();

                await _productRepository.UpdateAsync(product, cancellationToken);

                return Result.Ok();
            }
            catch (Exception ex)
            {
                return Result.Fail($"Ошибка при обновлении продукта: {ex.Message}");
            }
        }

        protected override async Task<Result> ValidateCommand(UpdateProductCommand command)
        {
            if (string.IsNullOrWhiteSpace(command.Name))
                return Result.Fail("Название продукта обязательно для заполнения");

            if (command.Name.Length > 200)
                return Result.Fail("Название продукта не должно превышать 200 символов");

            if (command.StockQuantity < 0)
                return Result.Fail("Количество на складе не может быть отрицательным");

            if (command.Price <= 0)
                return Result.Fail("Цена продукта должна быть больше нуля");

            if (string.IsNullOrWhiteSpace(command.SKU))
                return Result.Fail("Артикул (SKU) обязателен для заполнения");

            // Проверяем уникальность SKU (исключая текущий продукт)
            var existingProduct = await _productRepository
                .FindAsync(p => p.SKU.ToLower() == command.SKU.ToLower() && p.Id != command.Id)
                .ContinueWith(t => t.Result.FirstOrDefault());

            if (existingProduct != null)
                return Result.Fail("Продукт с таким артикулом уже существует");

            // Проверяем существование категории, если указана
            if (command.CategoryId.HasValue)
            {
                var category = await _categoryRepository.GetByIdAsync(command.CategoryId.Value, CancellationToken.None);
                if (category == null)
                    return Result.Fail($"Категория с ID {command.CategoryId.Value} не найдена");
            }

            return Result.Ok();
        }
    }
}
