using Forceget.Enums;
using FS.FilterExpressionCreator.Filters;
using Microsoft.EntityFrameworkCore;
using N_Tier.Core.Common;
using N_Tier.Core.Exceptions;
using N_Tier.Shared.N_Tier.Core.Common;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;

namespace N_Tier.DataAccess.Repositories.Impl;

public class BaseGenericRepository<TDbContext>(TDbContext context) : IBaseGenericRepository where TDbContext : DbContext
{
    public IQueryable<TEntity> AsQueryable<TEntity>() where TEntity : class
    {
        var dbSet =
            context.Set<TEntity>();
        return dbSet.AsQueryable();
    }

    public async Task<TEntity> AddAsync<TEntity>(TEntity entity) where TEntity : class
    {
        var dbSet =
            context.Set<TEntity>();
        var addedEntity = (await dbSet.AddAsync(entity)).Entity;
        await
            context.SaveChangesAsync();
        return addedEntity;
    }

    public async Task<int> AddRangeAsync<TEntity>(IEnumerable<TEntity> entities) where TEntity : class
    {
        var dbSet =
            context.Set<TEntity>();
        await dbSet.AddRangeAsync(entities);
        return await
            context.SaveChangesAsync();
    }

    public async Task<TEntity> UpdateAsync<TEntity>(TEntity entity) where TEntity : class
    {
        var dbSet =
            context.Set<TEntity>();
        dbSet.Update(entity);
        await
            context.SaveChangesAsync();
        return entity;
    }

    public async Task<int> UpdateRangeAsync<TEntity>(IEnumerable<TEntity> entities) where TEntity : class
    {
        var dbSet =
            context.Set<TEntity>();
        dbSet.UpdateRange(entities);
        return await
            context.SaveChangesAsync();
    }

    public async Task<TEntity> DeleteAsync<TEntity>(TEntity entity) where TEntity : class
    {
        if (entity is ForcegetBaseEntity)
            (entity as ForcegetBaseEntity).DataStatus = EDataStatus.Deleted;

        var dbSet =
            context.Set<TEntity>();
        dbSet.Update(entity);

        await
            context.SaveChangesAsync();
        return entity;
    }

    public async Task<TEntity> DeleteAsync<TEntity>(TEntity entity, bool hardDelete) where TEntity : class
    {
        if (hardDelete == true)
        {
            var dbSet =
                context.Set<TEntity>();
            dbSet.Remove(entity);
            await
                context.SaveChangesAsync();
            return entity;
        }

        return await DeleteAsync(entity);
    }

    public async Task<int> DeleteRangeAsync<TEntity>(IEnumerable<TEntity> entities) where TEntity : class
    {
        foreach (var entity in entities)
        {
            if (entity is ForcegetBaseEntity)
                (entity as ForcegetBaseEntity).DataStatus = EDataStatus.Deleted;
        }

        var dbSet =
            context.Set<TEntity>();
        dbSet.UpdateRange(entities);

        return await
            context.SaveChangesAsync();
    }

    public async Task<int> DeleteRangeAsync<TEntity>(IEnumerable<TEntity> entities, bool hardDelete)
        where TEntity : class
    {
        if (hardDelete == true)
        {
            var dbSet =
                context.Set<TEntity>();
            dbSet.RemoveRange(entities);
            return await
                context.SaveChangesAsync();
        }

        return await DeleteRangeAsync(entities);
    }

    public async Task<List<TEntity>> GetAllAsync<TEntity>(Expression<Func<TEntity, bool>> predicate)
        where TEntity : class
    {
        var dbSet =
            context.Set<TEntity>();
        return await dbSet.Where(predicate).ToListAsync();
    }

    public async Task<TEntity> GetFirstOrDefaultAsync<TEntity>(Expression<Func<TEntity, bool>> predicate)
        where TEntity : class
    {
        var dbSet =
            context.Set<TEntity>();
        return await dbSet.Where(predicate).FirstOrDefaultAsync();
    }

    public async Task<TEntity> GetFirstAsync<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class
    {
        var dbSet =
            context.Set<TEntity>();
        var entity = await dbSet.Where(predicate).FirstOrDefaultAsync();
        return entity ?? throw new ResourceNotFoundException(typeof(TEntity));
    }

    public Task<int> CountAsync<TEntity>(IQueryable<TEntity> queryable, EntityFilter<TEntity> where)
        where TEntity : class
    {
        return queryable.CountAsync(where);
    }

    public async Task<int> CountAsync<TEntity>(GetAllRequest<TEntity> model) where TEntity : class
    {
        var dbSet =
            context.Set<TEntity>();
        return await dbSet.CountAsync(model.Filter);
    }

    public async Task<List<TEntity>> GetAllGenericAsync<TEntity>(GetAllRequest<TEntity> model) where TEntity : class
    {
        var dbSet =
            context.Set<TEntity>();
        return await dbSet.Where(model.Filter)
            .OrderBy(model.OrderBy ?? "Id DESC")
            .Skip(model.Skip)
            .Take(model.Take)
            .ToListAsync();
    }

    public async Task<List<TEntity>> GetAllGenericAsync<TEntity>(IQueryable<TEntity> queryable,
        GetAllRequest<TEntity> model) where TEntity : class
    {
        return await queryable.Where(model.Filter)
            .OrderBy(model.OrderBy ?? "Id DESC")
            .Skip(model.Skip)
            .Take(model.Take)
            .ToListAsync();
    }
}
