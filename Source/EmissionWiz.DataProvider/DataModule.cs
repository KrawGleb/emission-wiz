using Autofac;
using EmissionWiz.DataProvider.Database;
using EmissionWiz.DataProvider.Providers;
using EmissionWiz.DataProvider.Repositories.Base;
using EmissionWiz.Models;
using EmissionWiz.Models.Database;
using EmissionWiz.Models.Interfaces.Providers;

namespace EmissionWiz.DataProvider;

public class DataModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<DatabaseContext>().As<IDatabaseContext>().InstancePerLifetimeScope();
        builder.RegisterType<CommitProvider>().As<ICommitProvider>().InstancePerLifetimeScope();

        var dalAssembly = typeof(DataModule).Assembly;
        var modelAssembly = typeof(Constants).Assembly;

        var dalTypes = dalAssembly.GetTypes();

        foreach (var type in dalTypes.Where(t => !t.IsAbstract && typeof(BaseRepository).IsAssignableFrom(t)))
        {
            var interfaceName = modelAssembly.GetName().Name + ".Interfaces.Repositories.I" + type.Name;
            var interfaceType = modelAssembly.GetType(interfaceName);
            if (interfaceType != null)
            {
                builder.RegisterType(type).As(interfaceType)
                    .InstancePerLifetimeScope()
                    .PropertiesAutowired();
            }
        }
    }
}
