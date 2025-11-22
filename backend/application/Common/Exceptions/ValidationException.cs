namespace app.Application.Common.Exceptions;

public class ValidationException : Exception
{
    public Dictionary<string, string[]> Errors { get; }

    public ValidationException()
        : base("Ocorreram um ou mais erros de validação.")
    {
        Errors = new Dictionary<string, string[]>();
    }

    public ValidationException(Dictionary<string, string[]> errors)
        : this()
    {
        Errors = errors;
    }

    public ValidationException(string propertyName, string errorMessage)
        : this()
    {
        Errors.Add(propertyName, new[] { errorMessage });
    }
}
