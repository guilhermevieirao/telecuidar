using MediatR;
using System.Diagnostics;

namespace app.Application.Common.Behaviors;

public class PerformanceBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly Stopwatch _timer;

    public PerformanceBehavior()
    {
        _timer = new Stopwatch();
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        _timer.Start();

        var response = await next();

        _timer.Stop();

        var elapsedMilliseconds = _timer.ElapsedMilliseconds;

        // Alerta se a requisição demorar mais de 500ms
        if (elapsedMilliseconds > 500)
        {
            var requestName = typeof(TRequest).Name;
            
            Console.WriteLine($"⚠️ Requisição Lenta: {requestName} ({elapsedMilliseconds}ms)");
            
            // TODO: Adicionar logging estruturado ou telemetria
        }

        return response;
    }
}
