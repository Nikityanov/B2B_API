using FluentResults;
using FluentValidation;
using B2B_API.Application.Commands;
using B2B_API.Domain.Entities;
using B2B_API.Domain.Interfaces;
using B2B_API.CrossCutting.Validation;

namespace B2B_API.Application.Handlers
{
    /// <summary>
    /// Обработчик команды удаления продукта из прайс-листа
    /// </summary>
    public class RemoveProductFromPriceListCommandHandler : ValidatedCommandHandler<RemoveProductFromPriceListCommand>
    {
        private readonly IUnitOfWork _unitOfWork;

        public RemoveProductFromPriceListCommandHandler(
            IUnitOfWork unitOfWork,
            RemoveProductFromPriceListCommandValidator validator) : base(validator)
        {
            _unitOfWork = unitOfWork;
        }

        protected override async Task<Result> HandleValidated(RemoveProductFromPriceListCommand command, CancellationToken cancellationToken)
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
                    return Result.Fail("Невозможно удалить продукт из неактивного прайс-листа");

                // Удаляем связь между прайс-листом и продуктом
                await _unitOfWork.PriceListProducts.DeleteAsync(priceListProduct, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return Result.Ok();
            }
            catch (Exception ex)
            {
                return Result.Fail($"Ошибка при удалении продукта из прайс-листа: {ex.Message}");
            }
        }
    }
}