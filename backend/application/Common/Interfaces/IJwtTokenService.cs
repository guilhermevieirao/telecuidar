using app.Domain.Entities;

namespace app.Application.Common.Interfaces;

public interface IJwtTokenService
{
    string GenerateToken(User user);
    Guid? ValidateToken(string token);
}
