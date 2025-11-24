using MediatR;
using app.Application.Common.Models;
using app.Application.Admin.DTOs;

namespace app.Application.Admin.Queries.GetAllInvitations;

public class GetAllInvitationsQuery : IRequest<Result<List<InvitationTokenDto>>>
{
}
