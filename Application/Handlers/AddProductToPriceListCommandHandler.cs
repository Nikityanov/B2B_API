using FluentResults;
using FluentValidation;
using B2B_API.Application.Commands;
using B2B_API.Domain.Entities;
using B2B_API.Domain.Interfaces;
using B2B_API.CrossCutting.Validation;

namespace B2B_API.Application.Handlers
{
    /// <summary>
    /// Обработчик команды добавления продукта в прайс-лист
    /// </summary>
    public class AddProductToPriceListCommandHandler : ValidatedCommandHandler<AddProductToPriceListCommand, int>
    {
        private readonly IUnitOfWork _unitOfWork;

        public AddProductToPriceListCommandHandler(
            IUnitOfWork unitOfWork,
            AddProductToPriceListCommandValidator validator) : base(validator)
        {
            _unitOfWork = unitOfWork;
        }

        protected override async Task<Result<int>> HandleValidated(AddProductToPriceListCommand command, CancellationToken cancellationToken)
        {
            try
            {
                // Проверяем существование прайс-листа
                var priceList = await _unitOfWork.PriceLists.GetByIdAsync(command.PriceListId, cancellationToken);
                if (priceList == null)
                    return Result.Fail<int>($"Прайс-лист с ID {command.PriceListId} не найден");

                // Проверяем существование продукта
                var product = await _unitOfWork.Products.GetByIdAsync(command.ProductId, cancellationToken);
                if (product == null)
                    return Result.Fail<int>($"Продукт с ID {command.ProductId} не найден");

                // Проверяем, активен ли прайс-лист
                if (!priceList.IsActive)
                    return Result.Fail<int>("Невозможно добавить продукт в неактивный прайс-лист");

                // Проверяем, не добавлен ли уже этот продукт в прайс-лист
                var existingPriceListProduct = await _unitOfWork.PriceListProducts
                    .FindAsync(plp => plp.PriceListId == command.PriceListId && plp.ProductId == command.ProductId);

                if (existingPriceListProduct != null)
                    return Result.Fail<int>("Данный продукт уже добавлен в прайс-лист");

                // Создаем связь между прайс-листом и продуктом
                var priceListProduct = new PriceListProduct
                {
                    PriceListId = command.PriceListId,
                    ProductId = command.ProductId,
                    SpecialPrice = command.SpecialPrice,
                    IsActive = true
                };

                // Сохраняем в репозиторий
                var createdPriceListProduct = await _unitOfWork.PriceListProducts.AddAsync(priceListProduct, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return Result.Ok(createdPriceListProduct.Id);
            }
            catch (Exception ex)
            {
                return Result.Fail<int>($"Ошибка при добавлении продукта в прайс-лист: {ex.Message}");
            }
        }
    }
}