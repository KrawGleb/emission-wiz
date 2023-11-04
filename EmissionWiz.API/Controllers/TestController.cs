using EmissionWiz.Models.Calculations;
using EmissionWiz.Models.Interfaces.Managers;
using Microsoft.AspNetCore.Http;
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

        [HttpGet("test")]
        public async Task<IActionResult> Get()
        {
            var input = new MaxConcentrationInputModel
            {
                A = 160,
                H = 30,
                F = 1,
                AirTemperature = 25.3,
                EmissionTemperature = 160,
                M = 4.1,
                D = 1,
                Eta = 1,
                W = 7.06
            };

            return Ok(_maxConcentrationManager.CalculateMaxConcentration(input));
        }
    }
}
