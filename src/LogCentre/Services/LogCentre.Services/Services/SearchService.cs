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

        public async Task<IList<ItemModel>> SearchAsync(string dataItem, CancellationToken cancellationToken = default)
        {
            _logger.LogDebug("SearchAsync() | dataItem[{dataItem}]", dataItem);
            var count = await _dbContext.LogLines.CountAsync(cancellationToken);
            return new List<ItemModel>();
        }
    }
}
