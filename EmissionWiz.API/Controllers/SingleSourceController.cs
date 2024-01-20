using EmissionWiz.Models.Calculations.SingleSource;
using EmissionWiz.Models.Interfaces.Managers;
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
            var result = await _maxConcentrationManager.Calculate(model);
            return Ok(result);
        }
    }
}
