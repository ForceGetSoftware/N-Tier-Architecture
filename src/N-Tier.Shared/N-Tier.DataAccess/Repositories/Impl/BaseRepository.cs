﻿using Forceget.Enums;
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
using Plainquire.Filter;

namespace N_Tier.DataAccess.Repositories.Impl;

public class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : class
{
    private readonly DbContext _context;
    private readonly DbSet<TEntity> _dbSet;
    private readonly IBaseMongoRepository _baseMongoRepository;

    protected BaseRepository(DbContext context, IBaseMongoRepository baseMongoRepository)
    {
        _context = context;
        _dbSet = context.Set<TEntity>();
        _baseMongoRepository = baseMongoRepository;
    }

    public async Task<IDbContextTransaction> BeginTransactionAsync() => await _context.Database.BeginTransactionAsync();
    public async Task RollbackTransactionAsync() => await _context.Database.RollbackTransactionAsync();
    public async Task CommitTransactionAsync() => await _context.Database.CommitTransactionAsync();

    public IQueryable<TEntity> AsQueryable() => _dbSet.AsQueryable().AsNoTracking();

    public IQueryable<T> AsQueryableTable<T>() where T : class
    {
        return _context.Set<T>().AsQueryable().AsNoTracking();
    }

    public async Task<TEntity> AddAsync(TEntity entity)
    {
        var addedEntity = (await _dbSet.AddAsync(entity)).Entity;
        await _context.SaveChangesAsync();

        await _baseMongoRepository.CreateAsync(new History<TEntity>
        {
            Action = MongoHistoryActionType.Add,
            CreationTime = DateTime.Now,
            DbObject = addedEntity,
            PrimaryRefId = ReflectionHelper.GetPropertyValue(addedEntity, "refid")
        });

        return addedEntity;
    }

    public async Task<int> AddRangeAsync(List<TEntity> entities)
    {
        if (entities.Count == 0) return -1;

        await _dbSet.AddRangeAsync(entities);
        var addedEntities = await _context.SaveChangesAsync();

        await _baseMongoRepository.CreateAllAsync<TEntity>(entities.Select(entity => new History<TEntity>
        {
            Action = MongoHistoryActionType.AddRange,
            CreationTime = DateTime.Now,
            DbObject = entity,
            PrimaryRefId = ReflectionHelper.GetPropertyValue(entity, "refid")
        }).ToList());

        return addedEntities;
    }

    public async Task<TEntity> UpdateAsync(TEntity entity)
    {
        _dbSet.Update(entity);
        await _context.SaveChangesAsync();

        await _baseMongoRepository.CreateAsync(new History<TEntity>
        {
            Action = MongoHistoryActionType.Update,
            CreationTime = DateTime.Now,
            DbObject = entity,
            PrimaryRefId = ReflectionHelper.GetPropertyValue(entity, "refid")
        });

        return entity;
    }

    public async Task<int> UpdateRangeAsync(List<TEntity> entities)
    {
        if (entities.Count == 0) return -1;
        _dbSet.UpdateRange(entities);
        var result = await _context.SaveChangesAsync();

        await _baseMongoRepository.CreateAllAsync(entities.Select(entity => new History<TEntity>
        {
            Action = MongoHistoryActionType.UpdateRange,
            CreationTime = DateTime.Now,
            DbObject = entity,
            PrimaryRefId = ReflectionHelper.GetPropertyValue(entity, "refid")
        }).ToList());

        return result;
    }

    public async Task<TEntity> DeleteAsync(TEntity entity)
    {
        if (entity is ForcegetBaseEntity baseEntity)
            baseEntity.datastatus = EDataStatus.Deleted;

        _dbSet.Update(entity);
        await _context.SaveChangesAsync();

        await _baseMongoRepository.CreateAsync(new History<TEntity>
        {
            Action = MongoHistoryActionType.Delete,
            CreationTime = DateTime.Now,
            DbObject = entity,
            PrimaryRefId = ReflectionHelper.GetPropertyValue(entity, "refid")
        });

        return entity;
    }

    public async Task<TEntity> DeleteAsync(TEntity entity, bool hardDelete)
    {
        if (hardDelete != true) return await DeleteAsync(entity);
        _dbSet.Remove(entity);

        await _baseMongoRepository.CreateAsync(new History<TEntity>
        {
            Action = MongoHistoryActionType.HardDelete,
            CreationTime = DateTime.Now,
            DbObject = entity,
            PrimaryRefId = ReflectionHelper.GetPropertyValue(entity, "refid")
        });

        await _context.SaveChangesAsync();

        return entity;
    }

    public async Task<int> DeleteRangeAsync(List<TEntity> entities)
    {
        if (entities.Count == 0) return -1;
        await _baseMongoRepository.CreateAllAsync(entities.Select(entity => new History<TEntity>
        {
            Action = MongoHistoryActionType.DeleteRange,
            CreationTime = DateTime.Now,
            DbObject = entity,
            PrimaryRefId = ReflectionHelper.GetPropertyValue(entity, "refid")
        }).ToList());

        _dbSet.UpdateRange(entities.Select(entity =>
        {
            if (entity is ForcegetBaseEntity baseEntity)
                baseEntity.datastatus = EDataStatus.Deleted;
            return entity;
        }));

        return await _context.SaveChangesAsync();
    }

    public async Task<int> DeleteRangeAsync(List<TEntity> entities, bool hardDelete)
    {
        if (entities.Count == 0) return -1;
        if (!hardDelete) return await DeleteRangeAsync(entities);
        _dbSet.RemoveRange(entities.Select(s =>
        {
            if (s is ForcegetBaseEntity baseEntity)
                baseEntity.datastatus = EDataStatus.Deleted;
            return s;
        }));

        await _baseMongoRepository.CreateAllAsync(entities.Select(entity => new History<TEntity>
        {
            Action = MongoHistoryActionType.HardDeleteRange,
            CreationTime = DateTime.Now,
            DbObject = entity,
            PrimaryRefId = ReflectionHelper.GetPropertyValue(entity, "refid")
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

    public async Task<int> CountAsync<TEntity>(IQueryable<TEntity> queryable, EntityFilter<TEntity> where)
    {
        return await queryable.CountAsync(where);
    }

    public async Task<int> CountAsync(GetAllRequest<TEntity> model)
    {
        return await _dbSet.CountAsync(model.Filter);
    }

    public async Task<List<TEntity>> GetAllGenericAsync(GetAllRequest<TEntity> model)
    {
        var queryable = _dbSet.Where(model.Filter);
        if (string.IsNullOrEmpty(model.OrderBy))
        {
            model.OrderBy = typeof(TEntity).GetProperty("createdon") != null ? "createdon DESC" : "id DESC";
        }

        if (!string.IsNullOrEmpty(model.OrderBy))
            queryable = queryable.OrderBy(model.OrderBy);

        return await queryable
            .Skip(model.Skip)
            .Take(model.Take)
            .ToListAsync();
    }

    public async Task<List<TEntity>> GetAllGenericAsync<TEntity>(IQueryable<TEntity> queryable,
        GetAllRequest<TEntity> model)
    {
        queryable = queryable.Where(model.Filter);
        if (string.IsNullOrEmpty(model.OrderBy))
        {
            model.OrderBy = typeof(TEntity).GetProperty("createdon") != null ? "createdon DESC" : "id DESC";
        }

        if (!string.IsNullOrEmpty(model.OrderBy))
            queryable = queryable.OrderBy(model.OrderBy);

        return await queryable
            .Skip(model.Skip)
            .Take(model.Take)
            .ToListAsync();
    }

    public async Task<List<History<dynamic>>> GetAllHistory(HistoryRequest model) =>
        await _baseMongoRepository.GetHistoriesAsync(model);
}
