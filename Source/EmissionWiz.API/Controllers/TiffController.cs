using EmissionWiz.API.Controllers.Base;
using EmissionWiz.Models.Interfaces.Repositories;
using Microsoft.AspNetCore.Mvc;
using SixLabors.ImageSharp;

namespace EmissionWiz.API.Controllers;

public class TiffController : BaseApiController
{
    private readonly ITempFileRepository _tempFileRepository;

    public TiffController(ITempFileRepository tempFileRepository)
    {
        _tempFileRepository = tempFileRepository;
    }

    [HttpGet]
    public async Task<IActionResult> Preview([FromQuery] Guid id)
    {
        var file = await _tempFileRepository.GetByIdAsync(id);
        if (file?.Data == null)
            return NotFound();

        using var inputMs = new MemoryStream(file.Data);
        using var image = await Image.LoadAsync(inputMs);

        using var outputMs = new MemoryStream();
        await image.SaveAsJpegAsync(outputMs);

        return File(outputMs.ToArray(), "application/octet-stream", file.FileName);
    }
}
