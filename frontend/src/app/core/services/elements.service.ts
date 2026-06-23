import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, map } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse, BuildingElement, CreateIssueRequest, Issue } from '../models/api.models';

@Injectable({ providedIn: 'root' })
export class ElementsService {
  private readonly baseUrl = `${environment.apiBaseUrl}/api/elements`;

  constructor(private readonly http: HttpClient) {}

  getElement(elementId: number): Observable<BuildingElement> {
    return this.http.get<ApiResponse<BuildingElement>>(`${this.baseUrl}/${elementId}`).pipe(map(response => response.data));
  }

  getIssues(elementId: number): Observable<Issue[]> {
    return this.http.get<ApiResponse<Issue[]>>(`${this.baseUrl}/${elementId}/issues`).pipe(map(response => response.data));
  }

  createIssue(elementId: number, request: CreateIssueRequest): Observable<Issue> {
    return this.http.post<ApiResponse<Issue>>(`${this.baseUrl}/${elementId}/issues`, request).pipe(map(response => response.data));
  }
}
