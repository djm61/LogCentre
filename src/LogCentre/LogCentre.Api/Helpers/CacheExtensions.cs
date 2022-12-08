namespace LogCentre.Api.Helpers
{
    /// <summary>
    /// Distributed Cache Extensions
    /// </summary>
    public static class CacheExtensions
    {
        /// <summary>
        /// Extention method for figuring out which type of distributed cache to use
        /// </summary>
        /// <param name="services">IServiceCollection</param>
        /// <param name="cachingSettings">Cache settings</param>
        /// <returns>Service Collection</returns>
        /// <exception cref="ArgumentNullException">Thrown if anything is null</exception>
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
