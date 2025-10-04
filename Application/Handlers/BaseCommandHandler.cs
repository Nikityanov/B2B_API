using FluentResults;
using MediatR;
using B2B_API.Application.Interfaces;

namespace B2B_API.Application.Handlers
{
    /// <summary>
    /// Базовый класс для обработчиков команд
    /// </summary>
    public abstract class BaseCommandHandler<TCommand> : IRequestHandler<TCommand, Result>
        where TCommand : ICommand
    {
        public abstract Task<Result> Handle(TCommand command, CancellationToken cancellationToken);

        /// <summary>
        /// Валидация бизнес-правил перед выполнением команды
        /// </summary>
        protected virtual Task<Result> ValidateCommand(TCommand command)
        {
            return Task.FromResult(Result.Ok());
        }
    }

    /// <summary>
    /// Базовый класс для обработчиков команд с результатом
    /// </summary>
    public abstract class BaseCommandHandler<TCommand, TResult> : IRequestHandler<TCommand, Result<TResult>>
        where TCommand : ICommand<TResult>
    {
        public abstract Task<Result<TResult>> Handle(TCommand command, CancellationToken cancellationToken);

        /// <summary>
        /// Валидация бизнес-правил перед выполнением команды
        /// </summary>
        protected virtual Task<Result> ValidateCommand(TCommand command)
        {
            return Task.FromResult(Result.Ok());
        }
    }
}
