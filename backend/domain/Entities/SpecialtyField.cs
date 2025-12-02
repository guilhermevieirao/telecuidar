namespace app.Domain.Entities;

public class SpecialtyField : BaseEntity
{
    public int SpecialtyId { get; set; }
    public Specialty Specialty { get; set; } = null!;
    
    public string FieldName { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string FieldType { get; set; } = string.Empty; // text, textarea, number, date, select, checkbox, radio
    public string? Options { get; set; } // JSON para opções de select/radio (ex: ["Opção 1", "Opção 2"])
    public bool IsRequired { get; set; }
    public int DisplayOrder { get; set; }
    public string? DefaultValue { get; set; }
    public string? ValidationRules { get; set; } // JSON para regras de validação (ex: {"min": 0, "max": 100})
    public string? Placeholder { get; set; }
    
    // Navigation properties
    public ICollection<AppointmentFieldValue> AppointmentFieldValues { get; set; } = new List<AppointmentFieldValue>();
}
