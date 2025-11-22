namespace app.Application.Common.Exceptions;

public class ForbiddenAccessException : Exception
{
    public ForbiddenAccessException()
        : base("Acesso negado.")
    {
    }

    public ForbiddenAccessException(string message)
        : base(message)
    {
    }
}
