namespace Pillars.Exceptions;

internal static class ExceptionHelper
{
    public static InitializationException FailedToInit(string library,string reason)
    {
        return new InitializationException(library, reason);
    }
}