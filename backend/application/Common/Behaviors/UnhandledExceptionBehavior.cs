using MediatR;
using Microsoft.Extensions.Logging;

namespace app.Application.Common.Behaviors;

/// <summary>
/// Behavior para capturar e registrar exceções não tratadas nas requisições MediatR.
/// </summary>
public class UnhandledExceptionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<UnhandledExceptionBehavior<TRequest, TResponse>> _logger;

    public UnhandledExceptionBehavior(ILogger<UnhandledExceptionBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        try
        {
            return await next();
        }
        catch (Exception ex)
        {
            var requestName = typeof(TRequest).Name;
            
            _logger.LogError(ex, "🔥 Exceção não tratada na requisição {RequestName}: {ErrorMessage}", 
                requestName, ex.Message);
            
            throw;
        }
    }
}
