using FluentResults;
using FluentValidation;
using B2B_API.Application.Commands;
using B2B_API.Domain.Entities;
using B2B_API.Domain.Interfaces;
using B2B_API.CrossCutting.Validation;

namespace B2B_API.Application.Handlers
{
    /// <summary>
    /// Обработчик команды удаления прайс-листа
    /// </summary>
    public class DeletePriceListCommandHandler : ValidatedCommandHandler<DeletePriceListCommand>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeletePriceListCommandHandler(
            IUnitOfWork unitOfWork,
            DeletePriceListCommandValidator validator) : base(validator)
        {
            _unitOfWork = unitOfWork;
        }

        protected override async Task<Result> HandleValidated(DeletePriceListCommand command, CancellationToken cancellationToken)
        {
            try
            {
                // Получаем прайс-лист для удаления
                var priceList = await _unitOfWork.PriceLists.GetByIdAsync(command.Id, cancellationToken);
                if (priceList == null)
                    return Result.Fail($"Прайс-лист с ID {command.Id} не найден");

                // Проверяем, есть ли активные продукты в прайс-листе
                var activeProducts = await _unitOfWork.PriceLists
                    .FindAsync(pl => pl.Id == command.Id)
                    .ContinueWith(t => t.Result.FirstOrDefault()?.PriceListProducts
                        .Where(plp => plp.IsActive).ToList());

                if (activeProducts != null && activeProducts.Any())
                    return Result.Fail("Невозможно удалить прайс-лист, содержащий активные продукты. Сначала удалите все продукты из прайс-листа.");

                // Удаляем прайс-лист
                await _unitOfWork.PriceLists.DeleteAsync(priceList, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return Result.Ok();
            }
            catch (Exception ex)
            {
                return Result.Fail($"Ошибка при удалении прайс-листа: {ex.Message}");
            }
        }
    }
}