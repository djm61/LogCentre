using LogCentre.Data.Configuration;
using LogCentre.Data.Configuration.Log;
using LogCentre.Data.Entities;
using LogCentre.Data.Entities.Log;
using LogCentre.Data.Interfaces;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LogCentre.Data
{
    public class LogCentreDbContext : DbContext, ILogCentreDbContext
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly ILogger<LogCentreDbContext> _logger;

        public LogCentreDbContext(DbContextOptions<LogCentreDbContext> options, ILoggerFactory loggerFactory)
            : base(options)
        {
            _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
            _logger = loggerFactory.CreateLogger<LogCentreDbContext>();
        }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseLoggerFactory(_loggerFactory);
        //    base.OnConfiguring(optionsBuilder);
        //}

        #region Tables

        public DbSet<Host> Hosts { get; set; }
        public DbSet<Provider> Providers { get; set; }
        public DbSet<LogSource> Sources { get; set; }
        public DbSet<Entities.Log.File> LogFiles { get; set; }
        public DbSet<Line> LogLines { get; set; }

        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            _logger.LogDebug("OnModelCreating");
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new HostConfig());
            modelBuilder.ApplyConfiguration(new ProviderConfig());
            modelBuilder.ApplyConfiguration(new LogSourceConfig());
            modelBuilder.ApplyConfiguration(new FileConfig());
            modelBuilder.ApplyConfiguration(new LineConfig());
        }

        public override int SaveChanges()
        {
            SetRowVersion();
            return base.SaveChanges();
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            SetRowVersion();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            SetRowVersion();
            return base.SaveChangesAsync(cancellationToken);
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            SetRowVersion();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        private void SetRowVersion()
        {
            var entities = ChangeTracker.Entries()
                .Where(x => x.State == EntityState.Added || x.State == EntityState.Modified)
                .Select(x => x.Entity);
            foreach (var entity in entities)
            {
                if (entity is IBaseEntity<long> e)
                {
                    if (string.IsNullOrWhiteSpace(e.LastUpdatedBy))
                    {
                        e.LastUpdatedBy = "LastUpdatedBy Missing";
                    }

                    e.RowVersion = DateTime.UtcNow;
                }
            }
        }
    }
}
