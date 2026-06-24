import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map, Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  ApiResponse,
  BuildingInformation,
  CreateUserRequest,
  DashboardSummary,
  Issue,
  OperationsReport,
  SensorWarning,
  UserListItem,
  WorkOrder
} from '../models/api.models';

@Injectable({ providedIn: 'root' })
export class OperationsService {
  private readonly baseUrl = `${environment.apiBaseUrl}/api/operations`;

  constructor(private readonly http: HttpClient) {}

  getDashboard(): Observable<DashboardSummary> {
    return this.get<DashboardSummary>('dashboard');
  }

  getIncidents(): Observable<Issue[]> {
    return this.get<Issue[]>('incidents');
  }

  getWorkOrders(): Observable<WorkOrder[]> {
    return this.get<WorkOrder[]>('work-orders');
  }

  getSensorWarnings(): Observable<SensorWarning[]> {
    return this.get<SensorWarning[]>('sensor-warnings');
  }

  getBuildings(): Observable<BuildingInformation[]> {
    return this.get<BuildingInformation[]>('buildings');
  }

  getReport(): Observable<OperationsReport> {
    return this.get<OperationsReport>('report');
  }

  getUsers(): Observable<UserListItem[]> {
    return this.get<UserListItem[]>('users');
  }

  createUser(request: CreateUserRequest): Observable<UserListItem> {
    return this.http
      .post<ApiResponse<UserListItem>>(`${this.baseUrl}/users`, request)
      .pipe(map(response => response.data));
  }

  private get<T>(path: string): Observable<T> {
    return this.http.get<ApiResponse<T>>(`${this.baseUrl}/${path}`).pipe(map(response => response.data));
  }
}
