#nullable enable
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Template.Domain.Common;
using Template.Domain.Interfaces;
using Template.Infrastructure.Persistance;

namespace Template.Infrastructure.Repositories;

public class BaseRepository<TEntity> : IBaseRepository<TEntity>
    where TEntity : BaseEntity, new()
{
    private readonly AppCommandDbContext _commandContext;
    private readonly AppQueryDbContext _queryContext;

    public BaseRepository(AppCommandDbContext commandContext, AppQueryDbContext queryContext)
    {
        _commandContext = commandContext;
        _queryContext = queryContext;
    }

    public virtual async Task<IEnumerable<TEntity>> GetAllAsync()
    {
        return await _queryContext.Set<TEntity>()
                    .AsNoTracking()
                    .ToListAsync()
                    .ConfigureAwait(false);
    }

    public virtual async Task<TEntity?> GetByIdAsync(int id)
    {
        return await _queryContext.Set<TEntity>()
                    .AsNoTracking()
                    .FirstOrDefaultAsync(o => o.Id == id)
                    .ConfigureAwait(false);
    }

    public virtual async Task AddAsync(TEntity entity)
    {
        await _commandContext.Set<TEntity>().AddAsync(entity).ConfigureAwait(false);
    }

    public virtual void Update(TEntity entity)
    {
        _commandContext.Entry(entity).State = EntityState.Modified;
    }

    public virtual void Delete(TEntity entity)
    {
        _commandContext.Set<TEntity>().Remove(entity);
    }

    public virtual async Task<bool> ExistsAsync(int id)
    {
        return await _queryContext.Set<TEntity>()
                    .AsNoTracking()
                    .AnyAsync(x => x.Id == id)
                    .ConfigureAwait(false);
    }

    public virtual async Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return await _queryContext.Set<TEntity>()
                    .AsNoTracking()
                    .Where(predicate)
                    .ToListAsync()
                    .ConfigureAwait(false);
    }

    public virtual async Task<int> CountAsync()
    {
        return await _queryContext.Set<TEntity>()
                    .AsNoTracking()
                    .CountAsync()
                    .ConfigureAwait(false);
    }

    public virtual async Task<List<TEntity>> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        int skip = (pageNumber - 1) * pageSize;
        return await _queryContext.Set<TEntity>()
                    .AsNoTracking()
                    .Skip(skip)
                    .Take(pageSize)
                    .ToListAsync(cancellationToken)
                    .ConfigureAwait(false);
    }
}
