﻿using LogCentre.ApiClient;
using LogCentre.Console.Models;
using LogCentre.Model;
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

        public Producer(ILoggerFactory loggerFactory,
            ChannelWriter<LineModel> channelWriter,
            IHttpClientFactory clientFactory,
            HostIdModel hostIdModel)
        {
            _logger = loggerFactory?.CreateLogger<Producer>() ?? throw new ArgumentNullException(nameof(loggerFactory));
            _channelWriter = channelWriter ?? throw new ArgumentNullException(nameof(channelWriter));
            _hostId = hostIdModel?.HostId ?? throw new ArgumentNullException(nameof(hostIdModel));

            var clientLogger = loggerFactory.CreateLogger<LogCentreApiClient>();
            _client = new LogCentreApiClient(clientLogger, clientFactory, ClientLiterals.ApiClientName);
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
                        if (provider == null)
                        {
                            _logger.LogWarning("StartAsync() thread | logSource[{0}] doesn't have a provider", logSource.Id);
                            continue;
                        }

                        if (string.IsNullOrWhiteSpace(provider.Regex))
                        {
                            _logger.LogWarning("StartAsync() thread | provider[{0}] doesn't have a regex", provider.Id);
                            continue;
                        }

                        regex = new Regex(provider.Regex);
                        try
                        {
                            var fileModels = await _client.GetFilesByLogSourceIdAsync(logSource.Id);
                            var directory = new DirectoryInfo(logSource.Path);
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

                            //todo create a file system watcher for the last file to keep reading the file
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
            var date = DateTime.UtcNow;
            var level = string.Empty;
            var thread = string.Empty;
            var source = string.Empty;
            var logLine = string.Empty;
            foreach (var line in lines)
            {
                if (lineCount > 0 && lineCount < lines.Count && currentRow < lineCount)
                {
                    currentRow++;
                    continue;
                }

                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                var matches = regex.Match(line);
                if (matches.Success)
                {
                    grouping = Guid.NewGuid();
                    _ = DateTime.TryParse(matches.Groups["Date"].Value, out date);
                    level = matches.Groups["Level"]?.Value?.Trim() ?? string.Empty;
                    thread = matches.Groups["Thread"]?.Value?.Trim() ?? string.Empty;
                    source = matches.Groups["Source"]?.Value?.Trim() ?? string.Empty;
                    logLine = matches.Groups["Text"]?.Value?.Trim() ?? line;
                }
                else
                {
                    logLine = line;
                }

                var logLineModel = new LineModel
                {
                    FileId = fileModel.Id,
                    LogDate = date,
                    Level = level,
                    Thread = thread,
                    Source = source,
                    LogLine = logLine,
                    FullLine = line,
                    Grouping = grouping,
                    LastUpdatedBy = $"Producer-{_hostId}"
                };

                await _channelWriter.WriteAsync(logLineModel);
            }
        }
    }
}
