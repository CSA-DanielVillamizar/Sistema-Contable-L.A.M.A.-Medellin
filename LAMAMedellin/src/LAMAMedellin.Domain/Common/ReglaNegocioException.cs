namespace LAMAMedellin.Domain.Common;

public sealed class ReglaNegocioException : Exception
{
    public ReglaNegocioException(string message) : base(message)
    {
    }
}
