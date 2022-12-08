namespace LogCentre.Api.Services
{
    /// <summary>
    /// Interface for the Distributed Cache Service
    /// </summary>
    public interface IDistributedCacheService
    {
        /// <summary>
        /// Get from cache
        /// </summary>
        /// <typeparam name="T">The type of the item being retrieved</typeparam>
        /// <param name="key">Key of the cache</param>
        /// <param name="cancellationToken">cancellation token</param>
        /// <returns>Item of type T</returns>
        Task<T?> GetFromCache<T>(string key, CancellationToken cancellationToken = default) where T : class;

        /// <summary>
        /// Sets an item into the cache
        /// </summary>
        /// <typeparam name="T">The type of the item being cached</typeparam>
        /// <param name="key">Key of the cache</param>
        /// <param name="value">Value that is being cached</param>
        /// <param name="cancellationToken">cancellation token</param>
        /// <returns>Item of type T</returns>
        Task SetCache<T>(string key, T value, CancellationToken cancellationToken = default);

        /// <summary>
        /// Clears a cache entry with the provided key
        /// </summary>
        /// <param name="key">Key of the cache</param>
        /// <param name="cancellationToken">cancellation token</param>
        /// <returns>void</returns>
        Task ClearCache(string key, CancellationToken cancellationToken = default);
    }
}
