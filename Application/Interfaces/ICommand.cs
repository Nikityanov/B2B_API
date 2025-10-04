using FluentResults;
using MediatR;

namespace B2B_API.Application.Interfaces
{
    /// <summary>
    /// Базовый интерфейс для команд CQRS
    /// </summary>
    public interface ICommand : IRequest<Result>
    {
    }

    /// <summary>
    /// Базовый интерфейс для команд с результатом
    /// </summary>
    public interface ICommand<TResult> : IRequest<Result<TResult>>
    {
    }
}
