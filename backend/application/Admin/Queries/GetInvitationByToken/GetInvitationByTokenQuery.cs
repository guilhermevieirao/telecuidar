using MediatR;
using app.Application.Common.Models;
using app.Application.Admin.DTOs;

namespace app.Application.Admin.Queries.GetInvitationByToken;

public class GetInvitationByTokenQuery : IRequest<Result<InvitationDetailsDto>>
{
    public string Token { get; set; } = string.Empty;
}
