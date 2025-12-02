namespace app.Application.Appointments.DTOs;

public class AppointmentFieldValueDto
{
    public int Id { get; set; }
    public int AppointmentId { get; set; }
    public int SpecialtyFieldId { get; set; }
    public string FieldValue { get; set; } = string.Empty;
    public string? FieldName { get; set; }
    public string? Label { get; set; }
    public string? FieldType { get; set; }
}

public class SaveAppointmentFieldValueDto
{
    public int SpecialtyFieldId { get; set; }
    public string FieldValue { get; set; } = string.Empty;
}

public class BulkSaveAppointmentFieldValuesDto
{
    public int AppointmentId { get; set; }
    public List<SaveAppointmentFieldValueDto> FieldValues { get; set; } = new();
}
