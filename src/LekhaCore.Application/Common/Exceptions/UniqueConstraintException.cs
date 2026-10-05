namespace LekhaCore.Application.Common.Exceptions;

public sealed class UniqueConstraintException : Exception
{
    public UniqueConstraintException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
