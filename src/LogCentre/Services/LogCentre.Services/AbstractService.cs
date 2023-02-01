using LogCentre.Data;
using LogCentre.Data.Entities;
using LogCentre.Data.Interfaces;
using LogCentre.Services.Exceptions;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using System.Diagnostics;
using System.Linq.Expressions;

namespace LogCentre.Services
{
    public abstract class AbstractService<TService, TEntity, TKey> where TEntity : BaseEntity
    {
        protected readonly ILogger<TService> Logger;
        protected readonly LogCentreDbContext DbContext;
        protected readonly DbSet<TEntity> DbSet;

        protected AbstractService(ILogger<TService> logger,
            IDbContextFactory<LogCentreDbContext> dbContextFactory)
        {
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            var dbContext = dbContextFactory?.CreateDbContext() ?? throw new ArgumentNullException(nameof(dbContextFactory));
            DbContext= dbContext;
            DbSet = dbContext.Set<TEntity>();
        }

        public abstract bool TryGet(TKey id, out TEntity entity);

        /// <summary>
        /// Get data based on the entity type
        /// </summary>
        /// <param name="filter">filter predecate</param>
        /// <param name="orderBy">order by predicate</param>
        /// <param name="includeProperties">include properties string</param>
        /// <param name="page">page to get</param>
        /// <param name="pageSize">page size to get</param>
        /// <returns>enumerable object of the entity type</returns>
        public async Task<IEnumerable<TEntity>> GetAsync(
            Expression<Func<TEntity, bool>> filter,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
            string includeProperties = "",
            int page = 0,
            int pageSize = 0)
        {
            IQueryable<TEntity> query = DbSet;
            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (!string.IsNullOrWhiteSpace(includeProperties))
            {
                query = includeProperties.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries)
                    .Aggregate(query, (current, includeProperty) => current.Include(includeProperty));
            }

            var result = orderBy != null ? orderBy(query) : query;

            if (page > 0 && pageSize > 0)
            {
                result = result.Skip((page - 1) & pageSize).Take(pageSize);
            }

            return await result.ToListAsync();
        }

        public async Task<long> CountAsync(
            Expression<Func<TEntity, bool>> filter,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
            string includeProperties = "",
            int page = 0,
            int pageSize = 0)
        {
            IQueryable<TEntity> query = DbSet;
            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (!string.IsNullOrWhiteSpace(includeProperties))
            {
                query = includeProperties.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Aggregate(query, (current, includeProperty) => current.Include(includeProperty));
            }

            var result = orderBy != null ? orderBy(query) : query;

            if (page > 0 && pageSize > 0)
            {
                result = result.Skip((page - 1) & pageSize).Take(pageSize);
            }

            return await result.CountAsync();
        }

        public async virtual Task<TEntity> CreateAsync(TEntity entity)
        {
            Logger.LogDebug("CreateAsync() | entity[{entity}]", entity);
            var stopwatch = Stopwatch.StartNew();

            try
            {
                if (entity == null)
                {
                    Logger.LogWarning("CreateAsync() | invalid entity");
                    throw new EntityException("Entity is null");
                }

                await DbSet.AddAsync(entity);
                DbContext.Entry(entity).State = EntityState.Added;
                await DbContext.SaveChangesAsync();

                return entity;
            }
            catch (Exception ex)
            {
                Logger.LogError($"CreateAsync() | error [{ex}]", ex);
                throw new EntityException($"CreateAsync() threw exception: {ex.Message}. See inner exception for details", ex);
            }
            finally
            {
                stopwatch.Stop();
                Logger.LogInformation("CreateAsync() took [{0}]", stopwatch.Elapsed);
            }
        }

        public async virtual Task UpdateAsync(TEntity entity)
        {
            Logger.LogDebug("UpdateAsync() | entity[{entity}]", entity);

            if (entity == null)
            {
                Logger.LogWarning("UpdateAsync() | invalid entity");
                throw new EntityException("Entity is null");
            }

            var stopwatch = Stopwatch.StartNew();

            try
            {
                DbSet.Attach(entity);
                DbContext.Entry(entity).State = EntityState.Modified;

                await DbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Logger.LogError($"UpdateAsync() | error [{ex}]", ex);
                throw new EntityException($"UpdateAsync() threw exception: {ex.Message}. See inner exception for details", ex);
            }
            finally
            {
                stopwatch.Stop();
                Logger.LogInformation("**** UpdateAsync() took [{0}]", stopwatch.Elapsed);
            }
        }

        public async Task DeleteAsync(TEntity entity)
        {
            Logger.LogDebug("DeleteAsync() | entity[{entity}]", entity);
            var stopwatch = Stopwatch.StartNew();

            try
            {
                if (entity == null)
                {
                    Logger.LogWarning("DeleteAsync() | invalid entity");
                    throw new EntityException("Entity is null");
                }

                entity.Deleted = DataLiterals.Yes;
                await UpdateAsync(entity);
            }
            catch (Exception ex)
            {
                Logger.LogError($"DeleteAsync() | error [{ex}]", ex);
                throw new EntityException($"DeleteAsync() for entity [{entity}] threw exception: {ex.Message}. See inner exception for details", ex);
            }
            finally
            {
                stopwatch.Stop();
                Logger.LogInformation("**** DeleteAsync() took [{0}]", stopwatch.Elapsed);
            }
        }

        public abstract Task RemoveAsync(TEntity entity);

        public bool Exists(TEntity entity)
        {
            Logger.LogDebug("Exists() | entity[{0}]", entity);

            return DbContext.Set<TEntity>().Any(e => e == entity);
        }

        public bool IsTracked(TEntity entity)
        {
            Logger.LogDebug("IsTracked() | entity[{entity}]", entity);

            return DbContext.Set<TEntity>().Local.Any(e => e == entity);
        }
    }
}