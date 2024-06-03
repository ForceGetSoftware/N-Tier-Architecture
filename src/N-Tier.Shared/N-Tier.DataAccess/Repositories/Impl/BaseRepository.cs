using System.Linq.Expressions;
using FS.FilterExpressionCreator.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using N_Tier.Application.Models;
using N_Tier.Core.Exceptions;
using N_Tier.Shared.N_Tier.Core.Common;
using System.Linq.Dynamic.Core;

namespace N_Tier.DataAccess.Repositories.Impl;

public class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : class
{
    private readonly DbContext _context;
    private readonly DbSet<TEntity> _dbSet;
    
    protected BaseRepository(DbContext context)
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
        await _context.SaveChangesAsync();
        
        return addedEntity;
    }
    
    public Task<TEntity> DeleteAsync(TEntity entity)
    {
        throw new Exception("Update Soft Delete!");
    }
    
    public async Task<List<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return await _dbSet.Where(predicate).ToListAsync();
    }
    
    public async Task<TEntity> GetFirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate)
    {
        var entity = await _dbSet.Where(predicate).FirstOrDefaultAsync();
        
        return entity;
    }
    
    public async Task<TEntity> GetFirstAsync(Expression<Func<TEntity, bool>> predicate)
    {
        var entity = await _dbSet.Where(predicate).FirstOrDefaultAsync();
        
        if (entity == null) throw new ResourceNotFoundException(typeof(TEntity));
        
        return entity;
    }
    
    public async Task<TEntity> UpdateAsync(TEntity entity)
    {
        _dbSet.Update(entity);
        await _context.SaveChangesAsync();
        
        return entity;
    }
    
    public async Task<ApiListResult<List<TResult>>> GetFilteredListAsync<TResult>(
        Expression<Func<TEntity, TResult>> select, Expression<Func<TEntity, bool>> where,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
        Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
        int skip = 0, int take = 10)
    {
        var query = _dbSet.AsQueryable();
        List<TResult> list;
        
        if (where != null)
            query = query.Where(where);
        if (include != null)
            query = include(query);
        if (orderBy != null)
            list = await orderBy(query).Select(select).Skip(skip).Take(take).ToListAsync();
        else
            list = await query.Select(select).Skip(skip).Take(take).ToListAsync();
        
        return ApiListResult<List<TResult>>.Success(list, await query.CountAsync());
    }
    
    public Task<int> CountAsync<TEntity>(IQueryable<TEntity> queryable, EntityFilter<TEntity> where)
    {
        return queryable.CountAsync(where);
    }
    
    public async Task<int> CountAsync(GetAllRequest<TEntity> model)
    {
        return await _dbSet.CountAsync(model.Filter);
    }
    
    public async Task<List<TEntity>> GetAllGenericAsync(GetAllRequest<TEntity> model)
    {
        return await _dbSet.Where(model.Filter)
            .OrderBy(model.OrderBy ?? "Id DESC")
            .Skip(model.Skip)
            .Take(model.Take)
            .ToListAsync();
    }
    
    public async Task<List<TEntity>> GetAllGenericAsync<TEntity>(IQueryable<TEntity> queryable,
        GetAllRequest<TEntity> model)
    {
        return await queryable.Where(model.Filter)
            .OrderBy(model.OrderBy ?? "Id DESC")
            .Skip(model.Skip)
            .Take(model.Take)
            .ToListAsync();
    }
}
