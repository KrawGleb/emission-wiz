using Autofac.Extensions.DependencyInjection;
using EmissionWiz.API.Code.Extensions;
using EmissionWiz.API.Code.GlobalFilters;
using EmissionWiz.Models;
using EmissionWiz.Models.Configs;
using EmissionWiz.Models.Database;
using EmissionWiz.Models.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<HttpClientLogHandler>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddMvc().AddControllersAsServices();

builder.Services.AddDbContextPool<EmissionWizDbContext>(opt =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultDatabaseConnection")
        ?? throw new AppException("Default database connection string is not defined");

    opt.EnableSensitiveDataLogging();
    opt.UseSqlServer(connectionString);
});

builder.Services.AddHttpClient();
builder.Services.Configure<GeoApiConfiguration>(config => builder.Configuration.GetSection("GeoApi").Bind(config));
builder.Services.AddHttpClient(Constants.HttpClientName.GeoApi, (serviceProvider, httpClient) =>
{
    var geoApiConfig = serviceProvider.GetRequiredService<IOptions<GeoApiConfiguration>>().Value;
    httpClient.BaseAddress = new Uri(geoApiConfig.BaseUrl);
});

builder.Host.AddAutofacModules();

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapFallbackToFile("/index.html");

app.Run();
