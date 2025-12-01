import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface ScheduleBlock {
  id: number;
  professionalId: number;
  professionalName: string;
  startDate: string;
  endDate: string;
  reason: string;
  status: string;
  adminJustification?: string;
  adminId?: number;
  adminName?: string;
  createdAt: string;
}

export interface RequestBlockPayload {
  startDate: string;
  endDate: string;
  reason: string;
}

export interface AdminRespondPayload {
  justification: string;
}

@Injectable({ providedIn: 'root' })
export class ScheduleBlocksService {
  private baseUrl = '/api/scheduleblocks';

  constructor(private http: HttpClient) {}

  getMyBlocks(): Observable<ScheduleBlock[]> {
    return this.http.get<ScheduleBlock[]>(`${this.baseUrl}/my`);
  }

  requestBlock(payload: RequestBlockPayload): Observable<ScheduleBlock> {
    return this.http.post<ScheduleBlock>(this.baseUrl, payload);
  }

  getAllBlocks(): Observable<ScheduleBlock[]> {
    return this.http.get<ScheduleBlock[]>(this.baseUrl);
  }

  acceptBlock(id: number, justification: string): Observable<ScheduleBlock> {
    return this.http.post<ScheduleBlock>(`${this.baseUrl}/${id}/accept`, { justification });
  }

  rejectBlock(id: number, justification: string): Observable<ScheduleBlock> {
    return this.http.post<ScheduleBlock>(`${this.baseUrl}/${id}/reject`, { justification });
  }
}
