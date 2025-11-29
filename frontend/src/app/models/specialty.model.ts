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
