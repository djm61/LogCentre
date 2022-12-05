using LogCentre.Console;
using LogCentre.Console.Config;
using LogCentre.Console.Models;
using LogCentre.Model.Log;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Serilog;

using System.Net.Http.Headers;
using System.Text;
using System.Threading.Channels;

var services = new ServiceCollection();
ConfigureServices(services);
var serviceProvider = services.BuildServiceProvider();

var producer = serviceProvider.GetService<Producer>();
var consumer = serviceProvider.GetService<Consumer>();

if (producer == null)
{
    throw new Exception("Unable to find: Producer");
}

if (consumer == null)
{
    throw new Exception("Unable to find: Consumer");
}

await producer.StartAsync();
await consumer.StartAsync();

Console.ReadKey();

void ConfigureServices(IServiceCollection serviceCollection)
{
    var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true)
                .Build();
    serviceCollection.AddSingleton(configuration);
    serviceCollection.AddOptions();

    var serilogLogger = new LoggerConfiguration()
        .ReadFrom.Configuration(configuration)
        .Enrich.WithThreadId()
        .CreateLogger();

    serviceCollection
                .AddLogging(configure =>
                {
                    configure.ClearProviders();
                    configure.AddConfiguration(configuration.GetSection("Logging"));
                    configure.AddSerilog(logger: serilogLogger, dispose: true);
                });

    AddThreadingChannels(serviceCollection);
    AddHostModel(serviceCollection, configuration);
    AddHttpClientFactory(serviceCollection, configuration);

    serviceCollection.AddTransient<Producer>();
    serviceCollection.AddTransient<Consumer>();
}

void AddThreadingChannels(IServiceCollection serviceCollection)
{
    var channels = Channel.CreateBounded<LineModel>(1000);
    var reader = channels.Reader;
    var writer = channels.Writer;
    serviceCollection.AddSingleton(reader);
    serviceCollection.AddSingleton(writer);
}

void AddHostModel(IServiceCollection serviceCollection, IConfiguration configuration)
{
    var hostModel = configuration.GetSection("Host").Get<HostModel>();
    if (hostModel == null)
    {
        throw new Exception("Missing Configuration Section: Host");
    }

    serviceCollection.AddSingleton(hostModel);
}

void AddHttpClientFactory(IServiceCollection serviceCollection, IConfiguration configuration)
{
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

    serviceCollection.AddHttpClient("LogCentreApiClient", client =>
    {
        client.BaseAddress = new Uri(host);
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(authToken));
        client.DefaultRequestHeaders.AcceptLanguage.Clear();
        client.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue("en-US"));
    });
}