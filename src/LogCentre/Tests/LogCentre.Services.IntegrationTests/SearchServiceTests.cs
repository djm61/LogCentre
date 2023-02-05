using LogCentre.Data;
using LogCentre.Model;
using LogCentre.Model.Search;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;

using System.Threading;

namespace LogCentre.Services.IntegrationTests
{
    public class SearchServiceTests
    {
        private const string ConnectionString = "Server=(localdb)\\mssqllocaldb;Database=LogCentre-local;Trusted_Connection=True;MultipleActiveResultSets=true;App=LogCentre";

        [Fact]
        public void TestSearchGrouping_ReturnsListOfLines()
        {
            var loggerFactory = new NullLoggerFactory();
            var dbContextOptionsBuilder = new DbContextOptionsBuilder<LogCentreDbContext>()
                .UseLoggerFactory(loggerFactory)
                .EnableDetailedErrors()
                .EnableSensitiveDataLogging()
                .UseSqlServer(ConnectionString);

            var dbContext = new LogCentreDbContext(dbContextOptionsBuilder.Options, loggerFactory);

            var lines = from line in dbContext.LogLines
                        group line by line.Grouping into g
                        orderby g.Min(l => l.LogDate)
                        select new
                        {
                            Grouping = g.Key,
                            Lines = g.OrderBy(x => x.LogDate)
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
                                }).ToList()
                            //Lines = g.ToList()
                        };
            var enumerableLines = lines.AsNoTracking().AsEnumerable();
            var results = enumerableLines.SelectMany(l => l.Lines).ToList();

            Assert.NotNull(results);
            Assert.IsType<List<SearchResultModel>>(results);
            Assert.True(results.Any());
        }

        [Fact]
        public async Task TestFileContents_ValidId_ReturnsFileContents()
        {
            const long lineId = 1;
            var loggerFactory = new NullLoggerFactory();
            var dbContextOptionsBuilder = new DbContextOptionsBuilder<LogCentreDbContext>()
                .UseLoggerFactory(loggerFactory)
                .EnableDetailedErrors()
                .EnableSensitiveDataLogging()
                .UseSqlServer(ConnectionString);

            var dbContext = new LogCentreDbContext(dbContextOptionsBuilder.Options, loggerFactory);

            var query = from line in dbContext.LogLines
                        join file in dbContext.LogFiles on line.FileId equals file.Id
                        join l in dbContext.LogLines on file.Id equals l.FileId
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
            var items = await query.AsNoTracking().ToListAsync();
            Assert.NotNull(items);
            Assert.True(items.Any());
        }
    }
}