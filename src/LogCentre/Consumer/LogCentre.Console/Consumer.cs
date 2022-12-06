using LogCentre.ApiClient;
using LogCentre.Console.Models;
using LogCentre.Model.Log;

using Microsoft.Extensions.Logging;

using System.Threading.Channels;

namespace LogCentre.Console
{
    internal class Consumer
    {
        private readonly ILogger<Consumer> _logger;
        private readonly ChannelReader<LineModel> _channelReader;
        private readonly ILogCentreApiClient _client;

        public Consumer(ILoggerFactory loggerFactory, ChannelReader<LineModel> channelReader, IHttpClientFactory clientFactory)
        {
            _logger = loggerFactory?.CreateLogger<Consumer>() ?? throw new ArgumentNullException(nameof(loggerFactory));
            _channelReader = channelReader ?? throw new ArgumentNullException(nameof(channelReader));
            var client = clientFactory.CreateClient("LogCentreApiClient");
            if (client == null) { throw new ArgumentNullException(nameof(client)); }

            var clientLogger = loggerFactory.CreateLogger<LogCentreApiClient>();
            _client = new LogCentreApiClient(clientLogger, client);
        }

        public async Task StartAsync()
        {
            await foreach (var item in _channelReader.ReadAllAsync())
            {
                try
                {
                    _logger.LogDebug("StartAsync() | writing line");
                    await _client.CreateLogLineAsync(item);
                    await Task.Delay(1000);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error writing [{ex}]", ex);
                }
            }
        }
    }
}
