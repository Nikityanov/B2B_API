using FluentResults;
using MediatR;
using B2B_API.Application.Interfaces;

namespace B2B_API.Application.Handlers
{
    /// <summary>
    /// Базовый класс для обработчиков запросов
    /// </summary>
    public abstract class BaseQueryHandler<TQuery, TResult> : IRequestHandler<TQuery, Result<TResult>>
        where TQuery : IQuery<TResult>
    {
        public abstract Task<Result<TResult>> Handle(TQuery query, CancellationToken cancellationToken);

        /// <summary>
        /// Валидация параметров запроса
        /// </summary>
        protected virtual Task<Result> ValidateQuery(TQuery query)
        {
            return Task.FromResult(Result.Ok());
        }
    }

    /// <summary>
    /// Специализированный базовый класс для запросов с пагинацией
    /// </summary>
    public abstract class BasePagedQueryHandler<TQuery, TItem> : IRequestHandler<TQuery, Result<(IEnumerable<TItem> Items, int TotalCount)>>
        where TQuery : IQuery<(IEnumerable<TItem> Items, int TotalCount)>
    {
        public abstract Task<Result<(IEnumerable<TItem> Items, int TotalCount)>> Handle(TQuery query, CancellationToken cancellationToken);

        /// <summary>
        /// Валидация параметров запроса
        /// </summary>
        protected virtual Task<Result> ValidateQuery(TQuery query)
        {
            return Task.FromResult(Result.Ok());
        }
    }

    /// <summary>
    /// Специализированный базовый класс для запросов с пагинацией (с конкретным типом списка)
    /// </summary>
    public abstract class BasePagedQueryHandler<TQuery, TItem, TList> : IRequestHandler<TQuery, Result<(TList Items, int TotalCount)>>
        where TQuery : IQuery<(TList Items, int TotalCount)>
        where TList : IEnumerable<TItem>
    {
        public abstract Task<Result<(TList Items, int TotalCount)>> Handle(TQuery query, CancellationToken cancellationToken);

        /// <summary>
        /// Валидация параметров запроса
        /// </summary>
        protected virtual Task<Result> ValidateQuery(TQuery query)
        {
            return Task.FromResult(Result.Ok());
        }
    }
}
