using EmissionWiz.Models.Calculations.SingleSource;
using Microsoft.AspNetCore.Mvc;
using System.IO;

namespace EmissionWiz.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SingleSourceController : ControllerBase
    {
        private readonly ISingleSourceEmissionCalculationManager _maxConcentrationManager;

        public SingleSourceController(ISingleSourceEmissionCalculationManager maxConcentrationManager)
        {
            _maxConcentrationManager = maxConcentrationManager;
        }

        [HttpPost]
        public async Task<IActionResult> Calculate([FromBody] SingleSourceInputModel model)
        {
            var (result, _) = await _maxConcentrationManager.Calculate(model, "report.pdf");
            return Ok(result);
        }

        [HttpPost("report")]
        public async Task<IActionResult> GetReport([FromBody] SingleSourceInputModel model)
        {
            var (_, stream) = await _maxConcentrationManager.Calculate(model, "report.pdf");
            return File(stream, "application/octet-stream", "report.pdf");
        }

        [HttpGet("hot")]
        public async Task<IActionResult> Get()
        {
            var hot = new SingleSourceInputModel
            {
                A = 160,
                H = 9,
                FCoef = 1,
                AirTemperature = 25.3,
                EmissionTemperature = 25.3+3,
                M = 4.1,
                D = 1,
                Eta = 1,
                W = 3.06,
                U = 4,
                X = 4
            };

            return Ok(await _maxConcentrationManager.Calculate(hot, "HotEmission.pdf"));
        }

        [HttpGet("cold")] 
        public async Task<IActionResult> Test2()
        {
            var cold = new SingleSourceInputModel
            {
                A = 160,
                H = 10,
                FCoef = 1,
                AirTemperature = 25.3,
                EmissionTemperature = 25.3 + 3,
                M = 4.1,
                D = 1,
                Eta = 1,
                W = 30.06,
                U = 4,
                X = 4
            };

            return Ok(await _maxConcentrationManager.Calculate(cold, "ColdEmission.pdf"));
        }

        [HttpGet("low_wind")]
        public async Task<IActionResult> Test3()
        {
            var lowWind = new SingleSourceInputModel
            {
                A = 160,
                H = 15,
                FCoef = 1,
                AirTemperature = 25.3,
                EmissionTemperature = 25.3 + 3,
                M = 4.1,
                D = 0.5,
                Eta = 1,
                W = 8.06,
                U = 4,
                X = 4
            };

            return Ok(await _maxConcentrationManager.Calculate(lowWind, "LowWindEmission.pdf"));
        }
    }
}
