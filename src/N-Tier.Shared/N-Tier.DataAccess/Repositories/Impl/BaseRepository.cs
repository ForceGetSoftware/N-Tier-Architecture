﻿using Forceget.Enums;
using FS.FilterExpressionCreator.Filters;
using Microsoft.EntityFrameworkCore;
using N_Tier.Core.Common;
using N_Tier.Core.Entities;
using N_Tier.Core.Exceptions;
using N_Tier.Shared.N_Tier.Application.Enums;
using N_Tier.Shared.N_Tier.Application.Helpers;
using N_Tier.Shared.N_Tier.Application.Models;
using N_Tier.Shared.N_Tier.Core.Common;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Storage;

namespace N_Tier.DataAccess.Repositories.Impl;

public class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : class
{
    private readonly DbContext _context;
    private readonly DbSet<TEntity> _dbSet;
    private readonly IBaseMongoRepository _forcegetMongoFuncRepository;

    protected BaseRepository(DbContext context, IBaseMongoRepository forcegetMongoFuncRepository)
    {
        _context = context;
        _dbSet = context.Set<TEntity>();
        _forcegetMongoFuncRepository = forcegetMongoFuncRepository;
    }

    public async Task<IDbContextTransaction> BeginTransactionAsync() => await _context.Database.BeginTransactionAsync();
    public async Task RollbackTransactionAsync() => await _context.Database.RollbackTransactionAsync();
    public async Task CommitTransactionAsync() => await _context.Database.CommitTransactionAsync();

    public IQueryable<TEntity> AsQueryable() => _dbSet.AsQueryable().AsNoTracking();

    public async Task<TEntity> AddAsync(TEntity entity)
    {
        var addedEntity = (await _dbSet.AddAsync(entity)).Entity;
        await _context.SaveChangesAsync();

        await _forcegetMongoFuncRepository.CreateAsync(new History<TEntity>
        {
            Action = MongoHistoryActionType.Add,
            CreationTime = DateTime.Now,
            DbObject = addedEntity,
            PrimaryRefId = ReflectionHelper.GetPropertyValue(addedEntity, "RefId")
        });

        return addedEntity;
    }

    public async Task<int> AddRangeAsync(List<TEntity> entities)
    {
        if(entities.Count == 0) return -1;
        
        await _dbSet.AddRangeAsync(entities);
        var addedEntities = await _context.SaveChangesAsync();

        await _forcegetMongoFuncRepository.CreateAllAsync<TEntity>(entities.Select(entity => new History<TEntity>
        {
            Action = MongoHistoryActionType.AddRange,
            CreationTime = DateTime.Now,
            DbObject = entity,
            PrimaryRefId = ReflectionHelper.GetPropertyValue(entity, "RefId")
        }).ToList());

        return addedEntities;
    }

    public async Task<TEntity> UpdateAsync(TEntity entity)
    {
        _dbSet.Update(entity);
        await _context.SaveChangesAsync();

        await _forcegetMongoFuncRepository.CreateAsync(new History<TEntity>
        {
            Action = MongoHistoryActionType.Update,
            CreationTime = DateTime.Now,
            DbObject = entity,
            PrimaryRefId = ReflectionHelper.GetPropertyValue(entity, "RefId")
        });

        return entity;
    }

    public async Task<int> UpdateRangeAsync(List<TEntity> entities)
    {
        if(entities.Count == 0) return -1;
        _dbSet.UpdateRange(entities);
        var result = await _context.SaveChangesAsync();

        await _forcegetMongoFuncRepository.CreateAllAsync(entities.Select(entity => new History<TEntity>
        {
            Action = MongoHistoryActionType.UpdateRange,
            CreationTime = DateTime.Now,
            DbObject = entity,
            PrimaryRefId = ReflectionHelper.GetPropertyValue(entity, "RefId")
        }).ToList());

        return result;
    }

    public async Task<TEntity> DeleteAsync(TEntity entity)
    {
        if (entity is ForcegetBaseEntity baseEntity)
            baseEntity.DataStatus = EDataStatus.Deleted;

        _dbSet.Update(entity);
        await _context.SaveChangesAsync();

        await _forcegetMongoFuncRepository.CreateAsync(new History<TEntity>
        {
            Action = MongoHistoryActionType.Delete,
            CreationTime = DateTime.Now,
            DbObject = entity,
            PrimaryRefId = ReflectionHelper.GetPropertyValue(entity, "RefId")
        });

        return entity;
    }

    public async Task<TEntity> DeleteAsync(TEntity entity, bool hardDelete)
    {
        if (hardDelete != true) return await DeleteAsync(entity);
        _dbSet.Remove(entity);

        await _forcegetMongoFuncRepository.CreateAsync(new History<TEntity>
        {
            Action = MongoHistoryActionType.HardDelete,
            CreationTime = DateTime.Now,
            DbObject = entity,
            PrimaryRefId = ReflectionHelper.GetPropertyValue(entity, "RefId")
        });

        await _context.SaveChangesAsync();

        return entity;
    }

    public async Task<int> DeleteRangeAsync(List<TEntity> entities)
    {
        if(entities.Count == 0) return -1;
        await _forcegetMongoFuncRepository.CreateAllAsync(entities.Select(entity => new History<TEntity>
        {
            Action = MongoHistoryActionType.DeleteRange,
            CreationTime = DateTime.Now,
            DbObject = entity,
            PrimaryRefId = ReflectionHelper.GetPropertyValue(entity, "RefId")
        }).ToList());

        _dbSet.UpdateRange(entities.Select(entity =>
        {
            if (entity is ForcegetBaseEntity baseEntity)
                baseEntity.DataStatus = EDataStatus.Deleted;
            return entity;
        }));

        return await _context.SaveChangesAsync();
    }

    public async Task<int> DeleteRangeAsync(List<TEntity> entities, bool hardDelete)
    {
        if(entities.Count == 0) return -1;
        if (!hardDelete) return await DeleteRangeAsync(entities);
        _dbSet.RemoveRange(entities.Select(s =>
        {
            if (s is ForcegetBaseEntity baseEntity)
                baseEntity.DataStatus = EDataStatus.Deleted;
            return s;
        }));

        await _forcegetMongoFuncRepository.CreateAllAsync(entities.Select(entity => new History<TEntity>
        {
            Action = MongoHistoryActionType.HardDeleteRange,
            CreationTime = DateTime.Now,
            DbObject = entity,
            PrimaryRefId = ReflectionHelper.GetPropertyValue(entity, "RefId")
        }).ToList());

        return await _context.SaveChangesAsync();
    }

    public async Task<List<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> predicate) =>
        await _dbSet.Where(predicate).ToListAsync();

    public async Task<TEntity> GetFirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate) =>
        await _dbSet.Where(predicate).FirstOrDefaultAsync();

    public async Task<TEntity> GetFirstAsync(Expression<Func<TEntity, bool>> predicate)
    {
        var entity = await _dbSet.Where(predicate).FirstOrDefaultAsync();
        return entity ?? throw new ResourceNotFoundException(typeof(TEntity));
    }

    public Task<int> CountAsync<TEntity>(IQueryable<TEntity> queryable, EntityFilter<TEntity> where) =>
        queryable.CountAsync(where);

    public async Task<int> CountAsync(GetAllRequest<TEntity> model) => await _dbSet.CountAsync(model.Filter);

    public async Task<List<TEntity>> GetAllGenericAsync(GetAllRequest<TEntity> model) => await _dbSet
        .Where(model.Filter)
        .OrderBy(model.OrderBy ?? "Id DESC")
        .Skip(model.Skip)
        .Take(model.Take)
        .ToListAsync();

    public async Task<List<TEntity>> GetAllGenericAsync<TEntity>(IQueryable<TEntity> queryable,
        GetAllRequest<TEntity> model) =>
        await queryable.Where(model.Filter)
            .OrderBy(model.OrderBy ?? "Id DESC")
            .Skip(model.Skip)
            .Take(model.Take)
            .ToListAsync();

    public async Task<List<History<dynamic>>> GetAllHistory(HistoryRequest model) =>
        await _forcegetMongoFuncRepository.GetHistoriesAsync(model);
}
