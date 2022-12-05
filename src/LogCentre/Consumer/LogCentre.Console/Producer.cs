using LogCentre.ApiClient;
using LogCentre.Model.Log;

using Microsoft.Extensions.Logging;

using System.Threading.Channels;

namespace LogCentre.Console
{
    internal class Producer
    {
        private readonly ILogger<Producer> _logger;
        private readonly ChannelWriter<LineModel> _channelWriter;
        private readonly ILogCentreApiClient _client;
        private readonly long _hostId;

        public Producer(ILoggerFactory loggerFactory, ChannelWriter<LineModel> channelWriter, HttpClient client, long hostId)
        {
            _logger = loggerFactory?.CreateLogger<Producer>() ?? throw new ArgumentNullException(nameof(loggerFactory));
            _channelWriter = channelWriter ?? throw new ArgumentNullException(nameof(channelWriter));
            if (client == null) { throw new ArgumentNullException(nameof(client)); }
            _hostId = hostId > 0 ? hostId : throw new ArgumentNullException(nameof(hostId));

            var clientLogger = loggerFactory.CreateLogger<LogCentreApiClient>();
            _client = new LogCentreApiClient(clientLogger, client);
        }

        public async Task StartAsync()
        {
            _logger.LogDebug("StartAsync()");

            try
            {
                var logSources = await _client.GetLogSourcesByHostAsync(_hostId);
                _ = Task.Factory.StartNew(async () =>
                {
                    foreach (var logSource in logSources)
                    {
                        //todo read files
                        var directory = new DirectoryInfo(logSource.Path);
                        //todo sort by oldest first
                        foreach (var file in directory.GetFiles())
                        {
                            //todo check if file is already fully read, if yes, skip file, if no, find last read line and then read from there (line count maybe?)
                            var lines = File.ReadAllLines(file.FullName);
                            var logFile = new FileModel();
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError("Error reading files", ex);
            }
        }
    }
}
