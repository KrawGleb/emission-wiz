using EmissionWiz.API.Controllers.Base;
using EmissionWiz.Models.Interfaces.Managers;
using Microsoft.AspNetCore.Mvc;

namespace EmissionWiz.API.Controllers;

public class SubstanceController : BaseApiController
{
    private readonly ISubstanceManager _substanceManager;

    public SubstanceController(ISubstanceManager substanceManager)
    {
        _substanceManager = substanceManager;
    }

    [HttpGet]
    public async Task<IActionResult> GetSubstances()
    {
        var result = await _substanceManager.GetSubstancesAsync();
        return Ok(result);
    }
}
