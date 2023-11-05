using EmissionWiz.Logic.Managers;
using EmissionWiz.Logic.Managers.CalculationManagers.MaxConcentrationSingleSource;
using EmissionWiz.Models.Interfaces.Managers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// TODO: Register by script
builder.Services.AddScoped<ISingleSourceEmissionCalculationManager, SingleSourceEmissionCalculationManager>();
builder.Services.AddScoped<ICalculationReportManager, CalculationReportManager>();
builder.Services.AddTransient<IPdfManager, PdfManager>();

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
