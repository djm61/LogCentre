using LogCentre.Data;
using LogCentre.Data.Entities;
using LogCentre.Data.Interfaces;
using LogCentre.Services.Exceptions;
using LogCentre.Services.Interfaces;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using System.Diagnostics;

namespace LogCentre.Services.Services
{
    public class LogSourceService : AbstractService<LogSourceService, LogSource, long>, ILogSourceService
    {
        private const string IncludeTables = "Host,Provider";

        public LogSourceService(ILogger<LogSourceService> logger,
            ILogCentreDbContext dbContext)
            : base(logger, dbContext)
        {
        }

        public override bool TryGet(long id, out LogSource entity)
        {
            Logger.LogDebug("TryGet() | id[{id}]", id);

            entity = DbSet
                .Include(a => a.Host)
                .Include(a => a.Provider)
                .FirstOrDefault(x => x.Id == id);
            return entity != null;
        }

        public async Task<LogSource> GetAsync(long id)
        {
            Logger.LogDebug("GetAsync() | id[{id}]", id);
            var stopwatch = Stopwatch.StartNew();

            try
            {
                if (id < 1)
                {
                    Logger.LogWarning("GetAsync() | invalid id");
                    throw new LogSourceException("Invalid Id: " + id);
                }

                var entity = await GetAsync(t => t.Id == id && t.Deleted == DataLiterals.No,
                    q => q.OrderBy(d => d.Id), IncludeTables);

                return entity.FirstOrDefault();
            }
            catch (Exception e)
            {
                Logger.LogError($"GetAsync() | error [{e}]", e);
                throw new LogSourceException($"GetAsync() for ID [{id}] threw exception: {e.Message}. See inner exception for details", e);
            }
            finally
            {
                stopwatch.Stop();
                Logger.LogInformation("**** GetAsync() took [{0}]", stopwatch.Elapsed);
            }
        }

        public override async Task RemoveAsync(LogSource entity)
        {
            Logger.LogDebug("RemoveAsync() | entity[{entity}]", entity);
            var stopwatch = Stopwatch.StartNew();

            try
            {
                if (DbContext.Entry(entity).State == EntityState.Detached)
                {
                    DbSet.Attach(entity);
                }

                DbSet.Remove(entity);
                await DbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Logger.LogError($"RemoveAsync() | Error deleting temperature [{ex}]", ex);
                throw new LogSourceException($"RemoveAsync() for entity [{entity}] threw exception: {ex.Message}. See inner exception for details", ex);
            }
            finally
            {
                stopwatch.Stop();
                Logger.LogInformation("**** RemoveAsync() took [{0}]", stopwatch.Elapsed);
            }
        }
    }
}
