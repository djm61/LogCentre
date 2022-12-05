using LogCentre.ApiClient;
using LogCentre.Console.Models;
using LogCentre.Model.Log;

using Microsoft.Extensions.Configuration;
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

        //public Producer(ILoggerFactory loggerFactory, ChannelWriter<LineModel> channelWriter, IHttpClientFactory clientFactory, long hostId)
        public Producer(ILoggerFactory loggerFactory, ChannelWriter<LineModel> channelWriter, IHttpClientFactory clientFactory, HostModel hostModel)
        {
            //_logger = loggerFactory?.CreateLogger<Producer>() ?? throw new ArgumentNullException(nameof(loggerFactory));
            //_channelWriter = channelWriter ?? throw new ArgumentNullException(nameof(channelWriter));
            //var client = clientFactory.CreateClient("LogCentreApiClient");
            //if (client == null) { throw new ArgumentNullException(nameof(clientFactory)); }
            //_hostId = hostId > 0 ? hostId : throw new ArgumentNullException(nameof(hostId));

            //var clientLogger = loggerFactory.CreateLogger<LogCentreApiClient>();
            //_client = new LogCentreApiClient(clientLogger, client);
            _logger = loggerFactory?.CreateLogger<Producer>() ?? throw new ArgumentNullException(nameof(loggerFactory));
            _channelWriter = channelWriter ?? throw new ArgumentNullException(nameof(channelWriter));
            var client = clientFactory.CreateClient("LogCentreApiClient");
            if (client == null) { throw new ArgumentNullException(nameof(client)); }
            _hostId = hostModel?.HostId ?? throw new ArgumentNullException(nameof(hostModel));

            var clientLogger = loggerFactory.CreateLogger<LogCentreApiClient>();
            _client = new LogCentreApiClient(clientLogger, client);
        }

        public async Task StartAsync()
        {
            _logger.LogDebug("StartAsync()");

            try
            {
                var logSources = await _client.GetLogSourcesByHostAsync(_hostId);
                var task = Task.Factory.StartNew(async () =>
                {
                    foreach (var logSource in logSources)
                    {
                        try
                        {
                            var fileModels = await _client.GetFilesByLogSourceIdAsync(logSource.Id);
                            var directory = new DirectoryInfo(logSource.Path);
                            //todo sort by oldest first
                            foreach (var file in directory.GetFiles().OrderBy(x => x.LastWriteTime))
                            {
                                var fileModel = fileModels.FirstOrDefault(x => x.Name == file.Name);
                                if (fileModel == null)
                                {
                                    //we don't have the file, read it!
                                    fileModel = new FileModel();
                                    fileModel.LogSourceId = logSource.Id;
                                    fileModel.Name = file.Name;
                                    fileModel.LastUpdatedBy = $"Producer-{logSource.HostId}";
                                    await _client.CreateLogFileAsync(fileModel);
                                }

                                //todo check if file is already fully read, if yes, skip file, if no, find last read line and then read from there (line count maybe?)
                                var lines = File.ReadAllLines(file.FullName);
                                var lineCount = await _client.GetLogLineCountByFileIdAsync(fileModel.Id);
                                if (lines.Length == lineCount)
                                {
                                    continue;
                                }

                                foreach (var line in lines)
                                {
                                    var logLine = new LineModel();
                                    logLine.FileId = fileModel.Id;
                                    logLine.LogLine = line;
                                    logLine.LastUpdatedBy = $"Producer-{logSource.HostId}";
                                    await _channelWriter.WriteAsync(logLine);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError($"Error [{ex}]", ex);
                        }
                    }
                });

                await task.WaitAsync(new CancellationToken());
            }
            catch (Exception ex)
            {
                _logger.LogError("Error reading files", ex);
            }
        }
    }
}
