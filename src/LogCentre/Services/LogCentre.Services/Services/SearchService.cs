using LogCentre.Data;
using LogCentre.Model;
using LogCentre.Model.Log;
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
            var itemQuery = from line in _dbContext.LogLines
                            group line by line.Grouping into g
                            orderby g.Min(r => r.LogDate)
                            where (searchModel.StartDate.HasValue ? g.All(r => r.LogDate >= searchModel.StartDate.Value) : g.All(r => r.LogDate <= DateTime.MaxValue))
                            && (searchModel.EndDate.HasValue ? g.All(r => r.LogDate <= searchModel.EndDate.Value) : g.All(r => r.LogDate >= DateTime.MinValue))
                            && (searchModel.Level != null && searchModel.Level != "" ? g.All(r => r.Level.ToUpper() == searchModel.Level.ToUpper()) : true)
                            && (searchModel.Source != null && searchModel.Source != "" ? g.All(r => r.Source.ToUpper().Contains(searchModel.Source.ToUpper())) : true)
                            && (searchModel.LogLine != null && searchModel.LogLine != "" ? g.All(r => r.LogLine.ToUpper().Contains(searchModel.LogLine.ToUpper())) : true)
                            select new
                            {
                                Grouping = g.Key,
                                Lines = g.OrderBy(x => x.LogDate)
                                    .ThenBy(x => x.File.Name)
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
                                    }).ToList(),
                                //Lines = g.ToList()
                            };

            //var enumerableItems = itemQuery.AsNoTracking().AsEnumerable();
            var enumerableItems = itemQuery.AsNoTracking().AsAsyncEnumerable();
            var items = new List<SearchResultModel>();
            await foreach (var item in enumerableItems
                .WithCancellation(cancellationToken)
                .ConfigureAwait(false))
            {
                items.AddRange(item.Lines);
            }
            //var items = enumerableItems.SelectMany(l => l.Lines).ToList();

            //var itemQuery = _dbContext.LogLines
            //    .Include(x => x.File)
            //    .ThenInclude(x => x.LogSource)
            //    .ThenInclude(x => x.Provider)
            //    .Include(x => x.File)
            //    .ThenInclude(x => x.LogSource)
            //    .ThenInclude(x => x.Host)
            //    .OrderBy(x => x.LogDate)
            //    .ThenBy(x => x.File.Name)
            //    .AsNoTracking()
            //    .Where(x => x.Active == ModelLiterals.Yes
            //        && x.Deleted == ModelLiterals.No
            //        && x.File.Active == ModelLiterals.Yes
            //        && x.File.Deleted == ModelLiterals.No
            //        && x.File.LogSource.Active == ModelLiterals.Yes
            //        && x.File.LogSource.Deleted == ModelLiterals.No
            //        && x.File.LogSource.Provider.Active == ModelLiterals.Yes
            //        && x.File.LogSource.Provider.Deleted == ModelLiterals.No
            //        && x.File.LogSource.Host.Active == ModelLiterals.Yes
            //        && x.File.LogSource.Host.Deleted == ModelLiterals.No);

            //if (searchModel.StartDate.HasValue)
            //{
            //    itemQuery = itemQuery.Where(x => x.LogDate >= searchModel.StartDate.Value);
            //}

            //if (searchModel.EndDate.HasValue)
            //{
            //    itemQuery = itemQuery.Where(x => x.LogDate <= searchModel.EndDate.Value);
            //}

            //if (!string.IsNullOrWhiteSpace(searchModel.Level))
            //{
            //    itemQuery = itemQuery.Where(x => x.Level.ToUpper() == searchModel.Level.ToUpper());
            //}

            //if (!string.IsNullOrWhiteSpace(searchModel.Source))
            //{
            //    itemQuery = itemQuery.Where(x => x.Source.ToUpper().Contains(searchModel.Source.ToUpper()));
            //}

            //if (!string.IsNullOrWhiteSpace(searchModel.LogLine))
            //{
            //    itemQuery = itemQuery.Where(x => x.LogLine.ToUpper().Contains(searchModel.LogLine.ToUpper()));
            //}

            //var items = await itemQuery
            //    .Select(x => new SearchResultModel
            //    {
            //        Id = x.Id,
            //        FileId = x.FileId,
            //        LogDate = x.LogDate,
            //        Level = x.Level,
            //        Source = x.Source,
            //        LogLine = x.LogLine,
            //        FileModel = x.File == null ? null : new Model.Log.FileModel
            //        {
            //            Id = x.File.Id,
            //            LogSourceId = x.File.LogSourceId,
            //            Name = x.File.Name,
            //            FileComplete = x.File.FileComplete,
            //            Active = x.File.Active,
            //            Deleted = x.File.Deleted,
            //            RowVersion = x.File.RowVersion,
            //            LogSource = x.File.LogSource == null ? null : new LogSourceModel
            //            {
            //                Id = x.File.LogSource.Id,
            //                HostId = x.File.LogSource.HostId,
            //                ProviderId = x.File.LogSource.ProviderId,
            //                Name = x.File.LogSource.Name,
            //                Path = x.File.LogSource.Path,
            //                Active = x.File.LogSource.Active,
            //                Deleted = x.File.LogSource.Deleted,
            //                RowVersion = x.File.LogSource.RowVersion,
            //                Host = x.File.LogSource.Host == null ? null : new HostModel
            //                {
            //                    Id = x.File.LogSource.Host.Id,
            //                    Name = x.File.LogSource.Host.Name,
            //                    Description = x.File.LogSource.Host.Description,
            //                    Active = x.File.LogSource.Host.Active,
            //                    Deleted = x.File.LogSource.Host.Deleted,
            //                    RowVersion = x.File.LogSource.Host.RowVersion
            //                },
            //                Provider = x.File.LogSource.Provider == null ? null : new ProviderModel
            //                {
            //                    Id = x.File.LogSource.Provider.Id,
            //                    Name = x.File.LogSource.Provider.Name,
            //                    Description = x.File.LogSource.Provider.Description,
            //                    Regex = x.File.LogSource.Provider.Regex,
            //                    Active = x.File.LogSource.Provider.Active,
            //                    Deleted = x.File.LogSource.Provider.Deleted,
            //                    RowVersion = x.File.LogSource.Provider.RowVersion
            //                }
            //            }
            //        }
            //    })
            //    .ToListAsync(cancellationToken);
            return items;
        }

        public async Task<IList<SearchResultModel>> GetFileLinesAsync(long lineId, CancellationToken cancellationToken = default)
        {
            _logger.LogDebug("GetFileLinesASync() | lineId[{lineId}]", lineId);
            var query = from line in _dbContext.LogLines
                        join file in _dbContext.LogFiles on line.FileId equals file.Id
                        join l in _dbContext.LogLines on file.Id equals l.FileId
                        where line.Id == lineId
                        orderby l.LogDate
                        select new SearchResultModel
                        {
                            Id = l.Id,
                            FileId = l.FileId,
                            LogDate = l.LogDate,
                            Level = l.Level,
                            Source = l.Source,
                            LogLine = l.LogLine,
                            FileModel = l.File == null ? null : new Model.Log.FileModel
                            {
                                Id = l.File.Id,
                                LogSourceId = l.File.LogSourceId,
                                Name = l.File.Name,
                                FileComplete = l.File.FileComplete,
                                Active = l.File.Active,
                                Deleted = l.File.Deleted,
                                RowVersion = l.File.RowVersion,
                                LogSource = l.File.LogSource == null ? null : new LogSourceModel
                                {
                                    Id = l.File.LogSource.Id,
                                    HostId = l.File.LogSource.HostId,
                                    ProviderId = l.File.LogSource.ProviderId,
                                    Name = l.File.LogSource.Name,
                                    Path = l.File.LogSource.Path,
                                    Active = l.File.LogSource.Active,
                                    Deleted = l.File.LogSource.Deleted,
                                    RowVersion = l.File.LogSource.RowVersion,
                                    Host = l.File.LogSource.Host == null ? null : new HostModel
                                    {
                                        Id = l.File.LogSource.Host.Id,
                                        Name = l.File.LogSource.Host.Name,
                                        Description = l.File.LogSource.Host.Description,
                                        Active = l.File.LogSource.Host.Active,
                                        Deleted = l.File.LogSource.Host.Deleted,
                                        RowVersion = l.File.LogSource.Host.RowVersion
                                    },
                                    Provider = l.File.LogSource.Provider == null ? null : new ProviderModel
                                    {
                                        Id = l.File.LogSource.Provider.Id,
                                        Name = l.File.LogSource.Provider.Name,
                                        Description = l.File.LogSource.Provider.Description,
                                        Regex = l.File.LogSource.Provider.Regex,
                                        Active = l.File.LogSource.Provider.Active,
                                        Deleted = l.File.LogSource.Provider.Deleted,
                                        RowVersion = l.File.LogSource.Provider.RowVersion
                                    }
                                }
                            }
                        };
            var items = query.AsNoTracking().ToListAsync(cancellationToken);
            return await items;
        }
    }
}
