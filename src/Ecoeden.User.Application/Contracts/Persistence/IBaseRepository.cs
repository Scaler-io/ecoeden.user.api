using Ecoeden.User.Application.Contracts.HealthStatus;
using Ecoeden.User.Domain.Models.Core;
using System.Linq.Expressions;

namespace Ecoeden.User.Application.Contracts.Persistence
{
    public interface IBaseRepository<T> : IHealthCheck where T: class
    {
        Task<Result<IReadOnlyList<T>>> GetAllAsync(string includeString = "");
        Task<Result<IReadOnlyList<T>>> GetAsync(Expression<Func<T, bool>> predicate);
        Task<Result<IReadOnlyList<T>>> GetAsync(Expression<Func<T, bool>> predicate,
                                        Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
                                        string includeString = null,
                                        bool disableTracking = true);
        Task<Result<IReadOnlyList<T>>> GetAsync(Expression<Func<T, bool>> predicate,
                                        Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
                                        List<Expression<Func<T, object>>> includse = null,
                                        bool disableTracking = true);
        Task<Result<long>> GetCount(Expression<Func<T, bool>> predicate);

        Task<bool> TableExists();

        Task<Result<T>> GetByIdAsync(object id);
        void Add(T entity);
        void Update(T entity);
        void Delete(T entity);
        Task<Result<int>> Completed();
    }
}
