using EmissionWiz.Logic.Managers;
using EmissionWiz.Models;
using EmissionWiz.Models.Attributes;
using Microsoft.Extensions.DependencyInjection;

namespace EmissionWiz.Logic
{
    public static class LogicModule
    {
        public static IServiceCollection AddLogic(this IServiceCollection services)
        {
            var logicAssembly = typeof(LogicModule).Assembly;
            var modelAssembly = typeof(Constants).Assembly;

            var logicTypes = logicAssembly.GetTypes();
            foreach (var type in logicTypes.Where(t => !t.IsAbstract && typeof(BaseManager).IsAssignableFrom(t)))
            {
                var interfaceName = modelAssembly.GetName().Name + ".Interfaces.Managers.I" + type.Name;
                var interfaceType = modelAssembly.GetType(interfaceName);
                if (interfaceType != null)
                {
                    var transientAttribute = type.GetCustomAttributes(typeof(TransientDependencyAttribute), true).Any();

                    if (transientAttribute)
                        services.AddTransient(interfaceType, type);
                    else
                        services.AddScoped(interfaceType, type);
                }
            }

            return services;
        }
    }
}
