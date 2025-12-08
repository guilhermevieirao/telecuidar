export interface AppointmentFieldValueDto {
  id: number;
  appointmentId: number;
  specialtyFieldId: number;
  fieldValue: string;
  createdAt?: Date;
  updatedAt?: Date;
}

export interface SaveAppointmentFieldValueDto {
  specialtyFieldId: number;
  fieldValue: string;
}
