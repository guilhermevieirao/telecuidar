namespace app.Application.Common.Exceptions;

public class NotFoundException : Exception
{
    public NotFoundException(string name, object key)
        : base($"Entity \"{name}\" ({key}) não foi encontrada.")
    {
    }

    public NotFoundException(string message)
        : base(message)
    {
    }
}
