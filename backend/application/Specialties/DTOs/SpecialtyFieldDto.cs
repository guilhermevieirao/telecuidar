namespace app.Application.Specialties.DTOs;

public class SpecialtyFieldDto
{
    public int Id { get; set; }
    public int SpecialtyId { get; set; }
    public string FieldName { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string FieldType { get; set; } = string.Empty; // text, textarea, number, date, select, checkbox, radio
    public List<string>? Options { get; set; } // Para select/radio
    public bool IsRequired { get; set; }
    public int DisplayOrder { get; set; }
    public string? DefaultValue { get; set; }
    public Dictionary<string, object>? ValidationRules { get; set; }
    public string? Placeholder { get; set; }
}

public class CreateSpecialtyFieldDto
{
    public string FieldName { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string FieldType { get; set; } = string.Empty;
    public List<string>? Options { get; set; }
    public bool IsRequired { get; set; }
    public int DisplayOrder { get; set; }
    public string? DefaultValue { get; set; }
    public Dictionary<string, object>? ValidationRules { get; set; }
    public string? Placeholder { get; set; }
}

public class UpdateSpecialtyFieldDto
{
    public string? FieldName { get; set; }
    public string? Label { get; set; }
    public string? Description { get; set; }
    public string? FieldType { get; set; }
    public List<string>? Options { get; set; }
    public bool? IsRequired { get; set; }
    public int? DisplayOrder { get; set; }
    public string? DefaultValue { get; set; }
    public Dictionary<string, object>? ValidationRules { get; set; }
    public string? Placeholder { get; set; }
}
