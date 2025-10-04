using FluentResults;
using MediatR;
using B2B_API.Application.Commands;
using B2B_API.Domain.Entities;
using B2B_API.Domain.Interfaces;

namespace B2B_API.Application.Handlers
{
    /// <summary>
    /// Обработчик команды удаления продукта
    /// </summary>
    public class DeleteProductCommandHandler : BaseCommandHandler<DeleteProductCommand>
    {
        private readonly IRepository<Product> _productRepository;

        public DeleteProductCommandHandler(IRepository<Product> productRepository)
        {
            _productRepository = productRepository;
        }

        public override async Task<Result> Handle(DeleteProductCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var product = await _productRepository.GetByIdAsync(command.Id, cancellationToken);

                if (product == null)
                    return Result.Fail($"Продукт с ID {command.Id} не найден");

                // Проверяем, используется ли продукт в заказах
                // В будущем здесь можно добавить проверку через OrderItems

                await _productRepository.DeleteAsync(product, cancellationToken);

                return Result.Ok();
            }
            catch (Exception ex)
            {
                return Result.Fail($"Ошибка при удалении продукта: {ex.Message}");
            }
        }
    }
}
