using MediatR;
using Microsoft.EntityFrameworkCore;
using app.Application.Common.Interfaces;
using app.Application.Appointments.DTOs;

namespace app.Application.Appointments.Queries;

public class GetAppointmentFieldValuesQueryHandler : IRequestHandler<GetAppointmentFieldValuesQuery, List<AppointmentFieldValueDto>>
{
    private readonly IApplicationDbContext _context;

    public GetAppointmentFieldValuesQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<AppointmentFieldValueDto>> Handle(GetAppointmentFieldValuesQuery request, CancellationToken cancellationToken)
    {
        var values = await _context.AppointmentFieldValues
            .Include(v => v.SpecialtyField)
            .Where(v => v.AppointmentId == request.AppointmentId && v.IsActive)
            .Select(v => new AppointmentFieldValueDto
            {
                Id = v.Id,
                AppointmentId = v.AppointmentId,
                SpecialtyFieldId = v.SpecialtyFieldId,
                FieldValue = v.FieldValue,
                FieldName = v.SpecialtyField.FieldName,
                Label = v.SpecialtyField.Label,
                FieldType = v.SpecialtyField.FieldType
            })
            .ToListAsync(cancellationToken);

        return values;
    }
}
