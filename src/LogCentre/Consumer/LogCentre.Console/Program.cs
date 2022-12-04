using LogCentre.ApiClient;
using LogCentre.Console;
using LogCentre.Console.Config;
using LogCentre.Model.Log;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Serilog;

using System.Net.Http.Headers;
using System.Text;
using System.Threading.Channels;

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.Development.json", optional: true, reloadOnChange: true)
    .Build();

//var serviceProvider = new ServiceCollection()
//                  //.AddLogging(builder =>
//                  //{
//                  //    //var logger = new LoggerConfiguration()
//                  //    //              .MinimumLevel.Information()
//                  //    //              .WriteTo.File(path: "\\Logs", restrictedToMinimumLevel: LogEventLevel.Information, outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}",
//                  //    //              fileSizeLimitBytes: 1000000, rollOnFileSizeLimit: true, retainedFileCountLimit: 365, retainedFileTimeLimit: new TimeSpan(365, 0, 0, 0))
//                  //    //              .CreateLogger();
//                  //    //builder.AddSerilog(logger);
//                  //    builder.ClearProviders();
//                  //    builder.AddConfiguration(configuration.GetSection("Logging"));
//                  //    builder.AddSerilog(new LoggerConfiguration()
//                  //        .ReadFrom.Configuration(context.Configuration)
//                  //        .Enrich.WithThreadId()
//                  //        .CreateLogger());
//                  //})
//                  //this is important
//                  //.AddSingleton<Serilog.ILogger>(sp =>
//                  //{
//                  //    return new LoggerConfiguration()
//                  //        .MinimumLevel.Debug()
//                  //        .CreateLogger();
//                  //})
//                  .BuildServiceProvider();
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(configuration)
    .Enrich.WithThreadId()
    .CreateLogger();

var hostId = configuration.GetValue<long>("HostId");
if (hostId <= 0)
{
    throw new Exception("Invalid Host Id");
}

var apiConnection = configuration.GetSection("ApiConnectionSettings").Get<ApiConnectionSettings>();
if (apiConnection == null)
{
    throw new Exception("Missing Configuration Section: ApiConnectionSettings");
}

if (string.IsNullOrWhiteSpace(apiConnection.Host))
{
    throw new Exception("Mistting API Url");
}

if (string.IsNullOrWhiteSpace(apiConnection.BasicAuthUsername))
{
    throw new Exception("Missing Basic Auth Username");
}

if (string.IsNullOrWhiteSpace(apiConnection.BasicAuthPassword))
{
    throw new Exception("Missing Basic Auth Password");
}

var host = apiConnection.Host.EndsWith("/") ? apiConnection.Host : apiConnection.Host + "/";
var username = apiConnection.BasicAuthUsername;
var password = apiConnection.BasicAuthPassword;
var authToken = Encoding.ASCII.GetBytes($"{username}:{password}");
var client = new HttpClient();
client.BaseAddress = new Uri(host);
client.DefaultRequestHeaders.Accept.Clear();
client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(authToken));

var loggerFactory = new LoggerFactory();
var channels = Channel.CreateBounded<LineModel>(1000);
var producer = new Producer(loggerFactory, channels.Writer, client, hostId);
var consumer = new Consumer(loggerFactory, channels.Reader, client, hostId);

await producer.StartAsync();
await consumer.StartAsync();