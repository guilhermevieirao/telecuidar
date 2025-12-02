using MediatR;
using Microsoft.EntityFrameworkCore;
using app.Application.Common.Interfaces;
using app.Application.Appointments.DTOs;
using app.Domain.Entities;

namespace app.Application.Appointments.Commands;

public class SaveAppointmentFieldValuesCommandHandler : IRequestHandler<SaveAppointmentFieldValuesCommand, List<AppointmentFieldValueDto>>
{
    private readonly IApplicationDbContext _context;

    public SaveAppointmentFieldValuesCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<AppointmentFieldValueDto>> Handle(SaveAppointmentFieldValuesCommand request, CancellationToken cancellationToken)
    {
        var appointmentId = request.Dto.AppointmentId;

        // Buscar valores existentes
        var existingValues = await _context.AppointmentFieldValues
            .Where(v => v.AppointmentId == appointmentId)
            .ToListAsync(cancellationToken);

        var result = new List<AppointmentFieldValueDto>();

        foreach (var fieldValue in request.Dto.FieldValues)
        {
            var existing = existingValues.FirstOrDefault(v => v.SpecialtyFieldId == fieldValue.SpecialtyFieldId);

            if (existing != null)
            {
                // Atualizar valor existente
                existing.FieldValue = fieldValue.FieldValue;
                existing.UpdatedAt = DateTime.UtcNow;
            }
            else
            {
                // Criar novo valor
                var newValue = new AppointmentFieldValue
                {
                    AppointmentId = appointmentId,
                    SpecialtyFieldId = fieldValue.SpecialtyFieldId,
                    FieldValue = fieldValue.FieldValue,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                };
                _context.AppointmentFieldValues.Add(newValue);
                existingValues.Add(newValue);
            }
        }

        await _context.SaveChangesAsync(cancellationToken);

        // Buscar informações completas dos campos para retornar
        var fieldIds = request.Dto.FieldValues.Select(v => v.SpecialtyFieldId).ToList();
        var fields = await _context.SpecialtyFields
            .Where(f => fieldIds.Contains(f.Id))
            .ToListAsync(cancellationToken);

        foreach (var fieldValue in request.Dto.FieldValues)
        {
            var field = fields.FirstOrDefault(f => f.Id == fieldValue.SpecialtyFieldId);
            var savedValue = existingValues.FirstOrDefault(v => v.SpecialtyFieldId == fieldValue.SpecialtyFieldId);

            if (savedValue != null)
            {
                result.Add(new AppointmentFieldValueDto
                {
                    Id = savedValue.Id,
                    AppointmentId = savedValue.AppointmentId,
                    SpecialtyFieldId = savedValue.SpecialtyFieldId,
                    FieldValue = savedValue.FieldValue,
                    FieldName = field?.FieldName,
                    Label = field?.Label,
                    FieldType = field?.FieldType
                });
            }
        }

        return result;
    }
}
