using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using N_Tier.Core.Exceptions;

namespace N_Tier.DataAccess.Repositories.Impl;

public class ForcegetBaseRepository<TEntity> : IForcegetBaseRepository<TEntity> where TEntity : class
{
    private readonly DbContext _context;
    private readonly DbSet<TEntity> _dbSet;
    
    protected ForcegetBaseRepository(DbContext context)
    {
        _context = context;
        _dbSet = context.Set<TEntity>();
    }
    
    public IQueryable<TEntity> AsQueryable()
    {
        return _dbSet.AsQueryable();
    }
    
    public async Task<TEntity> AddAsync(TEntity entity)
    {
        var addedEntity = (await _dbSet.AddAsync(entity)).Entity;
        var num = await _context.SaveChangesAsync();
        var entity1 = addedEntity;
        addedEntity = default;
        return entity1;
    }
    
    public async Task<TEntity> DeleteAsync(TEntity entity)
    {
        var removedEntity = _dbSet.Remove(entity).Entity;
        var num = await _context.SaveChangesAsync();
        var entity1 = removedEntity;
        removedEntity = default;
        return entity1;
    }
    
    public async Task<List<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return await _dbSet.Where(predicate).ToListAsync();
    }
    
    public async Task<TEntity> GetFirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return await _dbSet.Where(predicate).FirstOrDefaultAsync();
    }
    
    public async Task<TEntity> GetFirstAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return await _dbSet.Where(predicate).FirstOrDefaultAsync() ??
               throw new ResourceNotFoundException(typeof(TEntity));
    }
    
    public async Task<TEntity> UpdateAsync(TEntity entity)
    {
        _dbSet.Update(entity);
        var num = await _context.SaveChangesAsync();
        return entity;
    }
}
