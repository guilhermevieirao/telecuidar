import { TestBed } from '@angular/core/testing';
import { SpecialtiesService, SpecialtyStatus } from './specialties.service';
import { firstValueFrom } from 'rxjs';

describe('SpecialtiesService', () => {
  let service: SpecialtiesService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(SpecialtiesService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  describe('getSpecialties', () => {
    it('should return paginated specialties', async () => {
      const result = await firstValueFrom(service.getSpecialties({}, undefined, 1, 10));
      
      expect(result).toBeDefined();
      expect(result.data).toBeInstanceOf(Array);
      expect(result.total).toBeGreaterThan(0);
      expect(result.page).toBe(1);
      expect(result.pageSize).toBe(10);
    });

    it('should filter by status', async () => {
      const result = await firstValueFrom(
        service.getSpecialties({ status: 'Active' }, undefined, 1, 10)
      );
      
      expect(result.data.every(s => s.status === 'Active')).toBe(true);
    });

    it('should search by name', async () => {
      const result = await firstValueFrom(
        service.getSpecialties({ search: 'cardio' }, undefined, 1, 10)
      );
      
      expect(result.data.length).toBeGreaterThan(0);
      expect(result.data.some(s => 
        s.name.toLowerCase().includes('cardio')
      )).toBe(true);
    });
  });

  describe('getSpecialtyById', () => {
    it('should return specialty by id', async () => {
      const specialty = await firstValueFrom(service.getSpecialtyById('1'));
      
      expect(specialty).toBeDefined();
      expect(specialty?.id).toBe('1');
      expect(specialty?.name).toBe('Cardiologia');
    });

    it('should return undefined for non-existent id', async () => {
      const specialty = await firstValueFrom(service.getSpecialtyById('999999'));
      
      expect(specialty).toBeUndefined();
    });
  });

  describe('createSpecialty', () => {
    it('should create a new specialty', async () => {
      const newSpecialty = {
        name: 'Nova Especialidade',
        description: 'Descrição da nova especialidade',
        status: 'Active' as SpecialtyStatus
      };

      const created = await firstValueFrom(service.createSpecialty(newSpecialty));
      
      expect(created).toBeDefined();
      expect(created.id).toBeDefined();
      expect(created.name).toBe(newSpecialty.name);
    });
  });

  describe('updateSpecialty', () => {
    it('should update an existing specialty', async () => {
      const updates = { name: 'Nome Atualizado' };
      
      const updated = await firstValueFrom(service.updateSpecialty('1', updates));
      
      expect(updated).toBeDefined();
      expect(updated.name).toBe('Nome Atualizado');
    });
  });

  describe('deleteSpecialty', () => {
    it('should delete a specialty', async () => {
      const result = await firstValueFrom(service.deleteSpecialty('1'));
      
      expect(result).toBeUndefined();
    });
  });
});
