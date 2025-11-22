using MediatR;

namespace app.Application.Common.Behaviors;

public class UnhandledExceptionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        try
        {
            return await next();
        }
        catch (Exception ex)
        {
            var requestName = typeof(TRequest).Name;
            
            // TODO: Adicionar logging estruturado
            Console.WriteLine($"Exceção não tratada na requisição {requestName}: {ex.Message}");
            
            throw;
        }
    }
}
