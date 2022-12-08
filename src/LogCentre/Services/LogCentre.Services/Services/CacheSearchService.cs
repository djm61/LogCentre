using LogCentre.Model.Cache;
using LogCentre.Services.Interfaces;

namespace LogCentre.Services.Services
{
    public class CacheSearchService : ICacheSearchService
    {
        public Task<IList<CacheItemModel>> SearchAsync(string dataItem)
        {
            throw new NotImplementedException();
        }
    }
}
