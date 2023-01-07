using LogCentre.ApiClient;
using LogCentre.Console.Models;
using LogCentre.Model;
using LogCentre.Model.Log;

using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.Extensions.Logging;

using System.IO;
using System.Runtime.CompilerServices;
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

        private LogSourceModel _logSource;
        private IList<FileModel> _fileModels;
        private FileInfo _file;
        private Regex _regex;

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
                    foreach (var logSource in logSources)
                    {
                        _logSource = logSource;
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

                        _regex = new Regex(provider.Regex);
                        try
                        {
                            _fileModels = await _client.GetFilesByLogSourceIdAsync(logSource.Id);
                            var directory = new DirectoryInfo(logSource.Path);
                            foreach (var file in directory.GetFiles().OrderBy(x => x.LastWriteTime))
                            {
                                await ReadFileAsync(file);
                            }

                            //todo create a file system watcher for the last file to keep reading the file
                            var watcher = new FileSystemWatcher(logSource.Path)
                            {
                                IncludeSubdirectories = true,
                                NotifyFilter = NotifyFilters.Attributes |
                                NotifyFilters.CreationTime |
                                NotifyFilters.DirectoryName |
                                NotifyFilters.FileName |
                                NotifyFilters.LastAccess |
                                NotifyFilters.LastWrite |
                                NotifyFilters.Security |
                                NotifyFilters.Size
                            };

                            watcher.Changed += new FileSystemEventHandler(OnChanged);
                            watcher.Created += new FileSystemEventHandler(OnChanged);
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

        /// <summary>
        /// Reads the current file
        /// </summary>
        /// <param name="file">Current FileInfo file</param>
        /// <returns>void - nothing</returns>
        private async Task ReadFileAsync(FileInfo file)
        {
            _logger.LogDebug("ReadFileAsync() | file[{0}]", file);
            var fileModel = await GetOrCreateFileModelAsync(_fileModels);
            if (fileModel == null)
            {
                _logger.LogWarning("ReadFileAsync() | Producing files doesn't have a matching file, what?");
                return;
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
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                lines.Add(line.Trim());
            }

            var lineCount = await _client.GetLogLineCountByFileIdAsync(fileModel.Id);
            if (lines.Count == lineCount)
            {
                _logger.LogInformation("ReadFileAsync() | lineCount[{0}] matches stored lineCount[{1}]", lines.Count, lineCount);
                return;
            }

            await CreateLineAsync(lines, lineCount, fileModel);
            fileModel.FileComplete = ModelLiterals.Yes;
            await _client.UpdateLogFileAsync(fileModel);
        }

        private async Task<FileModel> GetOrCreateFileModelAsync(IList<FileModel> fileModels)
        {
            _logger.LogDebug("GetOrCreateFileModelAsync() | fileModels[{0}], logSource[{1}], fileFullName[{2}]", fileModels.Count, _logSource, _file.FullName);
            var fileModel = fileModels.FirstOrDefault(x => x.Name == _file.Name);
            if (fileModel == null)
            {
                //we don't have the file, read it!
                fileModel = new FileModel
                {
                    LogSourceId = _logSource.Id,
                    Name = _file.Name,
                    LastUpdatedBy = $"Producer-{_hostId}"
                };

                await _client.CreateLogFileAsync(fileModel);
                fileModels = await _client.GetFilesByLogSourceIdAsync(_logSource.Id);
                fileModel = fileModels.FirstOrDefault(x => x.Name == _file.Name);
                if (fileModel == null)
                {
                    _logger.LogError("Unable to read back the file that was just created, what?");
                    return null;
                }
            }

            return fileModel;
        }

        private async Task CreateLineAsync(IList<string> lines, long lineCount, FileModel fileModel)
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

                var matches = _regex.Match(line);
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

        private void OnChanged(object source, FileSystemEventArgs e)
        {
            _logger.LogDebug("OnChanged() | filePath[{0}]", e.FullPath);
            var fileInfo = new FileInfo(e.FullPath);
            ReadFileAsync(fileInfo).GetAwaiter().GetResult();
        }
    }
}
