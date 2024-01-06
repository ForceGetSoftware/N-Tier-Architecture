using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using N_Tier.Core.Exceptions;

namespace N_Tier.DataAccess.Repositories.Impl;

public class ForcegetBaseRepository<TEntity> : IForcegetBaseRepository<
#nullable disable
    TEntity> where TEntity : class
{
    private readonly DbContext _context;
    private readonly DbSet<TEntity> _dbSet;

    protected ForcegetBaseRepository(DbContext context)
    {
        this._context = context;
        this._dbSet = context.Set<TEntity>();
    }

    public IQueryable<TEntity> AsQueryable() => this._dbSet.AsQueryable();

    public async Task<TEntity> AddAsync(TEntity entity)
    {
        var addedEntity = (await this._dbSet.AddAsync(entity)).Entity;
        var num = await this._context.SaveChangesAsync();
        var entity1 = addedEntity;
        addedEntity = default(TEntity);
        return entity1;
    }

    public async Task<TEntity> DeleteAsync(TEntity entity)
    {
        var removedEntity = this._dbSet.Remove(entity).Entity;
        var num = await this._context.SaveChangesAsync();
        var entity1 = removedEntity;
        removedEntity = default(TEntity);
        return entity1;
    }

    public async Task<List<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return await this._dbSet.Where<TEntity>(predicate).ToListAsync<TEntity>();
    }

    public async Task<TEntity> GetFirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return await this._dbSet.Where<TEntity>(predicate).FirstOrDefaultAsync<TEntity>();
    }

    public async Task<TEntity> GetFirstAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return await this._dbSet.Where<TEntity>(predicate).FirstOrDefaultAsync<TEntity>() ??
               throw new ResourceNotFoundException(typeof(TEntity));
    }

    public async Task<TEntity> UpdateAsync(TEntity entity)
    {
        this._dbSet.Update(entity);
        var num = await this._context.SaveChangesAsync();
        return entity;
    }
}
