using Autofac;
using EmissionWiz.Models.Interfaces.Managers;

namespace EmissionWiz.Logic.Managers;

public abstract class BaseManager : IBaseManager
{
    public IComponentContext ComponentContext { get; set; } = null!;
}
