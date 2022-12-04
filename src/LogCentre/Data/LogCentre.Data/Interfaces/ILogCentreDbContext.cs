using LogCentre.Data.Entities;
using LogCentre.Data.Entities.Log;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace LogCentre.Data.Interfaces
{
    public interface ILogCentreDbContext
    {
        #region Tables

        public DbSet<Host> Hosts { get; set; }
        public DbSet<Provider> Providers { get; set; }
        public DbSet<LogSource> Sources { get; set; }
        public DbSet<Line> LogLines { get; set; }

        #endregion

        #region DbContext Overrides

        DatabaseFacade Database { get; }
        ChangeTracker ChangeTracker { get; }

        int SaveChanges();
        int SaveChanges(bool acceptAllChangesOnSuccess);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default);

        EntityEntry Entry(object entity);

        DbSet<TEntity> Set<TEntity>() where TEntity : class;

        #endregion
    }
}
