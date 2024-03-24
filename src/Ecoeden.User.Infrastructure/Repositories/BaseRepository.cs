using App.Metrics.Health;
using Ecoeden.User.Application.Contracts.Persistence;
using Ecoeden.User.Domain.Models.Core;
using Ecoeden.User.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Linq.Expressions;

namespace Ecoeden.User.Infrastructure.Repositories
{
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        private readonly UserDbContext _context;

        public BaseRepository(UserDbContext context)
        {
            _context = context;
        }

        public Task<Result<T>> AddAsync(T entity)
        {
            throw new NotImplementedException();
        }

        public Task<Result<int>> Completed()
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(T entity)
        {
            throw new NotImplementedException();
        }

        public Task<Result<IReadOnlyList<T>>> GetAllAsync(RequestQuery querySpec)
        {
            throw new NotImplementedException();
        }

        public Task<Result<IReadOnlyList<T>>> GetAsync(Expression<Func<T, bool>> predicate, RequestQuery querySpec)
        {
            throw new NotImplementedException();
        }

        public Task<Result<IReadOnlyList<T>>> GetAsync(RequestQuery querySpec, Expression<Func<T, bool>> predicate, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, string includeString = null, bool disableTracking = true)
        {
            throw new NotImplementedException();
        }

        public Task<Result<IReadOnlyList<T>>> GetAsync(RequestQuery querySpec, Expression<Func<T, bool>> predicate, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, List<Expression<Func<T, object>>> includse = null, bool disableTracking = true)
        {
            throw new NotImplementedException();
        }

        public Task<Result<T>> GetByIdAsync(object id)
        {
            throw new NotImplementedException();
        }

        public Task<Result<long>> GetCount(Expression<Func<T, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> TableExists()
        {
            try
            {
               await _context.Set<T>().CountAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public async ValueTask<HealthCheckResult> CheckIsHealthyAsync()
        {
            var result = await TableExists();
            return result ? HealthCheckResult.Healthy() : HealthCheckResult.Unhealthy();
        }

        public Task UpdateAsync(T entity)
        {
            throw new NotImplementedException();
        }
    }
}
