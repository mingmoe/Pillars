using System.Text;
using Microsoft.Extensions.Configuration;

namespace Pillars.DefaultToolset;

public static class Configuration
{
    public const string DefaultConfiguration =
        """
        {
            "Serilog": {
              "Using":  [ "Serilog.Sinks.Console", "Serilog.Sinks.File", "Serilog.Sinks.Debug" ],
              "MinimumLevel": "Debug",
              "WriteTo": [
                { "Name": "Console" },
                { "Name": "File", "Args": { "path": "Logs/log.txt" } },
                "Debug"
              ],
              "Enrich": [ "FromLogContext", "WithMachineName", "WithThreadId" ],
              "Destructure": [
                { "Name": "ToMaximumDepth", "Args": { "maximumDestructuringDepth": 4 } },
                { "Name": "ToMaximumStringLength", "Args": { "maximumStringLength": 100 } },
                { "Name": "ToMaximumCollectionCount", "Args": { "maximumCollectionCount": 10 } }
              ],
              "Properties": {
                  "Application": "Pillars"
              }
            }
        }
        """;
    
    public static IConfiguration CreateFastCreateConfiguration()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection()
            .AddJsonStream(new MemoryStream(Encoding.UTF8.GetBytes(DefaultConfiguration)))
            .Build();
        
        return config;
    }
}