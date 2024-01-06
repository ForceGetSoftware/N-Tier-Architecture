using Microsoft.EntityFrameworkCore;
using N_Tier.Core.Common;
using N_Tier.Core.Exceptions;
using System.Linq.Expressions;
using Forceget.Enums;
using MongoDB.Driver;
using N_Tier.Core.Entities;

#nullable enable
namespace N_Tier.DataAccess.Repositories.Impl
{
    public class CustomBaseRepository<TEntity> : ICustomBaseRepository<
#nullable disable
        TEntity> where TEntity : CustomBaseEntity
    {
        private readonly DbContext _context;
        private readonly DbSet<TEntity> _dbSet;
        private readonly IMongoCollection<History<TEntity>> _mongoCollection;

        protected CustomBaseRepository(DbContext context, IMongoClient client)
        {
            _context = context;
            _dbSet = context.Set<TEntity>();
            _mongoCollection = client.GetDatabase("Forceget").GetCollection<History<TEntity>>(nameof(TEntity));
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

        private static readonly Func<DbContext, Guid, Task<TEntity>>
            GetByRefIdCompiled =
                EF.CompileAsyncQuery((DbContext context, Guid refId) =>
                    context.Set<TEntity>()
                        .SingleOrDefault(x => x.RefId == refId && x.DataStatus == EuDataStatus.Active));

        public async Task<TEntity> GetByRefIdAsync(Guid refId)
        {
            return await GetByRefIdCompiled(_context, refId);
        }

        public async Task<TEntity> UpdateAsync(TEntity entity)
        {
            this._dbSet.Update(entity);
            var num = await this._context.SaveChangesAsync();
            return entity;
        }

        public async Task<IEnumerable<History<TEntity>>> GetAllHistoryAsync(
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
}
