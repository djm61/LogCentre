using LogCentre.Data;
using LogCentre.Data.Interfaces;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LogCentre.BackgroundServices
{
    public class LogBackgroundService : BackgroundService
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly ILogger<LogBackgroundService> _logger;
        private readonly LogBackgroundServiceSettings _settings;
        private readonly IDbContextFactory<LogCentreDbContext> _dbContextFactory;
        private readonly TimeSpan _period;

        private bool IsRunning { get; set; }

        public LogBackgroundService(ILoggerFactory loggerFactory,
            LogBackgroundServiceSettings settings,
            IDbContextFactory<LogCentreDbContext> dbContextFactory)
        {
            _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
            _logger = loggerFactory.CreateLogger<LogBackgroundService>();
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _dbContextFactory = dbContextFactory ?? throw new ArgumentNullException(nameof(dbContextFactory));

            _period = TimeSpan.FromMilliseconds(_settings.UpdateInterval);

            IsRunning = false;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogDebug("ExecuteAsync()");
            if (!_settings.Enabled)
            {
                _logger.LogWarning("ExecuteAsync() | background service is not enalbed, exit");
                return;
            }

            if (IsRunning) return;

            var timer = new PeriodicTimer(_period);
            while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken))
            {
                _logger.LogDebug("ExecuteAsync() | tick!");

                //todo prefrom background service settings
                var dbContext = await _dbContextFactory.CreateDbContextAsync(stoppingToken);
            }
        }
    }
}