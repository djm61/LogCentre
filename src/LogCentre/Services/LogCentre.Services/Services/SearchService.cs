using LogCentre.Data;
using LogCentre.Model.Search;
using LogCentre.Services.Interfaces;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LogCentre.Services.Services
{
    public class SearchService : ISearchService
    {
        private readonly ILogger<SearchService> _logger;
        private readonly LogCentreDbContext _dbContext;

        public SearchService(ILogger<SearchService> logger,
            IDbContextFactory<LogCentreDbContext> dbContextFactory)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            var factory = dbContextFactory ?? throw new ArgumentNullException(nameof(dbContextFactory));
            _dbContext = factory.CreateDbContext() ?? throw new ArgumentNullException(nameof(dbContextFactory));
        }

        public async Task<IList<string>> GetDistinctLogLevelsAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogDebug("GetDistinctLogLevelsAsync()");

            var items = await _dbContext.LogLines
                .Select(x => x.Level)
                .Distinct()
                .ToListAsync(cancellationToken);

            return items;
        }

        public async Task<IList<SearchResultModel>> SearchAsync(SearchModel searchModel, CancellationToken cancellationToken = default)
        {
            _logger.LogDebug("SearchAsync() | searchModel[{searchModel}]", searchModel);
            var itemQuery = _dbContext.LogLines
                .Include(x => x.File)
                .OrderByDescending(x => x.LogDate)
                .AsNoTracking();

            if (searchModel.StartDate.HasValue)
            {
                itemQuery = itemQuery.Where(x => x.LogDate >= searchModel.StartDate.Value);
            }

            if (searchModel.EndDate.HasValue)
            {
                itemQuery = itemQuery.Where(x => x.LogDate <= searchModel.EndDate.Value);
            }

            if (!string.IsNullOrWhiteSpace(searchModel.Level))
            {
                itemQuery = itemQuery.Where(x => x.Level.ToUpper() == searchModel.Level.ToUpper());
            }

            if (!string.IsNullOrWhiteSpace(searchModel.Source))
            {
                itemQuery = itemQuery.Where(x => x.Source.ToUpper().Contains(searchModel.Source.ToUpper()));
            }

            if (!string.IsNullOrWhiteSpace(searchModel.LogLine))
            {
                itemQuery = itemQuery.Where(x => x.LogLine.ToUpper().Contains(searchModel.LogLine.ToUpper()));
            }

            var items = await itemQuery
                .Select(x => new SearchResultModel
                {
                    Id = x.Id,
                    LogDate = x.LogDate,
                    Level = x.Level,
                    Source = x.Source,
                    LogLine = x.LogLine,
                })
                .ToListAsync(cancellationToken);
            return items;
        }
    }
}
