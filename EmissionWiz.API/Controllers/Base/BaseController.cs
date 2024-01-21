using EmissionWiz.Models.Interfaces.Providers;
using Microsoft.AspNetCore.Mvc;

namespace EmissionWiz.API.Controllers.Base;

public abstract class BaseController : Controller
{
    public ICommitProvider CommitProvider { get; set; } = null!;
}
