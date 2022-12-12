using LogCentre.Data;
using LogCentre.Model;
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
                .ThenInclude(x => x.LogSource)
                .ThenInclude(x => x.Provider)
                .Include(x => x.File)
                .ThenInclude(x => x.LogSource)
                .ThenInclude(x => x.Host)
                .OrderByDescending(x => x.LogDate)
                .ThenBy(x => x.File.Name)
                .AsNoTracking()
                .Where(x => x.Active == ModelLiterals.Yes
                    && x.Deleted == ModelLiterals.No
                    && x.File.Active == ModelLiterals.Yes
                    && x.File.Deleted == ModelLiterals.No
                    && x.File.LogSource.Active == ModelLiterals.Yes
                    && x.File.LogSource.Deleted == ModelLiterals.No
                    && x.File.LogSource.Provider.Active == ModelLiterals.Yes
                    && x.File.LogSource.Provider.Deleted == ModelLiterals.No
                    && x.File.LogSource.Host.Active == ModelLiterals.Yes
                    && x.File.LogSource.Host.Deleted == ModelLiterals.No);

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
                    FileId = x.FileId,
                    LogDate = x.LogDate,
                    Level = x.Level,
                    Source = x.Source,
                    LogLine = x.LogLine,
                    FileModel = x.File == null ? null : new Model.Log.FileModel
                    {
                        Id = x.File.Id,
                        LogSourceId = x.File.LogSourceId,
                        Name = x.File.Name,
                        FileComplete = x.File.FileComplete,
                        Active = x.File.Active,
                        Deleted = x.File.Deleted,
                        RowVersion = x.File.RowVersion,
                        LogSource = x.File.LogSource == null ? null : new LogSourceModel
                        {
                            Id = x.File.LogSource.Id,
                            HostId = x.File.LogSource.HostId,
                            ProviderId = x.File.LogSource.ProviderId,
                            Name = x.File.LogSource.Name,
                            Path = x.File.LogSource.Path,
                            Active = x.File.LogSource.Active,
                            Deleted = x.File.LogSource.Deleted,
                            RowVersion = x.File.LogSource.RowVersion,
                            Host = x.File.LogSource.Host == null ? null : new HostModel
                            {
                                Id = x.File.LogSource.Host.Id,
                                Name = x.File.LogSource.Host.Name,
                                Description = x.File.LogSource.Host.Description,
                                Active = x.File.LogSource.Host.Active,
                                Deleted = x.File.LogSource.Host.Deleted,
                                RowVersion = x.File.LogSource.Host.RowVersion
                            },
                            Provider = x.File.LogSource.Provider == null ? null : new ProviderModel
                            {
                                Id = x.File.LogSource.Provider.Id,
                                Name = x.File.LogSource.Provider.Name,
                                Description = x.File.LogSource.Provider.Description,
                                Regex = x.File.LogSource.Provider.Regex,
                                Active = x.File.LogSource.Provider.Active,
                                Deleted = x.File.LogSource.Provider.Deleted,
                                RowVersion = x.File.LogSource.Provider.RowVersion
                            }
                        }
                    }
                })
                .ToListAsync(cancellationToken);
            return items;
        }
    }
}
