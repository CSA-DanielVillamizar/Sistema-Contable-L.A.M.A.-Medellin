namespace LAMAMedellin.Application.Common.Exceptions;

public sealed class ExcepcionNegocio(string mensaje) : Exception(mensaje)
{
}
