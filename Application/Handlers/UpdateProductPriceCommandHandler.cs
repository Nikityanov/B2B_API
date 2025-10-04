using FluentResults;
using FluentValidation;
using B2B_API.Application.Commands;
using B2B_API.Domain.Entities;
using B2B_API.Domain.Interfaces;
using B2B_API.CrossCutting.Validation;

namespace B2B_API.Application.Handlers
{
    /// <summary>
    /// Обработчик команды обновления цены продукта в прайс-листе
    /// </summary>
    public class UpdateProductPriceCommandHandler : ValidatedCommandHandler<UpdateProductPriceCommand>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateProductPriceCommandHandler(
            IUnitOfWork unitOfWork,
            UpdateProductPriceCommandValidator validator) : base(validator)
        {
            _unitOfWork = unitOfWork;
        }

        protected override async Task<Result> HandleValidated(UpdateProductPriceCommand command, CancellationToken cancellationToken)
        {
            try
            {
                // Получаем связь продукт-прайс-лист
                var priceListProduct = await _unitOfWork.PriceLists
                    .FindAsync(pl => pl.Id == command.PriceListId)
                    .ContinueWith(t => t.Result.FirstOrDefault()?.PriceListProducts
                        .FirstOrDefault(plp => plp.ProductId == command.ProductId));

                if (priceListProduct == null)
                    return Result.Fail($"Продукт с ID {command.ProductId} не найден в прайс-листе с ID {command.PriceListId}");

                // Проверяем, активен ли прайс-лист
                var priceList = await _unitOfWork.PriceLists.GetByIdAsync(command.PriceListId, cancellationToken);
                if (priceList == null)
                    return Result.Fail($"Прайс-лист с ID {command.PriceListId} не найден");

                if (!priceList.IsActive)
                    return Result.Fail("Невозможно обновить цену продукта в неактивном прайс-листе");

                // Обновляем специальную цену, если указана
                if (command.SpecialPrice.HasValue)
                    priceListProduct.SpecialPrice = command.SpecialPrice.Value;

                // Обновляем статус активности, если указан
                if (command.IsActive.HasValue)
                    priceListProduct.IsActive = command.IsActive.Value;

                // Сохраняем изменения
                await _unitOfWork.PriceListProducts.UpdateAsync(priceListProduct, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return Result.Ok();
            }
            catch (Exception ex)
            {
                return Result.Fail($"Ошибка при обновлении цены продукта: {ex.Message}");
            }
        }
    }
}