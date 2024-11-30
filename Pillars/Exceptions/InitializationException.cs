namespace Pillars.Exceptions;

public sealed class InitializationException : Exception
{
    public string Library { get; init; }
    
    public string Reason { get; init; }

    public InitializationException(string library, string reason) : base($"the library {library} initialization failed because:{reason}")
    {
        Library = library;
        Reason = reason;
    }
}