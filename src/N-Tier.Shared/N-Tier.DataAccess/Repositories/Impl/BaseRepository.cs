﻿using Forceget.Enums;
using FS.FilterExpressionCreator.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using N_Tier.Application.Models;
using N_Tier.Core.Common;
using N_Tier.Core.Entities;
using N_Tier.Core.Exceptions;
using N_Tier.Shared.N_Tier.Core.Common;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;

namespace N_Tier.DataAccess.Repositories.Impl;

public class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : class
{
    private readonly DbContext _context;
    private readonly DbSet<TEntity> _dbSet;
    private readonly IForcegetMongoFuncRepository _forcegetMongoFuncRepository;

    protected BaseRepository(DbContext context)
    {
        _context = context;
        _dbSet = context.Set<TEntity>();
    }

    protected BaseRepository(DbContext context, IForcegetMongoFuncRepository forcegetMongoFuncRepository)
    {
        _context = context;
        _dbSet = context.Set<TEntity>();
        _forcegetMongoFuncRepository = forcegetMongoFuncRepository;
    }

    public IQueryable<TEntity> AsQueryable() => _dbSet.AsQueryable();

    public async Task<TEntity> AddAsync(TEntity entity)
    {
        var addedEntity = (await _dbSet.AddAsync(entity)).Entity;
        await _context.SaveChangesAsync();
        return addedEntity;
    }

    public async Task<int> AddRangeAsync(IEnumerable<TEntity> entities)
    {
        await _dbSet.AddRangeAsync(entities);
        return await _context.SaveChangesAsync();
    }

    public async Task<TEntity> UpdateAsync(TEntity entity)
    {
        if (_forcegetMongoFuncRepository != null)
        {
            var refId = entity.GetType().GetProperties().First(x => x.Name == "RefId")?.ToString() ?? Guid.NewGuid().ToString();
            await _forcegetMongoFuncRepository.CreateAsync(new History<TEntity>
            {
                Action = null,
                CreationTime = DateTime.Now,
                DbObject = entity,
                PrimaryRefId = refId

            });
        }

        _dbSet.Update(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<int> UpdateRangeAsync(IEnumerable<TEntity> entities)
    {
        if (_forcegetMongoFuncRepository != null)
            foreach (var entity in entities)
            {
                var refId = entity.GetType().GetProperties().First(x => x.Name == "RefId")?.ToString() ?? Guid.NewGuid().ToString();
                await _forcegetMongoFuncRepository.CreateAsync(new History<TEntity>
                {
                    Action = null,
                    CreationTime = DateTime.Now,
                    DbObject = entity,
                    PrimaryRefId = refId

                });
            }

        _dbSet.UpdateRange(entities);
        return await _context.SaveChangesAsync();
    }

    public async Task<TEntity> DeleteAsync(TEntity entity)
    {
        if (entity is ForcegetBaseEntity)
            (entity as ForcegetBaseEntity).DataStatus = EDataStatus.Deleted;

        _dbSet.Update(entity);

        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<TEntity> DeleteAsync(TEntity entity, bool hardDelete)
    {
        if (hardDelete == true)
        {
            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        return await DeleteAsync(entity);
    }

    public async Task<int> DeleteRangeAsync(IEnumerable<TEntity> entities)
    {
        foreach (var entity in entities)
        {
            if (entity is ForcegetBaseEntity)
                (entity as ForcegetBaseEntity).DataStatus = EDataStatus.Deleted;
        }

        _dbSet.UpdateRange(entities);

        return await _context.SaveChangesAsync();
    }

    public async Task<int> DeleteRangeAsync(IEnumerable<TEntity> entities, bool hardDelete)
    {
        if (hardDelete == true)
        {
            _dbSet.RemoveRange(entities);
            return await _context.SaveChangesAsync();
        }

        return await DeleteRangeAsync(entities);
    }

    public async Task<List<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> predicate) => await _dbSet.Where(predicate).ToListAsync();

    public async Task<TEntity> GetFirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate) => await _dbSet.Where(predicate).FirstOrDefaultAsync();

    public async Task<TEntity> GetFirstAsync(Expression<Func<TEntity, bool>> predicate)
    {
        var entity = await _dbSet.Where(predicate).FirstOrDefaultAsync();
        return entity ?? throw new ResourceNotFoundException(typeof(TEntity));
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

    public Task<int> CountAsync<TEntity>(IQueryable<TEntity> queryable, EntityFilter<TEntity> where) => queryable.CountAsync(where);

    public async Task<int> CountAsync(GetAllRequest<TEntity> model) => await _dbSet.CountAsync(model.Filter);

    public async Task<List<TEntity>> GetAllGenericAsync(GetAllRequest<TEntity> model) => await _dbSet.Where(model.Filter)
            .OrderBy(model.OrderBy ?? "Id DESC")
            .Skip(model.Skip)
            .Take(model.Take)
            .ToListAsync();

    public async Task<List<TEntity>> GetAllGenericAsync<TEntity>(IQueryable<TEntity> queryable, GetAllRequest<TEntity> model) =>
        await queryable.Where(model.Filter)
            .OrderBy(model.OrderBy ?? "Id DESC")
            .Skip(model.Skip)
            .Take(model.Take)
            .ToListAsync();

    public async Task<List<History<TEntity>>> GetAllHistory(Expression<Func<History<TEntity>, bool>> predicate)
    {
        if (_forcegetMongoFuncRepository != null)
            return await _forcegetMongoFuncRepository.GetAllAsync(predicate);
        return [];
    }
}
