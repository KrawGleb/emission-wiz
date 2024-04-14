using Autofac;
using EmissionWiz.Logic.Builders;
using EmissionWiz.Logic.Managers;
using EmissionWiz.Logic.Providers;
using EmissionWiz.Models;
using EmissionWiz.Models.Attributes;
using EmissionWiz.Models.Interfaces.Builders;
using EmissionWiz.Models.Interfaces.Providers;

namespace EmissionWiz.Logic
{
    public class LogicModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<CurrentTimeProvider>().As<ICurrentTimeProvider>().SingleInstance();
            builder.RegisterType<DateTimeProvider>().As<IDateTimeProvider>().SingleInstance();
            builder.RegisterType<MapImageBuilder>().As<IMapImageBuilder>().InstancePerLifetimeScope();

            var logicAssembly = typeof(LogicModule).Assembly;
            var modelAssembly = typeof(Constants).Assembly;

            var logicTypes = logicAssembly.GetTypes();
            foreach (var type in logicTypes.Where(t => !t.IsAbstract && typeof(BaseManager).IsAssignableFrom(t)))
            {
                var interfaceName = modelAssembly.GetName().Name + ".Interfaces.Managers.I" + type.Name;
                var interfaceType = modelAssembly.GetType(interfaceName);
                if (interfaceType != null)
                {
                    var instancePerDependency = type.GetCustomAttributes(typeof(InstancePerDependencyAttribute), true).Any();
                    var builderObj = builder.RegisterType(type).As(interfaceType).PropertiesAutowired();

                    if (!instancePerDependency)
                        builderObj.InstancePerLifetimeScope();
                }
            }
        }
    }
}
