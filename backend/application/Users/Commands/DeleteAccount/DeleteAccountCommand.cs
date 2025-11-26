using MediatR;

namespace app.Application.Users.Commands.DeleteAccount;

public record DeleteAccountCommand(int UserId) : IRequest<bool>;
