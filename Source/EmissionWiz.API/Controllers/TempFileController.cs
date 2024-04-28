using EmissionWiz.API.Controllers.Base;
using EmissionWiz.Models.Database;
using EmissionWiz.Models.Interfaces.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace EmissionWiz.API.Controllers;


public class TempFileController : BaseApiController
{
    private readonly ITempFileRepository _tempFileRepository;

    public TempFileController(ITempFileRepository tempFileRepository)
    {
        _tempFileRepository = tempFileRepository;
    }

    [HttpGet]
    public async Task<IActionResult> GetByIdAsync([FromQuery] Guid id)
    {
        var file = await _tempFileRepository.GetByIdAsync(id);
        if (file?.Data == null)
            return NotFound();

        return File(file.Data, "application/octet-stream", file.FileName);
    }
}
