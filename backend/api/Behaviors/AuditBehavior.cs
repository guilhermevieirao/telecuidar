using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using app.Domain.Interfaces;

namespace app.Api.Behaviors;

public class AuditBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuditBehavior(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
    {
        _unitOfWork = unitOfWork;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        // Configurar informações de auditoria antes de executar o comando
        var httpContext = _httpContextAccessor.HttpContext;
        
        if (httpContext != null)
        {
            // Obter ID do usuário do token JWT
            var userIdClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int? userId = null;
            
            if (!string.IsNullOrEmpty(userIdClaim) && int.TryParse(userIdClaim, out var parsedUserId))
            {
                userId = parsedUserId;
            }

            // Obter IP e User Agent
            var ipAddress = httpContext.Connection.RemoteIpAddress?.ToString();
            var userAgent = httpContext.Request.Headers["User-Agent"].ToString();

            _unitOfWork.SetAuditInfo(userId, ipAddress, userAgent);
        }

        // Continuar com a execução do comando
        return await next();
    }
}
