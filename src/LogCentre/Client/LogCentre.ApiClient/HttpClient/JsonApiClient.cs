using Microsoft.Extensions.Logging;

using System.Net.Http.Json;
using System.Text.Json;

namespace LogCentre.ApiClient.HttpClient
{
    public class JsonApiClient<T> where T : class
    {
        protected ILogger<T> Logger;
        protected System.Net.Http.HttpClient _httpClient;

        protected JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
        };

        public JsonApiClient(ILogger<T> logger, System.Net.Http.HttpClient httpClient)
        {
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        protected async Task<T> GetAsync<T>(string uri, CancellationToken cancellationToken = default)
        {
            Logger.LogDebug("GetAsync() | uri[{uri}]", uri);
            var response = await _httpClient.GetAsync(uri, cancellationToken);
            response.EnsureSuccessStatusCode();

            await using var responseStream = await response.Content.ReadAsStreamAsync(cancellationToken);
            return await JsonSerializer.DeserializeAsync<T>(responseStream, jsonSerializerOptions, cancellationToken);
        }

        protected async Task<T> PostAsync<T>(string uri, T data, CancellationToken cancellationToken = default)
        {
            Logger.LogDebug("PostAsync() | uri[{uri}], data[{data}]", uri, data);
            var json = JsonSerializer.Serialize(data);
            var dataItem = new StringContent(json);
            var response = await _httpClient.PostAsync(uri, dataItem, cancellationToken);
            response.EnsureSuccessStatusCode();

            await using var responseStream = await response.Content.ReadAsStreamAsync(cancellationToken);
            return await JsonSerializer.DeserializeAsync<T>(responseStream, jsonSerializerOptions, cancellationToken);
        }

        protected async Task<T> PutAsync<T>(string uri, T data, CancellationToken cancellationToken = default)
        {
            Logger.LogDebug("PugAsync() | uri[{uri}], data[{data}]", uri, data);
            var json = JsonSerializer.Serialize(data);
            var dataItem = new StringContent(json);
            var response = await _httpClient.PutAsync(uri, dataItem, cancellationToken);

            await using var responseStream = await response.Content.ReadAsStreamAsync(cancellationToken);
            return await JsonSerializer.DeserializeAsync<T>(responseStream, jsonSerializerOptions, cancellationToken);
        }

        protected async Task DeleteAsync(string uri, CancellationToken cancellationToken = default)
        {
            Logger.LogDebug("DeleteAsync() | uri[{uri}]", uri);
            var response = await _httpClient.DeleteAsync(uri, cancellationToken);
            response.EnsureSuccessStatusCode();
        }
    }
}
