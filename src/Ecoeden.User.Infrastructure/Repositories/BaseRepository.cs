using App.Metrics.Health;
using Ecoeden.User.Application.Contracts.Persistence;
using Ecoeden.User.Domain.Models.Core;
using Ecoeden.User.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
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

        public async Task<Result<IReadOnlyList<T>>> GetAllAsync(string includeString = "")
        {
            IQueryable<T> query = _context.Set<T>();
            if (!string.IsNullOrEmpty(includeString)) query = query.Include(includeString);
            return Result<IReadOnlyList<T>>.Success(await query.ToListAsync());
        }

        public async Task<Result<IReadOnlyList<T>>> GetAsync(Expression<Func<T, bool>> predicate)
        {
            var result = await _context.Set<T>().Where(predicate).ToListAsync();
            return Result<IReadOnlyList<T>>.Success(result);
        }

        public async Task<Result<IReadOnlyList<T>>> GetAsync(Expression<Func<T, bool>> predicate, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, string includeString = null, bool disableTracking = true)
        {
            IQueryable<T> query = _context.Set<T>();
            if (disableTracking) query.AsNoTracking();
            if (!string.IsNullOrEmpty(includeString)) query = query.Include(includeString);
            if (predicate != null) query = query.Where(predicate);
            return Result<IReadOnlyList<T>>.Success(await query.ToListAsync());
        }

        public async Task<Result<IReadOnlyList<T>>> GetAsync(Expression<Func<T, bool>> predicate, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, List<Expression<Func<T, object>>> includse = null, bool disableTracking = true)
        {
            IQueryable<T> query = _context.Set<T>();
            if (disableTracking) query.AsNoTracking();
            if (includse != null) query = includse.Aggregate(query, (current, include) => current.Include(include));
            if (predicate != null) query = query.Where(predicate);
            if (orderBy != null) return Result<IReadOnlyList<T>>.Success(await orderBy(query).ToListAsync());
            return Result<IReadOnlyList<T>>.Success(await query.ToListAsync());
        }

        public async Task<Result<T>> GetByIdAsync(object id)
        {
            var result = await _context.Set<T>().FindAsync(id);
            return Result<T>.Success(result);
        }

        public async Task<Result<long>> GetCount(Expression<Func<T, bool>> predicate)
        {
            var result = await _context.Set<T>().Where(predicate).LongCountAsync();
            return Result<long>.Success(result);
        }

        public async Task<bool> TableExists()
        {
            try
            {
               await _context.Set<T>().CountAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public async ValueTask<HealthCheckResult> CheckIsHealthyAsync()
        {
            var result = await TableExists();
            return result ? HealthCheckResult.Healthy() : HealthCheckResult.Unhealthy();
        }

        public void Add(T entity)
        {
            _context.Set<T>().Add(entity);
        }

        public void Update(T entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
        }

        public void Delete(T entity)
        {
            _context.Set<T>().Remove(entity);
        }
        public async Task<Result<int>> Completed()
        {
            var result = await _context.SaveChangesAsync();
            return Result<int>.Success(result);
        }
    }
}
