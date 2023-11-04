using EmissionWiz.Logic.Managers;
using EmissionWiz.Logic.Managers.CalculationManagers.MaxConcentrationSingleSource;
using EmissionWiz.Models.Interfaces.Managers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IMaxConcentrationSingleSourceCalculationManager, MaxConcentrationSingleSourceCalculationManager>();
builder.Services.AddScoped<ICalculationReportManager, CalulactionReportManager>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
