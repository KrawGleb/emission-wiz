using EmissionWiz.Models.Calculations.SingleSource;
using EmissionWiz.Models.Interfaces.Managers;
using Microsoft.AspNetCore.Mvc;

namespace EmissionWiz.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly ISingleSourceEmissionCalculationManager _maxConcentrationManager;

        public TestController(ISingleSourceEmissionCalculationManager maxConcentrationManager)
        {
            _maxConcentrationManager = maxConcentrationManager;
        }

        [HttpGet("hot")]
        public async Task<IActionResult> Get()
        {
            var hot = new SingleSourceInputModel
            {
                A = 160,
                H = 10,
                FCoef = 1,
                AirTemperature = 25.3,
                EmissionTemperature = 25.3+3,
                M = 4.1,
                D = 1,
                Eta = 1,
                W = 3.06,
                U = 4
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
                U = 4
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
                U = 4
            };

            return Ok(await _maxConcentrationManager.Calculate(lowWind, "LowWindEmission.pdf"));
        }
    }
}
