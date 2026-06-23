import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, map } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse, Worker } from '../models/api.models';

@Injectable({ providedIn: 'root' })
export class WorkersService {
  private readonly baseUrl = `${environment.apiBaseUrl}/api/workers`;

  constructor(private readonly http: HttpClient) {}

  getWorkers(buildingId?: number): Observable<Worker[]> {
    let params = new HttpParams();
    if (buildingId) {
      params = params.set('buildingId', buildingId);
    }
    return this.http.get<ApiResponse<Worker[]>>(this.baseUrl, { params }).pipe(map(response => response.data));
  }
}
