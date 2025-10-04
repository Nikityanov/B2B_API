using FluentResults;
using FluentValidation;
using B2B_API.Application.Commands;
using B2B_API.Domain.Entities;
using B2B_API.Domain.Interfaces;
using B2B_API.CrossCutting.Validation;

namespace B2B_API.Application.Handlers
{
    /// <summary>
    /// Обработчик команды создания прайс-листа
    /// </summary>
    public class CreatePriceListCommandHandler : ValidatedCommandHandler<CreatePriceListCommand, int>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreatePriceListCommandHandler(
            IUnitOfWork unitOfWork,
            CreatePriceListCommandValidator validator) : base(validator)
        {
            _unitOfWork = unitOfWork;
        }

        protected override async Task<Result<int>> HandleValidated(CreatePriceListCommand command, CancellationToken cancellationToken)
        {
            try
            {
                // Проверяем существование продавца
                var seller = await _unitOfWork.Users.GetByIdAsync(command.SellerId, cancellationToken);
                if (seller == null)
                    return Result.Fail<int>($"Продавец с ID {command.SellerId} не найден");

                // Проверяем уникальность названия прайс-листа для данного продавца
                var existingPriceList = await _unitOfWork.PriceLists
                    .FindAsync(pl => pl.Name.ToLower() == command.Name.ToLower() && pl.SellerId == command.SellerId)
                    .ContinueWith(t => t.Result.FirstOrDefault());

                if (existingPriceList != null)
                    return Result.Fail<int>("Прайс-лист с таким названием уже существует у данного продавца");

                // Создаем новый прайс-лист
                var priceList = new PriceList
                {
                    Name = command.Name,
                    Description = command.Description,
                    Currency = command.Currency,
                    SellerId = command.SellerId,
                    IsActive = true
                };

                // Сохраняем в репозиторий
                var createdPriceList = await _unitOfWork.PriceLists.AddAsync(priceList, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return Result.Ok(createdPriceList.Id);
            }
            catch (Exception ex)
            {
                return Result.Fail<int>($"Ошибка при создании прайс-листа: {ex.Message}");
            }
        }
    }
}