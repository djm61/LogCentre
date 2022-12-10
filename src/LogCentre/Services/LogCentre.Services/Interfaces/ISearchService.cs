using LogCentre.Model.Search;

namespace LogCentre.Services.Interfaces
{
    public interface ISearchService
    {
        Task<IList<string>> GetDistinctLogLevelsAsync(CancellationToken cancellationToken = default);
        Task<IList<SearchResultModel>> SearchAsync(SearchModel searchModel, CancellationToken cancellationToken = default);
    }
}
