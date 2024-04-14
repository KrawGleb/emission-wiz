using Autofac;
using Microsoft.AspNetCore.Mvc;

namespace EmissionWiz.API.Controllers.Base;

[Route("api/[controller]")]
[ApiController]
public class BaseApiController : BaseController
{
    public ILifetimeScope LifetimeScope { get; set; } = null!;
    protected CancellationToken CancellationToken => HttpContext.RequestAborted;
}
