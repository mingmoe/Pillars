using System.Runtime.CompilerServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Pillars;

/// <summary>
/// A pillar object will rule the whole application,taking charge of systems such as graph and audio system,meaning that other part of the application **should not** touch them.
/// </summary>
public class Pillar : IDisposable
{
    public required IServiceProvider ServiceProvider { get; init; }
    
    public required SdlLibrary SdlLibrary { private get; init; }

    public required ILogger<Pillar> Logger { private get; init; }

    protected bool disposed = false;

    ~Pillar()
    {
        Dispose(false);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected void Dispose(bool disposing)
    {
        if (disposed)
        {
            return;
        }
        disposed = true;
    }
    
}