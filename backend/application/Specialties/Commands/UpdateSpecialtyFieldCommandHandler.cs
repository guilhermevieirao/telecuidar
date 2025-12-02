using MediatR;
using Microsoft.EntityFrameworkCore;
using app.Application.Common.Interfaces;
using app.Application.Common.Exceptions;
using app.Application.Specialties.DTOs;
using System.Text.Json;

namespace app.Application.Specialties.Commands;

public class UpdateSpecialtyFieldCommandHandler : IRequestHandler<UpdateSpecialtyFieldCommand, SpecialtyFieldDto>
{
    private readonly IApplicationDbContext _context;

    public UpdateSpecialtyFieldCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<SpecialtyFieldDto> Handle(UpdateSpecialtyFieldCommand request, CancellationToken cancellationToken)
    {
        var field = await _context.SpecialtyFields
            .FirstOrDefaultAsync(f => f.Id == request.FieldId, cancellationToken);

        if (field == null)
            throw new NotFoundException("Campo não encontrado");

        if (request.FieldDto.FieldName != null)
            field.FieldName = request.FieldDto.FieldName;
        
        if (request.FieldDto.Label != null)
            field.Label = request.FieldDto.Label;
        
        if (request.FieldDto.Description != null)
            field.Description = request.FieldDto.Description;
        
        if (request.FieldDto.FieldType != null)
            field.FieldType = request.FieldDto.FieldType;
        
        if (request.FieldDto.Options != null)
            field.Options = JsonSerializer.Serialize(request.FieldDto.Options);
        
        if (request.FieldDto.IsRequired.HasValue)
            field.IsRequired = request.FieldDto.IsRequired.Value;
        
        if (request.FieldDto.DisplayOrder.HasValue)
            field.DisplayOrder = request.FieldDto.DisplayOrder.Value;
        
        if (request.FieldDto.DefaultValue != null)
            field.DefaultValue = request.FieldDto.DefaultValue;
        
        if (request.FieldDto.ValidationRules != null)
            field.ValidationRules = JsonSerializer.Serialize(request.FieldDto.ValidationRules);
        
        if (request.FieldDto.Placeholder != null)
            field.Placeholder = request.FieldDto.Placeholder;

        field.UpdatedAt = DateTime.UtcNow;

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
