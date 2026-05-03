import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface TimeSlot {
  startTime: string;
  endTime: string;
}

export interface ResourceWithSlots {
  resourceId: number;
  resourceName: string;
  resourceType: string;
  availableSlots: TimeSlot[];
}

export interface CreateReservationDto {
  resourceId: number;
  userId: number;
  startTime: string;
  endTime: string;
}

export interface ReservationDto {
  id: number;
  resourceId: number;
  resourceName: string;
  userId: number;
  userName: string;
  startTime: string;
  endTime: string;
  status: string;
}

@Injectable({ providedIn: 'root' })
export class ReservationService {
  private api = 'http://localhost:5190/api';

  constructor(private http: HttpClient) {}

  getResourcesWithSlots(): Observable<ResourceWithSlots[]> {
    return this.http.get<ResourceWithSlots[]>(`${this.api}/resources/available-slots`);
  }

  createReservation(dto: CreateReservationDto): Observable<ReservationDto> {
    return this.http.post<ReservationDto>(`${this.api}/reservations`, dto);
  }

  cancelReservation(id: number): Observable<void> {
    return this.http.patch<void>(`${this.api}/reservations/${id}/cancel`, {});
  }

  getReport(resourceId: number, from: string, to: string): Observable<ReservationDto[]> {
    return this.http.get<ReservationDto[]>(
      `${this.api}/reservations/report?resourceId=${resourceId}&from=${from}&to=${to}`
    );
  }
}