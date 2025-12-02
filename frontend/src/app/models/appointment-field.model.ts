export interface AppointmentFieldValueDto {
  id: number;
  appointmentId: number;
  specialtyFieldId: number;
  fieldValue: string;
  fieldName?: string;
  label?: string;
  fieldType?: string;
}

export interface SaveAppointmentFieldValueDto {
  specialtyFieldId: number;
  fieldValue: string;
}
