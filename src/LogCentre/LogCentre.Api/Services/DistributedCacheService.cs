using LogCentre.Api.Helpers;
using LogCentre.Services.Interfaces;

using Microsoft.Extensions.Caching.Distributed;

using System.Text.Json;
using System.Text.Json.Serialization;

namespace LogCentre.Api.Services
{
    /// <summary>
    /// Distributed cache service implementation
    /// </summary>
    public class DistributedCacheService : IDistributedCacheService
    {
        private readonly ILogger<DistributedCacheService> _logger;
        private readonly IDistributedCache _cache;
        private readonly DistributedCacheEntryOptions _options;
        private readonly CachingSettings _cachingSettings;

        /// <summary>
        /// Json Serializer Options
        /// </summary>
        private JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            WriteIndented = true,
            Converters = { new JsonStringEnumConverter() }
        };

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logger">Logger</param>
        /// <param name="cache">distributed cache</param>
        /// <param name="cachingSettings">cache settings</param>
        /// <exception cref="ArgumentNullException">Thrown if anything is null</exception>
        public DistributedCacheService(ILogger<DistributedCacheService> logger,
            IDistributedCache cache,
            CachingSettings cachingSettings)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _cachingSettings = cachingSettings ?? throw new ArgumentNullException(nameof(cachingSettings));
            _options = new DistributedCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromSeconds(cachingSettings.SlidingExpiration))
                .SetAbsoluteExpiration(DateTime.UtcNow.AddSeconds(cachingSettings.AbsoluteExpiration));
        }

        /// <summary>
        /// Get item from cache
        /// </summary>
        /// <typeparam name="T">The type of item being retireved</typeparam>
        /// <param name="key">Key of the cache</param>
        /// <param name="cancellationToken">cancellation token</param>
        /// <returns>Item of the provided type</returns>
        public async Task<T?> GetFromCache<T>(string key, CancellationToken cancellationToken = default) where T : class
        {
            _logger.LogDebug("GetFromCache() | key[{0}]", key);
            if (cancellationToken.IsCancellationRequested) return null;
            var cached = await _cache.GetStringAsync(key, cancellationToken);
            return cached == null
                ? null
                : JsonSerializer.Deserialize<T>(cached, jsonSerializerOptions);
        }

        /// <summary>
        /// Sets an item into the cache
        /// </summary>
        /// <typeparam name="T">The type of the item being cached</typeparam>
        /// <param name="key">Key of the cache</param>
        /// <param name="value">Value that is being cached</param>
        /// <param name="cancellationToken">cancellation token</param>
        /// <returns>Item of type T</returns>
        public async Task SetCache<T>(string key, T value, CancellationToken cancellationToken = default)
        {
            _logger.LogDebug("SetCache() | key[{key}], value[{value}]", key, value);
            if (cancellationToken.IsCancellationRequested) return;
            var serialized = JsonSerializer.Serialize(value, jsonSerializerOptions);
            await _cache.SetStringAsync(key, serialized, _options, cancellationToken);
        }

        /// <summary>
        /// Clears a cache entry with the provided key
        /// </summary>
        /// <param name="key">Key of the cache</param>
        /// <param name="cancellationToken">cancellation token</param>
        /// <returns>void</returns>
        public async Task ClearCache(string key, CancellationToken cancellationToken = default)
        {
            _logger.LogDebug("ClearCache() | key[{key}]", key);
            if (cancellationToken.IsCancellationRequested) return;
            await _cache.RemoveAsync(key, cancellationToken);
        }
    }
}
