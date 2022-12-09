using LogCentre.Api.Helpers;
using LogCentre.Api.Models;
using LogCentre.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using System.Text.RegularExpressions;

namespace LogCentre.Api.Services
{
    /// <summary>
    /// Log Background Service
    /// </summary>
    public class LogBackgroundService : BackgroundService
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly ILogger<LogBackgroundService> _logger;
        private readonly LogBackgroundServiceSettings _backgroundServiceSettings;
        private readonly IDbContextFactory<LogCentreDbContext> _dbContextFactory;
        private readonly IDistributedCacheService _distributedCacheService;
        private readonly TimeSpan _period;

        private bool IsRunning { get; set; }

        /// <summary>
        /// Log Background Service
        /// </summary>
        /// <param name="loggerFactory">Logging Factory</param>
        /// <param name="backgroundServiceSettings">Settings for the background service</param>
        /// <param name="dbContextFactory">DB Context Factory</param>
        /// <param name="distributedCacheService">Distributed Cache</param>
        /// <exception cref="ArgumentNullException">Thrown is anything is null</exception>
        public LogBackgroundService(ILoggerFactory loggerFactory,
            LogBackgroundServiceSettings backgroundServiceSettings,
            IDbContextFactory<LogCentreDbContext> dbContextFactory,
            IDistributedCacheService distributedCacheService)
        {
            _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
            _logger = loggerFactory.CreateLogger<LogBackgroundService>();
            _backgroundServiceSettings = backgroundServiceSettings ?? throw new ArgumentNullException(nameof(backgroundServiceSettings));
            _dbContextFactory = dbContextFactory ?? throw new ArgumentNullException(nameof(dbContextFactory));
            _distributedCacheService = distributedCacheService ?? throw new ArgumentNullException(nameof(distributedCacheService));

            _period = TimeSpan.FromMilliseconds(_backgroundServiceSettings.UpdateInterval);

            IsRunning = false;
        }

        /// <summary>
        /// The override of the Execute method for the background service
        /// </summary>
        /// <param name="stoppingToken"></param>
        /// <returns></returns>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogDebug("ExecuteAsync()");
            if (!_backgroundServiceSettings.Enabled)
            {
                _logger.LogWarning("ExecuteAsync() | background service is not enalbed, exit");
                return;
            }

            if (IsRunning) return;

            var timer = new PeriodicTimer(_period);
            while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken))
            {
                _logger.LogDebug("ExecuteAsync() | tick! IsRunning[{IsRunning}]", IsRunning);
                if (IsRunning) break;

                IsRunning = true;
                if (stoppingToken.IsCancellationRequested)
                {
                    IsRunning = false;
                    return;
                }

                var dbContext = await _dbContextFactory.CreateDbContextAsync(stoppingToken);
                if (stoppingToken.IsCancellationRequested)
                {
                    IsRunning = false;
                    return;
                }

                //var logLines = await dbContext.LogLines
                //    .Include(x => x.File)
                //    .ThenInclude(x => x.LogSource)
                //    .ThenInclude(x => x.Provider)
                //    .Include(x => x.File)
                //    .ThenInclude(x => x.LogSource)
                //    .ThenInclude(x => x.Host)
                //    .Where(x => x.Active == DataLiterals.Yes && x.Deleted == DataLiterals.No)
                //    .OrderByDescending(x => x.RowVersion)
                //    .AsNoTracking()
                //    .ToListAsync(stoppingToken);
                //if (stoppingToken.IsCancellationRequested)
                //{
                //    IsRunning = false;
                //    return;
                //}

                //foreach (var logLine in logLines)
                //{
                //    var regExValue = logLine.File?.LogSource?.Provider?.Regex ?? string.Empty;
                //    if (string.IsNullOrWhiteSpace(regExValue)) continue;

                //    var items = await _distributedCacheService.GetFromCache<IList<CacheItemModel>>(Literals.CacheKey, stoppingToken);
                //    if (items == null)
                //    {
                //        items = new List<CacheItemModel>();
                //    }

                //    if (stoppingToken.IsCancellationRequested)
                //    {
                //        IsRunning = false;
                //        return;
                //    }

                //    var found = items.FirstOrDefault(x => x.Id == logLine.Id);
                //    if (found != null)
                //    {
                //        continue;
                //    }

                //    var regex = new Regex(regExValue);
                //    var matches = regex.Match(logLine.LogLine);
                //    if (stoppingToken.IsCancellationRequested)
                //    {
                //        IsRunning = false;
                //        return;
                //    }

                //    if (matches.Success)
                //    {
                //        var lineModel = new CacheItemModel
                //        {
                //            Id = logLine.Id
                //        };

                //        if (stoppingToken.IsCancellationRequested)
                //        {
                //            IsRunning = false;
                //            return;
                //        }
                //        foreach (Group group in matches.Groups)
                //        {
                //            if (stoppingToken.IsCancellationRequested)
                //            {
                //                IsRunning = false;
                //                return;
                //            }

                //            if (group.Name == "0") continue;
                //            lineModel.Line.Add(group.Name, group.Value);
                //        }

                //        if (stoppingToken.IsCancellationRequested)
                //        {
                //            IsRunning = false;
                //            return;
                //        }

                //        items.Add(lineModel);
                //        await _distributedCacheService.SetCache(Literals.CacheKey, items, stoppingToken);
                //    }
                //}

                IsRunning = false;
            }
        }
    }
}