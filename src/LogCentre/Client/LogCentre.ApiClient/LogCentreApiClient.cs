using LogCentre.ApiClient.HttpClient;
using LogCentre.Model;
using LogCentre.Model.Log;

using Microsoft.Extensions.Logging;

namespace LogCentre.ApiClient
{
    public class LogCentreApiClient : JsonApiClient<LogCentreApiClient>, ILogCentreApiClient
    {
        public LogCentreApiClient(ILogger<LogCentreApiClient> logger, System.Net.Http.HttpClient httpClient)
            : base(logger, httpClient)
        {
        }

        #region Host

        /// <summary>
        /// Returns all <see cref="HostModel">hosts</see>
        /// </summary>
        /// <param name="cancellationToken">Default</param>
        /// <returns>IList of <see cref="HostModel">Hosts</see></returns>
        public async Task<IList<HostModel>> GetHostsAsync(CancellationToken cancellationToken = default)
        {
            Logger.LogDebug("GetHostsAsync()");
            var uri = "host/all";
            var response = await GetAsync<IList<HostModel>>(uri, cancellationToken);
            return response;
        }

        public async Task<HostModel> GetHostByIdAsync(long id, CancellationToken cancellationToken = default)
        {
            Logger.LogDebug("GetHostByIdAsync() | id[{id}]", id);
            var uri = $"host/{id}";
            var response = await GetAsync<HostModel>(uri, cancellationToken);
            return response;
        }

        public async Task<HostModel> CreateHostAsync(HostModel host, CancellationToken cancellationToken = default)
        {
            Logger.LogDebug("CreateHostAsync() | host[{host}]", host);
            var uri = "host";
            var response = await PostAsync(uri, host, cancellationToken);
            return response;
        }

        public async Task<HostModel> UpdateHostAsync(HostModel host, CancellationToken cancellationToken = default)
        {
            Logger.LogDebug("UpdateHostAsync() | host[{host}]", host);
            var uri = "host";
            var response = await PutAsync(uri, host, cancellationToken);
            return response;
        }

        public async Task DeleteHostAsync(long id, CancellationToken cancellationToken = default)
        {
            Logger.LogDebug("DeleteHostAsync() | id[{id}]", id);
            var uri = $"host/{id}";
            await DeleteAsync(uri, cancellationToken);
        }

        public async Task PurgeHostAsync(long id, CancellationToken cancellationToken = default)
        {
            Logger.LogDebug("PurgeHostAsync() | id[{id}]", id);
            var uri = $"host/{id}/purge";
            await DeleteAsync(uri, cancellationToken);
        }

        #endregion

        #region Provider

        public async Task<IList<ProviderModel>> GetProvidersAsync(CancellationToken cancellationToken = default)
        {
            Logger.LogDebug("GetProvidersAsync()");
            var uri = "provider/all";
            var response = await GetAsync<IList<ProviderModel>>(uri, cancellationToken);
            return response;
        }

        public async Task<ProviderModel> GetProviderByIdAsync(long id, CancellationToken cancellationToken = default)
        {
            Logger.LogDebug("GetProviderByIdAsync() | id[{id}]", id);
            var uri = $"provider/{id}";
            var response = await GetAsync<ProviderModel>(uri, cancellationToken);
            return response;
        }

        public async Task<ProviderModel> CreateProviderAsync(ProviderModel provider, CancellationToken cancellationToken = default)
        {
            Logger.LogDebug("CreateProviderAsync() | provider[{provider}]", provider);
            var uri = "provider";
            var response = await PostAsync(uri, provider, cancellationToken);
            return response;
        }

        public async Task<ProviderModel> UpdateProviderAsync(ProviderModel provider, CancellationToken cancellationToken = default)
        {
            Logger.LogDebug("UpdateProviderAsync() | provider[{provider}]", provider);
            var uri = "provider";
            var response = await PutAsync(uri, provider, cancellationToken);
            return response;
        }

        public async Task DeleteProviderAsync(long id, CancellationToken cancellationToken = default)
        {
            Logger.LogDebug("DeleteProviderAsync() | id[{id}]", id);
            var uri = $"provider/{id}";
            await DeleteAsync(uri, cancellationToken);
        }

        public async Task PurgeProviderAsync(long id, CancellationToken cancellationToken = default)
        {
            Logger.LogDebug("PurgeProviderAsync() | id[{id}]", id);
            var uri = $"provider/{id}/purge";
            await DeleteAsync(uri, cancellationToken);
        }

        #endregion

        #region Log Source

        public async Task<IList<LogSourceModel>> GetLogSourcesAsync(CancellationToken cancellationToken = default)
        {
            Logger.LogDebug("GetLogSourcesAsync()");
            var uri = "logsource/all";
            var response = await GetAsync<IList<LogSourceModel>>(uri, cancellationToken);
            return response;
        }

        public async Task<LogSourceModel> GetLogSourceByIdAsync(long id, CancellationToken cancellationToken = default)
        {
            Logger.LogDebug("GetLogSourceByIdAsync() | id[{id}]", id);
            var uri = $"logsource/{id}";
            var response = await GetAsync<LogSourceModel>(uri, cancellationToken);
            return response;
        }

        public async Task<IList<LogSourceModel>> GetLogSourcesByHostAsync(long id, CancellationToken cancellationToken = default)
        {
            Logger.LogDebug("GetLogSourcesByHostAsync() | id[{id}]", id);
            var uri = $"logsource/host/{id}";
            var response = await GetAsync<IList<LogSourceModel>>(uri, cancellationToken);
            return response;
        }

        public async Task<LogSourceModel> CreateLogSourceAsync(LogSourceModel logSource, CancellationToken cancellationToken = default)
        {
            Logger.LogDebug("CreateLogSourceAsync() | logFile[{logFile}]", logSource);
            var uri = "logsource";
            var response = await PostAsync(uri, logSource, cancellationToken);
            return response;
        }

        public async Task<LogSourceModel> UpdateLogSourceAsync(LogSourceModel logSource, CancellationToken cancellationToken = default)
        {
            Logger.LogDebug("UpdateLogSourceAsync() | logFile[{logFile}]", logSource);
            var uri = "logFile";
            var response = await PutAsync(uri, logSource, cancellationToken);
            return response;
        }

        public async Task DeleteLogSourceAsync(long id, CancellationToken cancellationToken = default)
        {
            Logger.LogDebug("DeleteLogSourceAsync() | id[{id}]", id);
            var uri = $"logsource/{id}";
            await DeleteAsync(uri, cancellationToken);
        }

        public async Task PurgeLogSourceAsync(long id, CancellationToken cancellationToken = default)
        {
            Logger.LogDebug("PurgeLogSourceAsync() | id[{id}]", id);
            var uri = $"logsource/{id}/purge";
            await DeleteAsync(uri, cancellationToken);
        }

        #endregion

        #region Log File

        public async Task<IList<FileModel>> GetLogFilesAsync(CancellationToken cancellationToken = default)
        {
            Logger.LogDebug("GetLogFilesAsync()");
            var uri = "logfile/all";
            var response = await GetAsync<IList<FileModel>>(uri, cancellationToken);
            return response;
        }

        public async Task<FileModel> GetLogFileByIdAsync(long id, CancellationToken cancellationToken = default)
        {
            Logger.LogDebug("GetLogFileByIdAsync() | id[{id}]", id);
            var uri = $"logfile/{id}";
            var response = await GetAsync<FileModel>(uri, cancellationToken);
            return response;
        }

        public async Task<FileModel> CreateLogFileAsync(FileModel logFile, CancellationToken cancellationToken = default)
        {
            Logger.LogDebug("CreateLogFileAsync() | logFile[{logFile}]", logFile);
            var uri = "logfile";
            var response = await PostAsync(uri, logFile, cancellationToken);
            return response;
        }

        public async Task<FileModel> UpdateLogFileAsync(FileModel logFile, CancellationToken cancellationToken = default)
        {
            Logger.LogDebug("UpdateLogFileAsync() | logFile[{logFile}]", logFile);
            var uri = "logfile";
            var response = await PutAsync(uri, logFile, cancellationToken);
            return response;
        }

        public async Task DeleteLogFileAsync(long id, CancellationToken cancellationToken = default)
        {
            Logger.LogDebug("DeleteLogFileAsync() | id[{id}]", id);
            var uri = $"logfile/{id}";
            await DeleteAsync(uri, cancellationToken);
        }

        public async Task PurgeLogFileAsync(long id, CancellationToken cancellationToken = default)
        {
            Logger.LogDebug("PurgeLogFileAsync() | id[{id}]", id);
            var uri = $"logfile/{id}/purge";
            await DeleteAsync(uri, cancellationToken);
        }

        #endregion
    }
}