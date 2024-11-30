using System.Diagnostics;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Pillars.DefaultToolset;

public static class Creator
{
    private static IContainer CreateContainer(IConfiguration createConfig,IConfiguration pillarConfig)
    {
        ContainerBuilder builder = new();
        
        // inject logging and IServiceProvider
        {
            var serviceCollection = new ServiceCollection();
            
            Serilog.Debugging.SelfLog.Enable(msg => Debug.WriteLine(msg));
            var logger = new LoggerConfiguration()
                .ReadFrom.Configuration(createConfig)
                .CreateLogger();
            
            serviceCollection.AddLogging(loggingBuilder =>
                loggingBuilder.AddSerilog(logger,dispose: true));
            builder.Populate(serviceCollection);
        }
        // inject library
        builder.RegisterType<Pillar>().AsSelf().SingleInstance();
        builder.RegisterType<SdlLibrary>().AsSelf().SingleInstance();
        builder.RegisterInstance(pillarConfig).As<IConfiguration>().SingleInstance().OwnedByLifetimeScope();
        
        return builder.Build();
    }

    private static AutofacServiceProvider CreateServiceProvider(IContainer container)
    {
        return new AutofacServiceProvider(container);
    }

    public static Pillar Create(IConfiguration createConfig,IConfiguration pillarConfig)
    {
        var service = CreateServiceProvider(CreateContainer(createConfig,pillarConfig));
        var pillar = service.GetService(typeof(Pillar));

        if (pillar is null)
        {
            throw ExceptionHelper.CanNotGetService(nameof(Pillar));
        }

        return (Pillar)pillar;
    }
}