﻿using LogCentre.Data;
using LogCentre.Data.Entities;
using LogCentre.Data.Interfaces;
using LogCentre.Services.Exceptions;
using LogCentre.Services.Interfaces;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using System.Diagnostics;

namespace LogCentre.Services
{
    public class HostService : AbstractService<HostService, Host, long>, IHostService
    {
        public HostService(ILogger<HostService> logger,
            ILogCentreDbContext dbContext)
            : base(logger, dbContext)
        {
        }

        public override bool TryGet(long id, out Host entity)
        {
            Logger.LogDebug("TryGet() | id[{id}]", id);

            entity = DbSet.FirstOrDefault(x => x.Id == id);
            return entity != null;
        }

        public async Task<Host> GetAsync(long id)
        {
            Logger.LogDebug("GetAsync() | id[{id}]", id);
            var stopwatch = Stopwatch.StartNew();

            try
            {
                if (id < 1)
                {
                    Logger.LogWarning("GetAsync() | invalid id");
                    throw new HostException("Invalid Id: " + id);
                }

                var entity = await GetAsync(t => t.Id == id && t.Deleted == DataLiterals.No,
                    q => q.OrderBy(d => d.Name));

                return entity.FirstOrDefault();
            }
            catch (Exception ex)
            {
                Logger.LogError($"GetAsync() | error [{ex}]", ex);
                throw new HostException($"GetAsync() for ID [{id}] threw exception: {ex.Message}. See inner exception for details", ex);
            }
            finally
            {
                stopwatch.Stop();
                Logger.LogInformation("**** GetAsync() took [{0}]", stopwatch.Elapsed);
            }
        }

        public override async Task RemoveAsync(Host entity)
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
                throw new HostException($"RemoveAsync() for entity [{entity}] threw exception: {ex.Message}. See inner exception for details", ex);
            }
            finally
            {
                stopwatch.Stop();
                Logger.LogInformation("**** RemoveAsync() took [{0}]", stopwatch.Elapsed);
            }
        }
    }
}
