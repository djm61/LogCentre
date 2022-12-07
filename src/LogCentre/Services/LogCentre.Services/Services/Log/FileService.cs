using LogCentre.Data;
using LogCentre.Services.Exceptions;
using LogCentre.Services.Interfaces.Log;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using System.Diagnostics;

namespace LogCentre.Services.Services.Log
{
    public class FileService : AbstractService<FileService, Data.Entities.Log.File, long>, IFileService
    {
        public FileService(ILogger<FileService> logger,
            IDbContextFactory<LogCentreDbContext> dbContextFactory)
            : base(logger, dbContextFactory)
        {
        }

        public override bool TryGet(long id, out Data.Entities.Log.File entity)
        {
            Logger.LogDebug("TryGet() | id[{id}]", id);

            entity = DbSet.FirstOrDefault(x => x.Id == id);
            return entity != null;
        }

        public async Task<Data.Entities.Log.File> GetAsync(long id)
        {
            Logger.LogDebug("GetAsync() | id[{id}]", id);
            var stopwatch = Stopwatch.StartNew();

            try
            {
                if (id < 1)
                {
                    Logger.LogWarning("GetAsync() | invalid id");
                    throw new FileException("Invalid Id: " + id);
                }

                var entity = await GetAsync(t => t.Id == id && t.Deleted == DataLiterals.No,
                    q => q.OrderBy(d => d.Name));

                return entity.FirstOrDefault();
            }
            catch (Exception ex)
            {
                Logger.LogError($"GetAsync() | error [{ex}]", ex);
                throw new FileException($"GetAsync() for ID [{id}] threw exception: {ex.Message}. See inner exception for details", ex);
            }
            finally
            {
                stopwatch.Stop();
                Logger.LogInformation("**** GetAsync() took [{0}]", stopwatch.Elapsed);
            }
        }

        public override async Task RemoveAsync(Data.Entities.Log.File entity)
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
                throw new FileException($"RemoveAsync() for entity [{entity}] threw exception: {ex.Message}. See inner exception for details", ex);
            }
            finally
            {
                stopwatch.Stop();
                Logger.LogInformation("**** RemoveAsync() took [{0}]", stopwatch.Elapsed);
            }
        }
    }
}
