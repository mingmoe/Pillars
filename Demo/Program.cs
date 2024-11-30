using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Pillars.DefaultToolset;

namespace Demo;

class Program
{
    static void Main(string[] args)
    {
        var fastCreateConfiguration = Configuration.CreateFastCreateConfiguration();
        
        var pillarConfig = new ConfigurationBuilder()
            .Build();
        
        using var pillar  = Creator.Create(fastCreateConfiguration,pillarConfig);
        
        var logger = (ILogger<Program>)pillar.ServiceProvider.GetService(typeof(ILogger<Program>))!;
        logger.LogDebug("Hello World!");
        logger.LogTrace("Hello World!");
        logger.LogInformation("Hello World!");
        logger.LogWarning("Hello World!");
        logger.LogError("Hello World!");
        logger.LogCritical("Hello World!");
    }
}