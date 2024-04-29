using Autofac.Extensions.DependencyInjection;
using EmissionWiz.API.Code.Extensions;
using EmissionWiz.API.Code.GlobalFilters;
using EmissionWiz.API.Code.Middleware;
using EmissionWiz.Models;
using EmissionWiz.Models.Configs;
using EmissionWiz.Models.Database;
using EmissionWiz.Models.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Globalization;
using System.Text.Json.Serialization;

var cultureInfo = new CultureInfo("en-US");

CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());

// Add services to the container.
builder.Services
    .AddMvc(options =>
    {
        options.Filters.Add(new ResponseCacheAttribute
        {
            NoStore = true,
            Location = ResponseCacheLocation.None
        });
    })
    .AddControllersAsServices()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSpaStaticFiles(c =>
{
    c.RootPath = "../EmissionWiz.Client/dist";
});

builder.Services.AddScoped<HttpClientLogHandler>();

builder.Services.AddHttpContextAccessor();

builder.Services.AddDbContextPool<EmissionWizDbContext>(opt =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultDatabaseConnection")
        ?? throw new AppException("Default database connection string is not defined");

    opt.EnableSensitiveDataLogging();
    opt.UseSqlServer(connectionString);
});


builder.Services.AddHttpClient();
builder.Services.Configure<GeoApiConfiguration>(config => builder.Configuration.GetSection("GeoApi").Bind(config));
builder.Services.AddHttpClient(Constants.HttpClientName.GeoApify, (serviceProvider, httpClient) =>
{
    var geoApiConfig = serviceProvider.GetRequiredService<IOptions<GeoApiConfiguration>>().Value;
    httpClient.BaseAddress = new Uri(geoApiConfig.BaseUrl);
});

builder.Host.AddAutofacModules();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles();
if (!app.Environment.IsDevelopment())
{
    app.UseSpaStaticFiles();
}

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapWhen(x => x.Request.Path.Value!.StartsWith("/src")
                     || x.Request.Path.Value!.StartsWith("/@react-refresh")
                     || x.Request.Path.Value!.StartsWith("/@vite")
                     || x.Request.Path.Value!.StartsWith("/node_modules"), spaBuilder =>
                     {
                         spaBuilder.UseSpa(spa =>
                         {
                             if (app.Environment.IsDevelopment())
                             {
                                 spa.Options.SourcePath = "../EmissionWiz.Client";
                                 spa.Options.StartupTimeout = TimeSpan.FromSeconds(10);

                                 spa.UseViteServer();
                             }
                         });
                     });

app.UseHtmlFallback();

app.Run();
