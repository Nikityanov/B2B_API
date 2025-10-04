using FluentResults;
using FluentValidation;
using B2B_API.Application.Commands;
using B2B_API.Domain.Entities;
using B2B_API.Domain.Interfaces;
using B2B_API.CrossCutting.Validation;

namespace B2B_API.Application.Handlers
{
    /// <summary>
    /// Обработчик команды обновления прайс-листа
    /// </summary>
    public class UpdatePriceListCommandHandler : ValidatedCommandHandler<UpdatePriceListCommand>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdatePriceListCommandHandler(
            IUnitOfWork unitOfWork,
            UpdatePriceListCommandValidator validator) : base(validator)
        {
            _unitOfWork = unitOfWork;
        }

        protected override async Task<Result> HandleValidated(UpdatePriceListCommand command, CancellationToken cancellationToken)
        {
            try
            {
                // Получаем прайс-лист для обновления
                var priceList = await _unitOfWork.PriceLists.GetByIdAsync(command.Id, cancellationToken);
                if (priceList == null)
                    return Result.Fail($"Прайс-лист с ID {command.Id} не найден");

                // Проверяем уникальность названия, если оно изменяется
                if (!string.IsNullOrEmpty(command.Name) && priceList.Name != command.Name)
                {
                    var existingPriceList = await _unitOfWork.PriceLists
                        .FindAsync(pl => pl.Name.ToLower() == command.Name.ToLower() && pl.SellerId == priceList.SellerId && pl.Id != command.Id)
                        .ContinueWith(t => t.Result.FirstOrDefault());

                    if (existingPriceList != null)
                        return Result.Fail("Прайс-лист с таким названием уже существует у данного продавца");
                }

                // Обновляем поля прайс-листа
                if (!string.IsNullOrEmpty(command.Name))
                    priceList.Name = command.Name;

                if (!string.IsNullOrEmpty(command.Description))
                    priceList.Description = command.Description;

                if (!string.IsNullOrEmpty(command.Currency))
                    priceList.Currency = command.Currency;

                if (command.IsActive.HasValue)
                    priceList.IsActive = command.IsActive.Value;

                // Сохраняем изменения
                await _unitOfWork.PriceLists.UpdateAsync(priceList, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return Result.Ok();
            }
            catch (Exception ex)
            {
                return Result.Fail($"Ошибка при обновлении прайс-листа: {ex.Message}");
            }
        }
    }
}