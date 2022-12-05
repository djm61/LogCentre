using LogCentre.Data.Entities.Log;

namespace LogCentre.Services.Interfaces.Log
{
    public interface ILineService : IService<long, Line>
    {
        Task<long> GetLineCountForFileAsync(long fileId);
    }
}
