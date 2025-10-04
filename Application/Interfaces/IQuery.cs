using FluentResults;
using MediatR;

namespace B2B_API.Application.Interfaces
{
    /// <summary>
    /// Базовый интерфейс для запросов CQRS
    /// </summary>
    public interface IQuery<TResult> : IRequest<Result<TResult>>
    {
    }
}
