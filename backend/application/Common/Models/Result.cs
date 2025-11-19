namespace app.Application.Common.Models;

public class Result<T>
{
    public T? Data { get; set; }
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<string> Errors { get; set; } = new();
    
    public static Result<T> Success(T data, string message = "Operação realizada com sucesso")
    {
        return new Result<T> { Data = data, IsSuccess = true, Message = message };
    }
    
    public static Result<T> Failure(string message, List<string>? errors = null)
    {
        return new Result<T> { IsSuccess = false, Message = message, Errors = errors ?? new List<string>() };
    }
}