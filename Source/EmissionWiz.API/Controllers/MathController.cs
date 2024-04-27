using EmissionWiz.API.Controllers.Base;
using EmissionWiz.Models.Dto;
using EmissionWiz.Models.Interfaces.Managers;
using Microsoft.AspNetCore.Mvc;

namespace EmissionWiz.API.Controllers;

public class MathController : BaseApiController
{
    private readonly IMathManager _mathManager;

    public MathController(IMathManager mathManager)
    {
        _mathManager = mathManager;
    }

    [HttpPost("spline")]
    public async Task<IActionResult> Spline([FromBody] SplineData input)
    {
        var result = _mathManager.Spline(input.Xs, input.Ys, input.Count);
        return Ok(result);
    }
}
