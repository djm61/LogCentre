using LogCentre.Api;
using LogCentre.Api.Models;
using LogCentre.Data;
using LogCentre.Data.Interfaces;
using LogCentre.SeedData;
using LogCentre.Services.Interfaces;
using LogCentre.Services.Interfaces.Log;
using LogCentre.Services.Services;
using LogCentre.Services.Services.Log;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

using Serilog;

using System.Reflection;
using System.Runtime;

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

// Add services to the container.
builder.Services.AddTransient<IHostService, HostService>();
builder.Services.AddTransient<IProviderService, ProviderService>();
builder.Services.AddTransient<ILogSourceService, LogSourceService>();
builder.Services.AddTransient<ILineService, LineService>();

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
    //s.SwaggerDoc(swaggerSettings.Version, apiInfo);
    if (File.Exists(xmlPath))
    {
        s.IncludeXmlComments(xmlPath);
    }
});

var connectionString = builder.Configuration.GetConnectionString("ConnectionString");
if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new ApplicationException("Invalid SQL Connection String");
}
builder.Services.AddDbContext<ILogCentreDbContext, LogCentreDbContext>(options =>
    options
        .UseLoggerFactory(new LoggerFactory())
        .UseSqlServer(connectionString)
        .EnableDetailedErrors()
        .EnableSensitiveDataLogging()
);

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
});
builder.Services.AddHttpClient();

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

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

InitializeDatabase(app);

app.Run();

void InitializeDatabase(IApplicationBuilder app)
{
    using var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope();
    var context = serviceScope.ServiceProvider.GetRequiredService<LogCentreDbContext>();
    context.Database.EnsureCreated();
    //var script = context.Database.GenerateCreateScript();
    context.Database.Migrate();

    DataInjector.Initialize(context);
}