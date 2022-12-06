using LogCentre.ApiClient;
using LogCentre.Console.Models;
using LogCentre.Model;
using LogCentre.Model.Log;

using Microsoft.Extensions.Logging;

using System.IO;
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

        public Producer(ILoggerFactory loggerFactory, ChannelWriter<LineModel> channelWriter, IHttpClientFactory clientFactory, HostIdModel hostIdModel)
        {
            _logger = loggerFactory?.CreateLogger<Producer>() ?? throw new ArgumentNullException(nameof(loggerFactory));
            _channelWriter = channelWriter ?? throw new ArgumentNullException(nameof(channelWriter));
            var client = clientFactory.CreateClient("LogCentreApiClient");
            if (client == null) { throw new ArgumentNullException(nameof(client)); }
            _hostId = hostIdModel?.HostId ?? throw new ArgumentNullException(nameof(hostIdModel));

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
                                var fileModel = await GetOrCreateFileModelAsync(fileModels, logSource, file);
                                if (fileModel == null)
                                {
                                    _logger.LogWarning("Producing files doesn't have a matching file, what?");
                                    continue;
                                }

                                //check if file is already fully read,
                                //if yes, skip file,
                                //if no, find last read line and then read from there (line count maybe?)
                                using var fs = new FileStream(file.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                                using var sr = new StreamReader(fs);
                                var lines = new List<string>();
                                while (!sr.EndOfStream)
                                {
                                    var line = sr.ReadLine();
                                    lines.Add(line);
                                }

                                var lineCount = await _client.GetLogLineCountByFileIdAsync(fileModel.Id);
                                if (lines.Count == lineCount)
                                {
                                    continue;
                                }

                                await CreateLineAsync(lines, lineCount, regex, fileModel);
                                fileModel.FileComplete = ModelLiterals.Yes;
                                await _client.UpdateLogFileAsync(fileModel);
                            }

                            //todo create a file system watcher
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

        private async Task<FileModel> GetOrCreateFileModelAsync(IList<FileModel> fileModels, LogSourceModel logSource, FileInfo file)
        {
            _logger.LogDebug("GetOrCreateFileModelAsync() | fileModels[{0}], logSource[{1}], fileFullName[{2}]", fileModels.Count, logSource, file.FullName);
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
                    return null;
                }
            }

            return fileModel;
        }

        private async Task CreateLineAsync(IList<string> lines, long lineCount, Regex regex, FileModel fileModel)
        {
            _logger.LogDebug("CreateLineAsync() | linesCount[{0}]", lines.Count);
            var currentRow = 0;
            var grouping = Guid.NewGuid();
            foreach (var line in lines)
            {
                if (lineCount > 0 && lineCount < lines.Count && currentRow < lineCount)
                {
                    currentRow++;
                    continue;
                }

                var matches = regex.Matches(line);
                if (matches.Count() > 0)
                {
                    grouping = Guid.NewGuid();
                }

                var logLine = new LineModel
                {
                    FileId = fileModel.Id,
                    LogLine = line,
                    Grouping = grouping,
                    LastUpdatedBy = $"Producer-{_hostId}"
                };

                await _channelWriter.WriteAsync(logLine);
            }
        }
    }
}
