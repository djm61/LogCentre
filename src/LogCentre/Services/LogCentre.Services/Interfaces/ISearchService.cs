using LogCentre.Model.Search;

namespace LogCentre.Services.Interfaces
{
    public interface ISearchService
    {
        Task<IList<ItemModel>> SearchAsync(string dataItem, CancellationToken cancellationToken = default);
    }
}
