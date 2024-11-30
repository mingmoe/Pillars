using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Pillars.Exceptions;
using SDL3;

namespace Pillars;

public sealed class SdlLibrary : IDisposable
{
    private bool _disposed=false;
    
    public ILogger<SdlLibrary> Logger { private get; init; }
    
    public static string GetLastError()
    {
        var err = SDL.SDL_GetError();
        return string.IsNullOrWhiteSpace(err) ? "unknown error" : err;
    }
    
    public SdlLibrary(ILogger<SdlLibrary> logger,IConfiguration config)
    {
        Logger = logger;
        var success = SDL.SDL_Init(SDL.SDL_InitFlags.SDL_INIT_AUDIO | 
                                   SDL.SDL_InitFlags.SDL_INIT_VIDEO |
                                   SDL.SDL_InitFlags.SDL_INIT_JOYSTICK |
                                   SDL.SDL_InitFlags.SDL_INIT_HAPTIC |
                                   SDL.SDL_InitFlags.SDL_INIT_GAMEPAD |
                                   SDL.SDL_InitFlags.SDL_INIT_EVENTS |
                                   SDL.SDL_InitFlags.SDL_INIT_SENSOR |
                                   SDL.SDL_InitFlags.SDL_INIT_CAMERA |
                                   SDL.SDL_InitFlags.SDL_INIT_TIMER
        );
        
        if (!success)
        {
            throw ExceptionHelper.FailedToInit("SDL3",GetLastError());
        }

        var options = config.Get<SdlOptions>() ?? new SdlOptions();
        
        if (options.LoggingEnabled == true)
        {
            SDL.SDL_SetLogPriorities(SDL.SDL_LogPriority.SDL_LOG_PRIORITY_TRACE);
            // note that we should reset the output function when disposing
            unsafe
            {
                SDL.SDL_SetLogOutputFunction((_,_,priority,message) =>
                {
                    var log = Marshal.PtrToStringUTF8((nint)message);

                    if (string.IsNullOrWhiteSpace(log))
                    {
                        return;
                    }

                    switch (priority)
                    {
                        case SDL.SDL_LogPriority.SDL_LOG_PRIORITY_INVALID:
                        case SDL.SDL_LogPriority.SDL_LOG_PRIORITY_TRACE:
                        case SDL.SDL_LogPriority.SDL_LOG_PRIORITY_VERBOSE:
                            Logger.LogTrace(log);
                            break;
                        case SDL.SDL_LogPriority.SDL_LOG_PRIORITY_DEBUG:
                            Logger.LogDebug(log);
                            break;
                        case SDL.SDL_LogPriority.SDL_LOG_PRIORITY_INFO:
                            Logger.LogInformation(log);
                            break;
                        case SDL.SDL_LogPriority.SDL_LOG_PRIORITY_WARN:
                            Logger.LogWarning(log);
                            break;
                        case SDL.SDL_LogPriority.SDL_LOG_PRIORITY_ERROR:
                            Logger.LogError(log);
                            break;
                        case SDL.SDL_LogPriority.SDL_LOG_PRIORITY_CRITICAL:
                        case SDL.SDL_LogPriority.SDL_LOG_PRIORITY_COUNT:
                            Logger.LogCritical(log);
                            break;
                        default:
                            Logger.LogError($"Unknown log priority with message:{log}");
                            break;
                    }
                },IntPtr.Zero);
            }
        }
    }
    
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    ~SdlLibrary()
    {
        Dispose(false);
    }

    private void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        
        SDL.SDL_ResetLogPriorities();
        // reset the log function
        SDL.SDL_GetLogOutputFunction(out SDL.SDL_LogOutputFunction callback,out nint bUserdata);
        SDL.SDL_SetLogOutputFunction(callback,bUserdata);
        SDL.SDL_Quit();
    }
}