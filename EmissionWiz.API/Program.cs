using EmissionWiz.API.Code.GlobalFilters;
using EmissionWiz.Logic;
using EmissionWiz.Models;
using EmissionWiz.Models.Configs;
using HandlebarsDotNet;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<HttpClientLogHandler>();
builder.Services.AddHttpContextAccessor();

builder.Services.AddLogic();

builder.Services.AddHttpClient();
builder.Services.Configure<GeoApiConfiguration>(config => builder.Configuration.GetSection("GeoApi").Bind(config));
builder.Services.AddHttpClient(Constants.HttpClientName.GeoApi, (serviceProvider, httpClient) =>
{
    var geoApiConfig = serviceProvider.GetRequiredService<IOptions<GeoApiConfiguration>>().Value;
    httpClient.BaseAddress = new Uri(geoApiConfig.BaseUrl);
});
 
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
