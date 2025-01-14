﻿using MongoDB.Driver;
using N_Tier.Core.Entities;
using N_Tier.Shared.N_Tier.Application.Models;
using System.Linq.Expressions;

namespace N_Tier.DataAccess.Repositories;

public interface IBaseMongoRepository
{
    IFindFluent<History<T>, History<T>> AsQuery<T>(Expression<Func<History<T>, bool>> filter);
    Task<List<History<T>>> GetAllAsync<T>();
    Task<List<History<T>>> GetAllAsync<T>(Expression<Func<History<T>, bool>> filter);
    Task<List<History<dynamic>>> GetHistoriesAsync(HistoryRequest model);
    Task<History<T>?> GetAsync<T>(string primaryRefId);
    Task<History<T>?> GetLatestAsync<T>(string primaryRefId);
    Task CreateAsync<T>(History<T> item);
    Task UpdateAsync<T>(string primaryRefId, History<T> item);
    Task RemoveAsync<T>(string primaryRefId);
    Task RemoveAllAsync<T>(Expression<Func<History<T>, bool>> filter);
    Task SaveAsync<T>(string primaryRefId, T element);
    Task CreateAllAsync<TEntity>(List<History<TEntity>> history) where TEntity : class;
}
