using Microsoft.Extensions.Caching.Memory;

namespace LogCentre.Api.Helpers
{
    public static class CacheExtensions
    {
        public static IServiceCollection AddDistributedCache(this IServiceCollection services, CachingSettings cachingSettings)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (!cachingSettings.Enabled) return services;
            switch (cachingSettings.CacheType)
            {
                case CacheType.Redis:
                    if (string.IsNullOrWhiteSpace(cachingSettings.ConnectionString)) throw new ArgumentNullException(nameof(cachingSettings.ConnectionString));
                    services.AddStackExchangeRedisCache(options =>
                    {
                        options.InstanceName = "LogCentreInstance";
                        options.Configuration = cachingSettings.ConnectionString;
                    });
                    break;
                case CacheType.Memory:
                    services.AddDistributedMemoryCache();
                    break;
                case CacheType.Null:
                default:
                    break;
            }

            return services;
        }
    }
}
