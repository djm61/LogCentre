using LogCentre.Data;
using LogCentre.Data.Entities;
using LogCentre.Data.Entities.Log;
using LogCentre.Data.Interfaces;
using LogCentre.Services.Exceptions;
using LogCentre.Services.Interfaces.Log;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using System.Diagnostics;

namespace LogCentre.Services.Services.Log
{
    public class LineService : AbstractService<LineService, Line, long>, ILineService
    {
        public LineService(ILogger<LineService> logger,
            ILogCentreDbContext dbContext)
            : base(logger, dbContext)
        {
        }

        public override bool TryGet(long id, out Line entity)
        {
            Logger.LogDebug("TryGet() | id[{id}]", id);

            entity = DbSet.FirstOrDefault(x => x.Id == id);
            return entity != null;
        }

        public async Task<Line> GetAsync(long id)
        {
            Logger.LogDebug("GetAsync() | id[{id}]", id);
            var stopwatch = Stopwatch.StartNew();

            try
            {
                if (id < 1)
                {
                    Logger.LogWarning("GetAsync() | invalid id");
                    throw new LineException("Invalid Id: " + id);
                }

                var entity = await GetAsync(t => t.Id == id && t.Deleted == DataLiterals.No,
                    q => q.OrderByDescending(d => d.RowVersion));

                return entity.FirstOrDefault();
            }
            catch (Exception ex)
            {
                Logger.LogError($"GetAsync() | error [{ex}]", ex);
                throw new LineException($"GetAsync() for ID [{id}] threw exception: {ex.Message}. See inner exception for details", ex);
            }
            finally
            {
                stopwatch.Stop();
                Logger.LogInformation("**** GetAsync() took [{0}]", stopwatch.Elapsed);
            }
        }

        public async Task<long> GetLineCountForFileAsync(long fileId)
        {
            Logger.LogDebug("GetLineCountForFileAsync() | fileId[{fileId}]", fileId);
            var stopwatch = Stopwatch.StartNew();

            try
            {
                if (fileId < 1)
                {
                    Logger.LogWarning("GetLineCountForFileAsync() | invalid file id");
                    throw new LineException("Invalid FileId: " + fileId);
                }

                var count = await CountAsync(t => t.FileId == fileId && t.Deleted == DataLiterals.No,
                    q => q.OrderByDescending(d => d.RowVersion));

                return count;
            }
            catch (Exception ex)
            {
                Logger.LogError($"CountAsync() | error [{ex}]", ex);
                throw new LineException($"CountAsync() for File Id [{fileId}] thre exception: {ex.Message}.  See inner exception for detail", ex);
            }
            finally
            {
                stopwatch.Stop();
                Logger.LogInformation("**** CountAsync() took [{0}]", stopwatch.Elapsed);
            }
        }

        public override async Task RemoveAsync(Line entity)
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
                throw new LineException($"RemoveAsync() for entity [{entity}] threw exception: {ex.Message}. See inner exception for details", ex);
            }
            finally
            {
                stopwatch.Stop();
                Logger.LogInformation("**** RemoveAsync() took [{0}]", stopwatch.Elapsed);
            }
        }
    }
}
