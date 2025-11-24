using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace app.Application.Common.Behaviors;

/// <summary>
/// Behavior para registrar logs detalhados de todas as requisições MediatR,
/// incluindo tempo de execução e alertas de performance.
/// </summary>
public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;
    private const int SlowRequestThresholdMs = 500;

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        
        _logger.LogInformation("Iniciando requisição: {RequestName} {@Request}", requestName, request);
        
        var stopwatch = Stopwatch.StartNew();
        
        try
        {
            var response = await next();
            
            stopwatch.Stop();
            var elapsedMs = stopwatch.ElapsedMilliseconds;
            
            // Alerta se a requisição for lenta
            if (elapsedMs > SlowRequestThresholdMs)
            {
                _logger.LogWarning("⚠️ Requisição LENTA: {RequestName} levou {ElapsedMilliseconds}ms (limite: {Threshold}ms)",
                    requestName, elapsedMs, SlowRequestThresholdMs);
            }
            else
            {
                _logger.LogInformation("Requisição {RequestName} concluída em {ElapsedMilliseconds}ms",
                    requestName, elapsedMs);
            }
            
            return response;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            
            _logger.LogError(ex, "Erro na requisição {RequestName} após {ElapsedMilliseconds}ms",
                requestName, stopwatch.ElapsedMilliseconds);
            
            throw;
        }
    }
}
