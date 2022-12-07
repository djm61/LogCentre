using LogCentre.ApiClient;
using LogCentre.Web.Config;
using LogCentre.Web.Services;

using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection.Extensions;

using Serilog;

using System.Net.Http.Headers;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
IConfiguration configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .Build();

builder.Host.ConfigureLogging((context, logging) =>
{
    logging.ClearProviders();
    logging.AddConfiguration(context.Configuration.GetSection("Logging"));
    logging.AddSerilog(new LoggerConfiguration()
        .ReadFrom.Configuration(context.Configuration)
        .Enrich.WithThreadId()
        .CreateLogger());
});
builder.Services.AddLogging();

// Add services to the container.
AddHttpClientFactory(builder.Services, configuration);
AddRenderService(builder.Services);

builder.Services.AddControllersWithViews().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
});
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    //endpoints.MapHealthChecks("/health");

    endpoints.MapControllerRoute(
        name: "areas",
        pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");

    endpoints.MapRazorPages();
});

app.Run();

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
    //var client = new HttpClient();

    serviceCollection.AddHttpClient("LogCentreApiClient", client =>
    {
        client.BaseAddress = new Uri(host);
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(authToken));
        client.DefaultRequestHeaders.AcceptLanguage.Clear();
        client.DefaultRequestHeaders.AcceptLanguage.Add(new StringWithQualityHeaderValue("en-US"));
    });

    serviceCollection.AddScoped<ILogCentreApiClient, LogCentreApiClient>();
}

void AddRenderService(IServiceCollection serviceCollection)
{
    serviceCollection.AddHttpContextAccessor();
    serviceCollection.AddTransient<IActionContextAccessor, ActionContextAccessor>();
    serviceCollection.AddScoped<IRazorRenderService, RazorRenderService>();
}