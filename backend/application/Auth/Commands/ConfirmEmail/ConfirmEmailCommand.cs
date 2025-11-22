using MediatR;
using app.Application.Common.Models;

namespace app.Application.Auth.Commands.ConfirmEmail;

public record ConfirmEmailCommand(string Token) : IRequest<Result<bool>>;
