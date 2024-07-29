using System.Linq.Expressions;
using Forceget.Enums;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using N_Tier.Core.Common;
using N_Tier.Core.Entities;
using N_Tier.Core.Exceptions;

namespace N_Tier.DataAccess.Repositories.Impl;

public class CustomBaseRepository<TEntity> : ICustomBaseRepository<
    TEntity> where TEntity : ForcegetBaseEntity
{
    private static readonly Func<DbContext, Guid, Task<TEntity>>
        GetByRefIdCompiled =
            EF.CompileAsyncQuery((DbContext context, Guid refId) =>
                context.Set<TEntity>()
                    .SingleOrDefault(x => x.RefId == refId && x.DataStatus == EDataStatus.Active));
    
    private readonly DbContext _context;
    private readonly DbSet<TEntity> _dbSet;
    private readonly IMongoCollection<History<TEntity>> _mongoCollection;
    
    protected CustomBaseRepository(DbContext context, IMongoClient client)
    {
        _context = context;
        _dbSet = context.Set<TEntity>();
        _mongoCollection = client.GetDatabase("Forceget").GetCollection<History<TEntity>>(typeof(TEntity).Name);
    }
    
    public IQueryable<TEntity> AsQueryable()
    {
        return _dbSet.AsQueryable();
    }
    
    public async Task<TEntity> AddAsync(TEntity entity)
    {
        entity.RefId = Guid.NewGuid();
        entity.CreatedOn = DateTime.Now;
        var addedEntity = (await _dbSet.AddAsync(entity)).Entity;
        var num = await _context.SaveChangesAsync();
        var entity1 = addedEntity;
        addedEntity = default;
        return entity1;
    }
    
    public async Task<TEntity> DeleteAsync(TEntity entity)
    {
        entity.DeletedOn = DateTime.Now;
        entity.DataStatus = EDataStatus.Deleted;
        var removedEntity = _dbSet.Remove(entity).Entity;
        var num = await _context.SaveChangesAsync();
        var entity1 = removedEntity;
        removedEntity = default;
        return entity1;
    }
    
    public async Task<List<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> predicate,
        params Expression<Func<TEntity, object>>[] includeProperties)
    {
        IQueryable<TEntity> query = _dbSet;
        if (includeProperties != null)
            query = includeProperties.Aggregate(query, (current, include) => current.Include(include));
        if (predicate != null) query = query.Where(predicate);
        return await query.ToListAsync();
    }
    
    public async Task<TEntity> GetFirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate,
        params Expression<Func<TEntity, object>>[] includeProperties)
    {
        IQueryable<TEntity> query = _dbSet;
        if (includeProperties != null)
            query = includeProperties.Aggregate(query, (current, include) => current.Include(include));
        if (predicate != null) query = query.Where(predicate);
        return await query.FirstOrDefaultAsync();
    }
    
    public async Task<TEntity> GetFirstAsync(Expression<Func<TEntity, bool>> predicate,
        params Expression<Func<TEntity, object>>[] includeProperties)
    {
        IQueryable<TEntity> query = _dbSet;
        if (includeProperties != null)
            query = includeProperties.Aggregate(query, (current, include) => current.Include(include));
        if (predicate != null) query = query.Where(predicate);
        return await query.FirstOrDefaultAsync() ??
               throw new ResourceNotFoundException(typeof(TEntity));
    }
    
    public async Task<TEntity> GetByRefIdAsync(Guid refId)
    {
        return await GetByRefIdCompiled(_context, refId);
    }
    
    public async Task<TEntity> UpdateAsync(TEntity entity)
    {
        entity.UpdatedOn = DateTime.Now;
        _dbSet.Update(entity);
        var num = await _context.SaveChangesAsync();
        return entity;
    }
    
    public async Task<List<History<TEntity>>> GetAllHistoryAsync(
        Expression<Func<History<TEntity>, bool>> filter = null)
    {
        return await _mongoCollection.Find(filter ?? (u => true)).ToListAsync();
    }
    
    public async Task<History<TEntity>> CreateHistoryAsync(History<TEntity> entity)
    {
        await _mongoCollection.InsertOneAsync(entity);
        return await _mongoCollection.Find(x => x.Id == entity.Id).FirstOrDefaultAsync();
    }
}
