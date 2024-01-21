using Autofac;
using EmissionWiz.DataProvider;
using EmissionWiz.Logic;

namespace EmissionWiz.API.Code.Extensions;

public static class AutofacServiceExtensions
{
    public static ConfigureHostBuilder AddAutofacModules(this ConfigureHostBuilder hostBuilder)
    {
        hostBuilder.ConfigureContainer<ContainerBuilder>(builder =>
        {
            builder.RegisterModule<ApiModule>();
            builder.RegisterModule<DataModule>();
            builder.RegisterModule<LogicModule>();
        });

        return hostBuilder;
    }
}
