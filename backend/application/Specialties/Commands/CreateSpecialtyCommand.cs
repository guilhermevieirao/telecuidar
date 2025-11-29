using MediatR;
using app.Application.Common.Models;

namespace app.Application.Specialties.Commands;

public class CreateSpecialtyCommand : IRequest<Result<int>>
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Icon { get; set; }
}
