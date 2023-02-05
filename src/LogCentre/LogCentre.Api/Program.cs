using LogCentre.Api;
using LogCentre.Api.Helpers;
using LogCentre.Api.Middleware;
using LogCentre.Api.Models;
using LogCentre.Api.Services;
using LogCentre.Data;
using LogCentre.SeedData;
using LogCentre.Services.Interfaces;
using LogCentre.Services.Interfaces.Log;
using LogCentre.Services.Services;
using LogCentre.Services.Services.Log;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

using OpenTelemetry;
using OpenTelemetry.Trace;

using Serilog;

using System.Reflection;

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

builder.Services.AddAutoMapper(typeof(LogCentreAutoMapperProfile));

//background settings
var backgroundServiceSettings = builder.Configuration.GetSection("BackgroundServiceSettings").Get<LogBackgroundServiceSettings>();
if (backgroundServiceSettings == null) throw new Exception("Missing Configuration Section: BackgroundServiceSettings");
builder.Services.AddSingleton(backgroundServiceSettings);

//caching settings
var cachingSettings = builder.Configuration.GetSection("CachingSettings").Get<CachingSettings>();
if (cachingSettings == null) throw new Exception("Missing Configuration Section: CachingSettings");
builder.Services.AddSingleton(cachingSettings);
builder.Services.AddDistributedCache(cachingSettings);

// Add services to the container.
AddEntityServices(builder.Services);
AddOpenTelemetry(builder.Services);
builder.Services.AddTransient<IDistributedCacheService, DistributedCacheService>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

var swaggerSettings = configuration.GetSection("SwaggerSettings").Get<SwaggerSettings>();
var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
builder.Services.AddSwaggerGen(s =>
{
    var basicAuthSecurityScheme = new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "basic",
        Reference = new OpenApiReference { Id = "BasicAuth", Type = ReferenceType.SecurityScheme }
    };
    s.AddSecurityDefinition(basicAuthSecurityScheme.Reference.Id, basicAuthSecurityScheme);
    s.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { basicAuthSecurityScheme, new string[] { } }
    });
    if (File.Exists(xmlPath))
    {
        s.IncludeXmlComments(xmlPath);
    }
});

AddLogCentreDbContext(builder);

//background service
builder.Services.AddSingleton<LogBackgroundService>();
builder.Services.AddHostedService<LogBackgroundService>();

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
});
builder.Services.AddHttpClient();
builder.Services.AddHttpContextAccessor();

//Versioning
var apiVersioningSettings = configuration.GetSection("ApiVersioningSettings").Get<ApiVersioningSettings>();
if (apiVersioningSettings == null)
{
    throw new ApplicationException("Invalid API Versioning Settings section");
}

builder.Services.AddApiVersioning(version =>
{
    version.ReportApiVersions = apiVersioningSettings.ReportApiVersionsValue;
    version.AssumeDefaultVersionWhenUnspecified = apiVersioningSettings.AssumeDefaultVersionWhenUnspecifiedValue;
    version.DefaultApiVersion =
        new ApiVersion(apiVersioningSettings.MajorVersionValue, apiVersioningSettings.MinorVersionValue);
});

//Authentication
var authConfig = configuration.GetSection("BasicAuthSettings");
if (authConfig == null)
{
    throw new ApplicationException("Invalid Auth Config section");
}

var basicAuthUsername = authConfig["BasicAuthUsername"];
var basicAuthPassword = authConfig["BasicAuthPassword"];
if (basicAuthUsername == null || basicAuthPassword == null)
{
    throw new ApplicationException("Invalid Auth Config Section - Username or Password");
}

builder.Services.AddAuthentication("BasicAuthentication")
    .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>
        ("BasicAuthentication", null);
builder.Services.AddAuthorization();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

//app.UseMiddleware<ApiKeyMiddleware>();
app.UseMiddleware<TraceIdMiddleware>();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

InitializeDatabase(app);

app.Run();

void AddEntityServices(IServiceCollection serviceCollection)
{
    serviceCollection.AddTransient<IHostService, HostService>();
    serviceCollection.AddTransient<IProviderService, ProviderService>();
    serviceCollection.AddTransient<ILogSourceService, LogSourceService>();
    serviceCollection.AddTransient<IFileService, FileService>();
    serviceCollection.AddTransient<ILineService, LineService>();
    serviceCollection.AddTransient<ISearchService, SearchService>();
}

void AddOpenTelemetry(IServiceCollection serviceCollection)
{
    serviceCollection.AddOpenTelemetry()
        .WithTracing(builder => builder
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddSqlClientInstrumentation()
            //.AddJaegerExporter()
            .AddConsoleExporter()
        )
        .StartWithHost();
}

void AddLogCentreDbContext(WebApplicationBuilder webApplicationBuilder)
{
    var connectionString = builder.Configuration.GetConnectionString("ConnectionString");
    if (string.IsNullOrWhiteSpace(connectionString)) throw new ApplicationException("Invalid SQL Connection String");
    webApplicationBuilder.Services.AddDbContextFactory<LogCentreDbContext>(optionsAction: options =>
        options
            .UseSqlServer(connectionString)
            .EnableDetailedErrors()
            .EnableSensitiveDataLogging()
            .UseLoggerFactory(new LoggerFactory())
    , lifetime: ServiceLifetime.Transient);
}

void InitializeDatabase(IApplicationBuilder app)
{
    using var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope();
    var contextFactory = serviceScope.ServiceProvider.GetRequiredService<IDbContextFactory<LogCentreDbContext>>();
    var context = contextFactory.CreateDbContext();
    context.Database.EnsureCreated();
    //var script = context.Database.GenerateCreateScript();
    context.Database.Migrate();

    DataInjector.Initialize(context);
}