using Microsoft.Extensions.Logging;

using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace LogCentre.ApiClient.HttpClient
{
    public class JsonApiClient<T> where T : class
    {
        private const string JsonContentType = "applicaiton/json";

        protected ILogger<T> Logger;
        protected System.Net.Http.HttpClient _httpClient;

        protected JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowReadingFromString,
        };

        public JsonApiClient(ILogger<T> logger, System.Net.Http.HttpClient httpClient)
        {
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "LogCentreApiClient");
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        protected async Task<T> GetAsync<T>(string uri, CancellationToken cancellationToken = default)
        {
            Logger.LogDebug("GetAsync() | uri[{uri}]", uri);
            try
            {
                var response = await _httpClient.GetAsync(uri, cancellationToken);
                response.EnsureSuccessStatusCode();

                await using var responseStream = await response.Content.ReadAsStreamAsync(cancellationToken);
                return await JsonSerializer.DeserializeAsync<T>(responseStream, jsonSerializerOptions, cancellationToken);
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error [{ex}]", ex);
                throw;
            }
        }

        protected async Task<T> PostAsync<T>(string uri, T data, CancellationToken cancellationToken = default)
        {
            Logger.LogDebug("PostAsync() | uri[{uri}], data[{data}]", uri, data);
            var json = JsonSerializer.Serialize(data);
            var dataItem = new StringContent(json, Encoding.UTF8, JsonContentType);
            var response = await _httpClient.PostAsJsonAsync(uri, data, jsonSerializerOptions, cancellationToken);
            //var response = await _httpClient.PostAsync(uri, dataItem, cancellationToken);
            response.EnsureSuccessStatusCode();

            var asdf1 = await response.Content.ReadAsStringAsync();

            await using var responseStream = await response.Content.ReadAsStreamAsync(cancellationToken);
            return await JsonSerializer.DeserializeAsync<T>(responseStream, jsonSerializerOptions, cancellationToken);
        }

        protected async Task<T> PutAsync<T>(string uri, T data, CancellationToken cancellationToken = default)
        {
            Logger.LogDebug("PugAsync() | uri[{uri}], data[{data}]", uri, data);
            var json = JsonSerializer.Serialize(data);
            var dataItem = new StringContent(json, Encoding.UTF8, JsonContentType);
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
