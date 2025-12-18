import { TestBed } from '@angular/core/testing';
import { AppointmentsService, AppointmentStatus } from './appointments.service';
import { firstValueFrom } from 'rxjs';

describe('AppointmentsService', () => {
  let service: AppointmentsService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(AppointmentsService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  describe('getAppointments', () => {
    it('should return all appointments', async () => {
      const result = await firstValueFrom(service.getAppointments());
      
      expect(result).toBeDefined();
      expect(result.data).toBeInstanceOf(Array);
      expect(result.data.length).toBeGreaterThan(0);
    });

    it('should filter by status', async () => {
      const result = await firstValueFrom(
        service.getAppointments({ status: 'Scheduled' })
      );
      
      expect(result.data.every((a: any) => a.status === 'Scheduled')).toBe(true);
    });

    it('should filter by search term', async () => {
      const result = await firstValueFrom(
        service.getAppointments({ search: 'joão' })
      );
      
      expect(result.data.length).toBeGreaterThan(0);
      expect(result.data.some((a: any) => 
        a.patientName.toLowerCase().includes('joão') ||
        a.professionalName.toLowerCase().includes('joão')
      )).toBe(true);
    });

    it('should filter by date range', async () => {
      const startDate = new Date().toISOString().split('T')[0];
      const result = await firstValueFrom(
        service.getAppointments({ startDate })
      );
      
      expect(result).toBeDefined();
    });
  });

  describe('getAppointmentById', () => {
    it('should return appointment by id', async () => {
      const appointment = await firstValueFrom(service.getAppointmentById('1'));
      
      expect(appointment).toBeDefined();
      expect(appointment?.id).toBe('1');
    });

    it('should return undefined for non-existent id', async () => {
      const appointment = await firstValueFrom(service.getAppointmentById('999999'));
      
      expect(appointment).toBeUndefined();
    });
  });

  describe('cancelAppointment', () => {
    it('should cancel an appointment', async () => {
      const result = await firstValueFrom(service.cancelAppointment('1'));
      
      expect(result).toBe(true);
    });
  });

  describe('completeAppointment', () => {
    it('should complete an appointment', async () => {
      const result = await firstValueFrom(
        service.completeAppointment('1', 'Consulta finalizada')
      );
      
      expect(result).toBe(true);
    });
  });
});
