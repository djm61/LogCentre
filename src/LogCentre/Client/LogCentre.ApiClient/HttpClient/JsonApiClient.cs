using Microsoft.Extensions.Logging;

using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace LogCentre.ApiClient.HttpClient
{
    public class JsonApiClient<T> where T : class
    {
        protected const string JsonContentType = "applicaiton/json";

        protected readonly ILogger<T> Logger;
        protected readonly System.Net.Http.HttpClient HttpClient;

        protected JsonSerializerOptions JsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowReadingFromString,
        };

        public JsonApiClient(ILogger<T> logger, System.Net.Http.HttpClient httpClient)
        {
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            HttpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            HttpClient.DefaultRequestHeaders.Add("User-Agent", ClientLiterals.ApiClientName);
            HttpClient.DefaultRequestHeaders.Accept.Clear();
            HttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpClient.DefaultRequestHeaders.AcceptLanguage.Clear();
            HttpClient.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue("en-US"));
        }

        public JsonApiClient(ILogger<T> logger, IHttpClientFactory clientFactory, string clientName = "")
        {
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            var cf = clientFactory ?? throw new ArgumentNullException(nameof(clientFactory));
            HttpClient = string.IsNullOrWhiteSpace(clientName)
                ? cf.CreateClient()
                : cf.CreateClient(clientName);

            HttpClient.DefaultRequestHeaders.Add("User-Agent", ClientLiterals.ApiClientName);
            HttpClient.DefaultRequestHeaders.Accept.Clear();
            HttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(JsonContentType));
            HttpClient.DefaultRequestHeaders.AcceptLanguage.Clear();
            HttpClient.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue("en-US"));
        }

        protected async Task<T> GetAsync<T>(string uri, CancellationToken cancellationToken = default)
        {
            Logger.LogDebug("GetAsync() | uri[{uri}]", uri);
            var response = await HttpClient.GetAsync(uri, cancellationToken);
            response.EnsureSuccessStatusCode();

            await using var responseStream = await response.Content.ReadAsStreamAsync(cancellationToken);
            return await JsonSerializer.DeserializeAsync<T>(responseStream, JsonSerializerOptions, cancellationToken);
        }

        protected async Task<T> PostAsync<T>(string uri, T data, CancellationToken cancellationToken = default)
        {
            Logger.LogDebug("PostAsync() | uri[{uri}], data[{data}]", uri, data);
            var response = await HttpClient.PostAsJsonAsync(uri, data, JsonSerializerOptions, cancellationToken);
            response.EnsureSuccessStatusCode();

            await using var responseStream = await response.Content.ReadAsStreamAsync(cancellationToken);
            return await JsonSerializer.DeserializeAsync<T>(responseStream, JsonSerializerOptions, cancellationToken);
        }

        protected async Task PutAsync<T>(string uri, T data, CancellationToken cancellationToken = default)
        {
            Logger.LogDebug("PugAsync() | uri[{uri}], data[{data}]", uri, data);
            var json = JsonSerializer.Serialize(data, JsonSerializerOptions);
            var dataItem = new StringContent(json, Encoding.UTF8, "application/json");
            //var response = await HttpClient.PutAsJsonAsync(uri, data, JsonSerializerOptions, cancellationToken);
            var response = await HttpClient.PutAsync(uri, dataItem, cancellationToken);
            response.EnsureSuccessStatusCode();
        }

        protected async Task DeleteAsync(string uri, CancellationToken cancellationToken = default)
        {
            Logger.LogDebug("DeleteAsync() | uri[{uri}]", uri);
            var response = await HttpClient.DeleteAsync(uri, cancellationToken);
            response.EnsureSuccessStatusCode();
        }
    }
}
