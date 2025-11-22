namespace app.Application.Common.Exceptions;

public class UnauthorizedException : Exception
{
    public UnauthorizedException()
        : base("Não autorizado.")
    {
    }

    public UnauthorizedException(string message)
        : base(message)
    {
    }
}
