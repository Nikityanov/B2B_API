using FluentResults;
using FluentValidation;
using B2B_API.Application.Commands;
using B2B_API.Domain.Entities;
using B2B_API.Domain.Interfaces;
using B2B_API.CrossCutting.Validation;

namespace B2B_API.Application.Handlers
{
    /// <summary>
    /// Обработчик команды создания продукта
    /// </summary>
    public class CreateProductCommandHandler : ValidatedCommandHandler<CreateProductCommand, int>
    {
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<Category> _categoryRepository;

        public CreateProductCommandHandler(
            IRepository<Product> productRepository,
            IRepository<Category> categoryRepository,
            CreateProductCommandValidator validator) : base(validator)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
        }

        protected override async Task<Result<int>> HandleValidated(CreateProductCommand command, CancellationToken cancellationToken)
        {
            try
            {
                // Проверяем уникальность SKU (бизнес-валидация)
                var existingProduct = await _productRepository
                    .FindAsync(p => p.SKU.ToLower() == command.SKU.ToLower())
                    .ContinueWith(t => t.Result.FirstOrDefault());

                if (existingProduct != null)
                    return Result.Fail<int>("Продукт с таким артикулом уже существует");

                // Проверяем существование категории, если указана
                if (command.CategoryId.HasValue)
                {
                    var category = await _categoryRepository.GetByIdAsync(command.CategoryId.Value, cancellationToken);
                    if (category == null)
                        return Result.Fail<int>($"Категория с ID {command.CategoryId.Value} не найдена");
                }

                // Создаем новый продукт
                var product = new Product
                {
                    Name = command.Name,
                    Description = command.Description,
                    StockQuantity = command.StockQuantity,
                    Price = command.Price,
                    SKU = command.SKU,
                    Manufacturer = command.Manufacturer,
                    Unit = command.Unit,
                    ImageUrl = command.ImageUrl,
                    ImageGallery = command.ImageGallery ?? new List<string>(),
                    Characteristics = command.Characteristics,
                    CategoryId = command.CategoryId
                };

                // Сохраняем в репозиторий
                var createdProduct = await _productRepository.AddAsync(product, cancellationToken);

                return Result.Ok(createdProduct.Id);
            }
            catch (Exception ex)
            {
                return Result.Fail<int>($"Ошибка при создании продукта: {ex.Message}");
            }
        }
    }
}
