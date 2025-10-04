using FluentResults;
using FluentValidation;
using MediatR;
using B2B_API.Application.Interfaces;

namespace B2B_API.Application.Handlers
{
    /// <summary>
    /// Базовый класс для обработчиков команд с валидацией
    /// </summary>
    public abstract class ValidatedCommandHandler<TCommand> : IRequestHandler<TCommand, Result>
        where TCommand : ICommand
    {
        private readonly IValidator<TCommand> _validator;

        protected ValidatedCommandHandler(IValidator<TCommand> validator)
        {
            _validator = validator;
        }

        public async Task<Result> Handle(TCommand command, CancellationToken cancellationToken)
        {
            // Выполняем валидацию команды
            var validationResult = await _validator.ValidateAsync(command, cancellationToken);

            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors
                    .Select(error => error.ErrorMessage)
                    .ToList();

                return Result.Fail(string.Join("; ", errors));
            }

            // Выполняем бизнес-логику
            return await HandleValidated(command, cancellationToken);
        }

        /// <summary>
        /// Обработка команды после успешной валидации
        /// </summary>
        protected abstract Task<Result> HandleValidated(TCommand command, CancellationToken cancellationToken);
    }

    /// <summary>
    /// Базовый класс для обработчиков команд с результатом и валидацией
    /// </summary>
    public abstract class ValidatedCommandHandler<TCommand, TResult> : IRequestHandler<TCommand, Result<TResult>>
        where TCommand : ICommand<TResult>
    {
        private readonly IValidator<TCommand> _validator;

        protected ValidatedCommandHandler(IValidator<TCommand> validator)
        {
            _validator = validator;
        }

        public async Task<Result<TResult>> Handle(TCommand command, CancellationToken cancellationToken)
        {
            // Выполняем валидацию команды
            var validationResult = await _validator.ValidateAsync(command, cancellationToken);

            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors
                    .Select(error => error.ErrorMessage)
                    .ToList();

                return Result.Fail<TResult>(string.Join("; ", errors));
            }

            // Выполняем бизнес-логику
            return await HandleValidated(command, cancellationToken);
        }

        /// <summary>
        /// Обработка команды после успешной валидации
        /// </summary>
        protected abstract Task<Result<TResult>> HandleValidated(TCommand command, CancellationToken cancellationToken);
    }
}
