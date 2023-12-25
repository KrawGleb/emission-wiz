using EmissionWiz.Logic.Managers;
using EmissionWiz.Logic.Managers.CalculationManagers.MaxConcentrationSingleSource;
using EmissionWiz.Logic.Managers.CalculationManagers.SingleSource;
using EmissionWiz.Logic.Managers.CalculationManagers.SingleSource.DangerouesWindSpeedCalculationManagers;
using EmissionWiz.Logic.Managers.CalculationManagers.SingleSource.DangerousDistanceCalculationManagers;
using EmissionWiz.Logic.Managers.CalculationManagers.SingleSource.MaxConcentrationCalculationManagers;
using EmissionWiz.Models.Interfaces.Managers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
    
builder.Services.AddHttpContextAccessor();

// TODO: Register by script
builder.Services.AddScoped<ISingleSourceEmissionCalculationManager, SingleSourceEmissionCalculationManager>();
builder.Services.AddScoped<ISingleSourceEmissionReportModelBuilder, SingleSourceEmissionReportModelBuilder>();
builder.Services.AddScoped<IReportManager, ReportManager>();
builder.Services.AddTransient<IPdfManager, PdfManager>();

builder.Services.AddScoped<IColdEmissionDangerousDistanceCalculationManager, ColdEmissionDangerousDistanceCalculationManager>();
builder.Services.AddScoped<IHotEmissionDangerousDistanceCalculationManager, HotEmissionDangerousDistanceCalculationManager>();
builder.Services.AddScoped<ILowWindDangerousDistanceCalculationManager, LowWindDangerousDistanceCalculationManager>();

builder.Services.AddScoped<IColdEmissionMaxConcentrationCalculationSubManager, ColdEmissionMaxConcentrationCalculationManager>();
builder.Services.AddScoped<IHotEmissionMaxConcentrationCalculationSubManager, HotEmissionMaxConcentrationCalculationManager>();
builder.Services.AddScoped<ILowWindMaxConcentrationCalculationSubManager, LowWindMaxConcentrationCalculationManager>();

builder.Services.AddScoped<IColdEmissionDangerousWindSpeedCalculationManager, ColdEmissionDangerousWindSpeedCalculationManager>();
builder.Services.AddScoped<IHotEmissionDangerousWindSpeedCalculationManager, HotEmissionDangerousWindSpeedCalculationManager>();
builder.Services.AddScoped<ILowWindDangerousWindSpeedCalculationManager, LowWindDangerousWindSpeedCalculationManager>();

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
