using LogCentre.Model;

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
    }
}
