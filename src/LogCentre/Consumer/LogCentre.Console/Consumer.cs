using LogCentre.ApiClient;
using LogCentre.Model.Log;

using Microsoft.Extensions.Logging;

using System.Threading.Channels;

namespace LogCentre.Console
{
    internal class Consumer
    {
        private readonly ILogger<Producer> _logger;
        private readonly ChannelReader<LineModel> _channelReader;
        private readonly ILogCentreApiClient _client;
        private readonly long _hostId;

        public Consumer(ILoggerFactory loggerFactory, ChannelReader<LineModel> channelReader, HttpClient client, long hostId)
        {
            _logger = loggerFactory?.CreateLogger<Producer>() ?? throw new ArgumentNullException(nameof(loggerFactory));
            _channelReader = channelReader ?? throw new ArgumentNullException(nameof(channelReader));
            if (client == null) { throw new ArgumentNullException(nameof(client)); }
            _hostId = hostId > 0 ? hostId : throw new ArgumentNullException(nameof(hostId));

            var clientLogger = loggerFactory.CreateLogger<LogCentreApiClient>();
            _client = new LogCentreApiClient(clientLogger, client);
        }

        public async Task StartAsync()
        {

        }
    }
}
