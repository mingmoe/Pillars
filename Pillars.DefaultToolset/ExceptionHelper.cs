namespace Pillars.DefaultToolset;

internal static class ExceptionHelper
{
    public static Exception CanNotGetService(string serviceName)
    {
        return new InvalidOperationException($"Can not get service {serviceName} from the IServiceProvider.");
    }
}