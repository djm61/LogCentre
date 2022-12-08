﻿using LogCentre.ApiClient.HttpClient;
using LogCentre.Model;
using LogCentre.Model.Cache;
using LogCentre.Model.Log;

using Microsoft.Extensions.Logging;

using System.Text.Json;
using System.Text;

namespace LogCentre.ApiClient
{
    public class LogCentreApiClient : JsonApiClient<LogCentreApiClient>, ILogCentreApiClient
    {
        //public LogCentreApiClient(ILogger<LogCentreApiClient> logger, System.Net.Http.HttpClient httpClient)
        //    : base(logger, httpClient)
        //{
        //}

        public LogCentreApiClient(ILogger<LogCentreApiClient> logger, IHttpClientFactory clientFactory, string clientName = "LogCentreApiClient")
            : base(logger, clientFactory, clientName)
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

        public async Task UpdateHostAsync(HostModel host, CancellationToken cancellationToken = default)
        {
            Logger.LogDebug("UpdateHostAsync() | host[{host}]", host);
            var uri = "host";
            await PutAsync(uri, host, cancellationToken);
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

        public async Task UpdateProviderAsync(ProviderModel provider, CancellationToken cancellationToken = default)
        {
            Logger.LogDebug("UpdateProviderAsync() | provider[{provider}]", provider);
            var uri = "provider";
            await PutAsync(uri, provider, cancellationToken);
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

        public async Task UpdateLogSourceAsync(LogSourceModel logSource, CancellationToken cancellationToken = default)
        {
            Logger.LogDebug("UpdateLogSourceAsync() | logFile[{logFile}]", logSource);
            var uri = "logsource";
            await PutAsync(uri, logSource, cancellationToken);
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

        public async Task<IList<FileModel>> GetFilesByLogSourceIdAsync(long id, CancellationToken cancellationToken = default)
        {
            Logger.LogDebug("GetFileByLogSourceIdAsync() | id[{id}]", id);
            var uri = $"logfile/logsource/{id}";
            var response = await GetAsync<IList<FileModel>>(uri, cancellationToken);
            return response;
        }

        public async Task<FileModel> CreateLogFileAsync(FileModel logFile, CancellationToken cancellationToken = default)
        {
            Logger.LogDebug("CreateLogFileAsync() | logFile[{logFile}]", logFile);
            var uri = "logfile";
            var response = await PostAsync(uri, logFile, cancellationToken);
            return response;
        }

        public async Task UpdateLogFileAsync(FileModel logFile, CancellationToken cancellationToken = default)
        {
            Logger.LogDebug("UpdateLogFileAsync() | logFile[{logFile}]", logFile);
            var uri = "logfile";
            await PutAsync(uri, logFile, cancellationToken);
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

        #region Log Line

        public async Task<IList<LineModel>> GetLogLinesAsync(CancellationToken cancellationToken = default)
        {
            Logger.LogDebug("GetLogLinesAsync()");
            var uri = "logline/all";
            var response = await GetAsync<IList<LineModel>>(uri, cancellationToken);
            return response;
        }

        public async Task<LineModel> GetLogLineByIdAsync(long id, CancellationToken cancellationToken = default)
        {
            Logger.LogDebug("GetLogLineByIdAsync() | id[{id}]", id);
            var uri = $"logline/{id}";
            var response = await GetAsync<LineModel>(uri, cancellationToken);
            return response;
        }

        public async Task<long> GetLogLineCountByFileIdAsync(long fileId, CancellationToken cancellationToken = default)
        {
            Logger.LogDebug("GetLogLineCountByFileIdAsync() | fileId[{fileId}]", fileId);
            var uri = $"logline/file/{fileId}/count";
            var response = await GetAsync<long>(uri, cancellationToken);
            return response;
        }

        public async Task<LineModel> CreateLogLineAsync(LineModel logFile, CancellationToken cancellationToken = default)
        {
            Logger.LogDebug("CreateLogLineAsync() | logFile[{logFile}]", logFile);
            var uri = "logline";
            var response = await PostAsync(uri, logFile, cancellationToken);
            return response;
        }

        public async Task UpdateLogLineAsync(LineModel logFile, CancellationToken cancellationToken = default)
        {
            Logger.LogDebug("UpdateLogLineAsync() | logFile[{logFile}]", logFile);
            var uri = "logline";
            await PutAsync(uri, logFile, cancellationToken);
        }

        public async Task DeleteLogLineAsync(long id, CancellationToken cancellationToken = default)
        {
            Logger.LogDebug("DeleteLogLineAsync() | id[{id}]", id);
            var uri = $"logline/{id}";
            await DeleteAsync(uri, cancellationToken);
        }

        public async Task PurgeLogLineAsync(long id, CancellationToken cancellationToken = default)
        {
            Logger.LogDebug("PurgeLogLineAsync() | id[{id}]", id);
            var uri = $"logline/{id}/purge";
            await DeleteAsync(uri, cancellationToken);
        }

        #endregion

        #region Cache Searching

        public async Task<IList<CacheItemModel>> GetItensForSearchingAsync(string searchText, CancellationToken cancellationToken = default)
        {
            Logger.LogDebug("GetItemsForSearchingAsync() | searchText[{searchText}]", searchText);
            var json = JsonSerializer.Serialize(searchText, jsonSerializerOptions);
            var dataItem = new StringContent(json, Encoding.UTF8, "application/json");
            var uri = $"cachesearch/{dataItem}";
            var response = await GetAsync<IList<CacheItemModel>>(uri, cancellationToken);
            return response;
        }

        #endregion
    }
}