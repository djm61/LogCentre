using LogCentre.ApiClient;
using LogCentre.Console.Models;
using LogCentre.Model.Log;

using Microsoft.Extensions.Logging;

using System.Text.RegularExpressions;
using System.Threading.Channels;

namespace LogCentre.Console
{
    internal class Producer
    {
        private readonly ILogger<Producer> _logger;
        private readonly ChannelWriter<LineModel> _channelWriter;
        private readonly ILogCentreApiClient _client;
        private readonly long _hostId;

        public Producer(ILoggerFactory loggerFactory, ChannelWriter<LineModel> channelWriter, IHttpClientFactory clientFactory, HostModel hostModel)
        {
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
                    Regex regex;
                    foreach (var logSource in logSources)
                    {
                        var provider = logSource.Provider;
                        //todo error checking!
                        regex = new Regex(provider.Regex);
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
                                    fileModel = new FileModel
                                    {
                                        LogSourceId = logSource.Id,
                                        Name = file.Name,
                                        LastUpdatedBy = $"Producer-{_hostId}"
                                    };

                                    await _client.CreateLogFileAsync(fileModel);
                                    fileModels = await _client.GetFilesByLogSourceIdAsync(logSource.Id);
                                    fileModel = fileModels.FirstOrDefault(x => x.Name == file.Name);
                                    if (fileModel == null)
                                    {
                                        _logger.LogError("Unable to read back the file that was just created, what?");
                                        continue;
                                    }
                                }

                                //todo check if file is already fully read, if yes, skip file, if no, find last read line and then read from there (line count maybe?)
                                var lines = File.ReadAllLines(file.FullName);
                                var lineCount = await _client.GetLogLineCountByFileIdAsync(fileModel.Id);
                                if (lines.Length == lineCount)
                                {
                                    continue;
                                }

                                var grouping = Guid.NewGuid();
                                foreach (var line in lines)
                                {
                                    var matches = regex.Matches(line);
                                    var logLine = new LineModel
                                    {
                                        FileId = fileModel.Id,
                                        LogLine = line,
                                        Grouping = grouping,
                                        LastUpdatedBy = $"Producer-{_hostId}"
                                    };

                                    await _channelWriter.WriteAsync(logLine);
                                    if (matches.Count() > 0)
                                    {
                                        grouping = Guid.NewGuid();
                                    }
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
