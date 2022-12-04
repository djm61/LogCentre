using LogCentre.Model;
using LogCentre.Model.Log;

namespace LogCentre.ApiClient
{
    public interface ILogCentreApiClient
    {
        #region Host

        Task<IList<HostModel>> GetHostsAsync(CancellationToken cancellationToken = default);
        Task<HostModel> GetHostByIdAsync(long id, CancellationToken cancellationToken = default);
        Task<HostModel> CreateHostAsync(HostModel host, CancellationToken cancellationToken = default);
        Task<HostModel> UpdateHostAsync(HostModel host, CancellationToken cancellationToken = default);
        Task DeleteHostAsync(long id, CancellationToken cancellationToken = default);
        Task PurgeHostAsync(long id, CancellationToken cancellationToken = default);

        #endregion

        #region Provider

        Task<IList<ProviderModel>> GetProvidersAsync(CancellationToken cancellationToken = default);
        Task<ProviderModel> GetProviderByIdAsync(long id, CancellationToken cancellationToken = default);
        Task<ProviderModel> CreateProviderAsync(ProviderModel provider, CancellationToken cancellationToken = default);
        Task<ProviderModel> UpdateProviderAsync(ProviderModel provider, CancellationToken cancellationToken = default);
        Task DeleteProviderAsync(long id, CancellationToken cancellationToken = default);
        Task PurgeProviderAsync(long id, CancellationToken cancellationToken = default);

        #endregion

        #region Log Source

        Task<IList<LogSourceModel>> GetLogSourcesAsync(CancellationToken cancellationToken = default);
        Task<LogSourceModel> GetLogSourceByIdAsync(long id, CancellationToken cancellationToken = default);
        Task<IList<LogSourceModel>> GetLogSourcesByHostAsync(long id, CancellationToken cancellationToken = default);
        Task<LogSourceModel> CreateLogSourceAsync(LogSourceModel logSource, CancellationToken cancellationToken = default);
        Task<LogSourceModel> UpdateLogSourceAsync(LogSourceModel logSource, CancellationToken cancellationToken = default);
        Task DeleteLogSourceAsync(long id, CancellationToken cancellationToken = default);
        Task PurgeLogSourceAsync(long id, CancellationToken cancellationToken = default);

        #endregion

        #region Log File

        Task<IList<FileModel>> GetLogFilesAsync(CancellationToken cancellationToken = default);
        Task<FileModel> GetLogFileByIdAsync(long id, CancellationToken cancellationToken = default);
        Task<FileModel> CreateLogFileAsync(FileModel provider, CancellationToken cancellationToken = default);
        Task<FileModel> UpdateLogFileAsync(FileModel provider, CancellationToken cancellationToken = default);
        Task DeleteLogFileAsync(long id, CancellationToken cancellationToken = default);
        Task PurgeLogFileAsync(long id, CancellationToken cancellationToken = default);

        #endregion

        #region Log Line

        Task<IList<LineModel>> GetLogLinesAsync(CancellationToken cancellationToken = default);
        Task<LineModel> GetLogLineByIdAsync(long id, CancellationToken cancellationToken = default);
        Task<LineModel> CreateLogLineAsync(LineModel provider, CancellationToken cancellationToken = default);
        Task<LineModel> UpdateLogLineAsync(LineModel provider, CancellationToken cancellationToken = default);
        Task DeleteLogLineAsync(long id, CancellationToken cancellationToken = default);
        Task PurgeLogLineAsync(long id, CancellationToken cancellationToken = default);

        #endregion
    }
}
