using Autofac;
using EmissionWiz.API.Controllers.Base;
using EmissionWiz.Models.Calculations.SingleSource;
using EmissionWiz.Models.Exceptions;
using EmissionWiz.Models.Interfaces.Managers;
using EmissionWiz.Models.Interfaces.Providers;
using EmissionWiz.Models.Interfaces.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace EmissionWiz.API.Controllers;

public class SingleSourceController : BaseApiController
{
    private readonly ITempFileRepository _tempFileRepository;

    public SingleSourceController(
        ITempFileRepository tempFileRepository)
    {
        _tempFileRepository = tempFileRepository;
    }

    [HttpPost]
    public async Task<IActionResult> Calculate([FromBody] SingleSourceInputModel model)
    {
        var results = new List<SingleSourceEmissionCalculationResult>();

        foreach (var substance in model.Substances)
        {
            CancellationToken.ThrowIfCancellationRequested();

            await using var scope = LifetimeScope.BeginLifetimeScope();
            var maxConcentrationManager = scope.Resolve<ISingleSourceEmissionCalculationManager>();
            var result = await maxConcentrationManager.Calculate(new SingleSourceCalculationData
            {
                A = model.A,
                AirTemperature = model.AirTemperature,
                D = model.D,
                EmissionTemperature = model.EmissionTemperature,
                Eta = model.Eta,
                FCoef = model.FCoef,
                H = model.H,
                Lat = model.Lat,
                Lon = model.Lon,
                M = substance.M,
                U = model.U,
                W = model.W,
                X = model.X,
                Y = model.Y,
                B = model.B,
                L = model.L,
                SubstanceId = substance.Id,
                SubstanceName = substance.Name,
                WindRose = model.WindRose,
                ResultsConfig = model.ResultsConfig,
            });

            results.Add(result);

            var commitProvider = scope.Resolve<ICommitProvider>();
            await commitProvider.CommitAsync();
        }

        return Ok(results);
    }

    [HttpGet("Report")]
    public async Task<IActionResult> GetReport([FromQuery] Guid reportId)
    {
        var report = await _tempFileRepository.GetByIdAsync(reportId)
            ?? throw new NotFoundException($"Report with id {reportId} was not found");

        if (report.Data == null)
            return NoContent();

        return File(report.Data, "application/octet-stream", report.FileName);
    }
}
