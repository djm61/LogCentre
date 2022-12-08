using LogCentre.Model.Cache;

namespace LogCentre.Services.Interfaces
{
    public interface ICacheSearchService
    {
        Task<IList<CacheItemModel>> SearchAsync(string dataItem);
    }
}
