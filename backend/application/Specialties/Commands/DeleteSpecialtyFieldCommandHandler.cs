using MediatR;
using Microsoft.EntityFrameworkCore;
using app.Application.Common.Interfaces;
using app.Application.Common.Exceptions;

namespace app.Application.Specialties.Commands;

public class DeleteSpecialtyFieldCommandHandler : IRequestHandler<DeleteSpecialtyFieldCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public DeleteSpecialtyFieldCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(DeleteSpecialtyFieldCommand request, CancellationToken cancellationToken)
    {
        var field = await _context.SpecialtyFields
            .FirstOrDefaultAsync(f => f.Id == request.FieldId, cancellationToken);

        if (field == null)
            throw new NotFoundException("Campo não encontrado");

        _context.SpecialtyFields.Remove(field);
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
