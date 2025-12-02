using MediatR;
using Microsoft.EntityFrameworkCore;
using app.Application.Common.Interfaces;
using app.Application.Specialties.DTOs;
using System.Text.Json;

namespace app.Application.Specialties.Queries;

public class GetSpecialtyFieldsQueryHandler : IRequestHandler<GetSpecialtyFieldsQuery, List<SpecialtyFieldDto>>
{
    private readonly IApplicationDbContext _context;

    public GetSpecialtyFieldsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<SpecialtyFieldDto>> Handle(GetSpecialtyFieldsQuery request, CancellationToken cancellationToken)
    {
        var fields = await _context.SpecialtyFields
            .Where(f => f.SpecialtyId == request.SpecialtyId && f.IsActive)
            .OrderBy(f => f.DisplayOrder)
            .ToListAsync(cancellationToken);

        return fields.Select(f => new SpecialtyFieldDto
        {
            Id = f.Id,
            SpecialtyId = f.SpecialtyId,
            FieldName = f.FieldName,
            Label = f.Label,
            Description = f.Description,
            FieldType = f.FieldType,
            Options = f.Options != null ? JsonSerializer.Deserialize<List<string>>(f.Options) : null,
            IsRequired = f.IsRequired,
            DisplayOrder = f.DisplayOrder,
            DefaultValue = f.DefaultValue,
            ValidationRules = f.ValidationRules != null ? JsonSerializer.Deserialize<Dictionary<string, object>>(f.ValidationRules) : null,
            Placeholder = f.Placeholder
        }).ToList();
    }
}
