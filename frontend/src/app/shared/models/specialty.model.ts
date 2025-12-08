export interface SpecialtyDto {
  id: number;
  name: string;
  description?: string;
  icon?: string;
  professionalsCount: number;
  isActive: boolean;
  createdAt: Date;
}

export interface CreateSpecialtyCommand {
  name: string;
  description?: string;
  icon?: string;
}

export interface UpdateSpecialtyCommand {
  id: number;
  name: string;
  description?: string;
  icon?: string;
}

export interface SpecialtyFieldDto {
  id: number;
  specialtyId: number;
  fieldName: string;
  label: string;
  description?: string;
  fieldType: string; // text, textarea, number, date, select, checkbox, radio
  options?: string[];
  isRequired: boolean;
  displayOrder: number;
  defaultValue?: string;
  validationRules?: { [key: string]: any };
  placeholder?: string;
}

export interface CreateSpecialtyFieldDto {
  fieldName: string;
  label: string;
  description?: string;
  fieldType: string;
  options?: string[];
  isRequired: boolean;
  displayOrder: number;
  defaultValue?: string;
  validationRules?: { [key: string]: any };
  placeholder?: string;
}

export interface UpdateSpecialtyFieldDto {
  fieldName?: string;
  label?: string;
  description?: string;
  fieldType?: string;
  options?: string[];
  isRequired?: boolean;
  displayOrder?: number;
  defaultValue?: string;
  validationRules?: { [key: string]: any };
  placeholder?: string;
}

