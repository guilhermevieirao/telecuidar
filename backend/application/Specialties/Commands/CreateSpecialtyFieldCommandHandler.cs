using MediatR;
using app.Application.Common.Interfaces;
using app.Application.Specialties.DTOs;
using app.Domain.Entities;
using System.Text.Json;

namespace app.Application.Specialties.Commands;

public class CreateSpecialtyFieldCommandHandler : IRequestHandler<CreateSpecialtyFieldCommand, SpecialtyFieldDto>
{
    private readonly IApplicationDbContext _context;

    public CreateSpecialtyFieldCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<SpecialtyFieldDto> Handle(CreateSpecialtyFieldCommand request, CancellationToken cancellationToken)
    {
        var field = new SpecialtyField
        {
            SpecialtyId = request.SpecialtyId,
            FieldName = request.FieldDto.FieldName,
            Label = request.FieldDto.Label,
            Description = request.FieldDto.Description,
            FieldType = request.FieldDto.FieldType,
            Options = request.FieldDto.Options != null ? JsonSerializer.Serialize(request.FieldDto.Options) : null,
            IsRequired = request.FieldDto.IsRequired,
            DisplayOrder = request.FieldDto.DisplayOrder,
            DefaultValue = request.FieldDto.DefaultValue,
            ValidationRules = request.FieldDto.ValidationRules != null ? JsonSerializer.Serialize(request.FieldDto.ValidationRules) : null,
            Placeholder = request.FieldDto.Placeholder,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        _context.SpecialtyFields.Add(field);
        await _context.SaveChangesAsync(cancellationToken);

        return new SpecialtyFieldDto
        {
            Id = field.Id,
            SpecialtyId = field.SpecialtyId,
            FieldName = field.FieldName,
            Label = field.Label,
            Description = field.Description,
            FieldType = field.FieldType,
            Options = field.Options != null ? JsonSerializer.Deserialize<List<string>>(field.Options) : null,
            IsRequired = field.IsRequired,
            DisplayOrder = field.DisplayOrder,
            DefaultValue = field.DefaultValue,
            ValidationRules = field.ValidationRules != null ? JsonSerializer.Deserialize<Dictionary<string, object>>(field.ValidationRules) : null,
            Placeholder = field.Placeholder
        };
    }
}
