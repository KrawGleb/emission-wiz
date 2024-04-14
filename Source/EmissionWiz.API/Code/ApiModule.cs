using Autofac;
using EmissionWiz.API.Controllers.Base;

namespace EmissionWiz.API.Code;

public class ApiModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<HttpContextAccessor>().As<IHttpContextAccessor>().SingleInstance();

        foreach (var type in typeof(BaseController).Assembly.GetTypes().Where(t => !t.IsAbstract && typeof(BaseController).IsAssignableFrom(t)))
        {
            builder.RegisterType(type).PropertiesAutowired();
        }
    }
}
