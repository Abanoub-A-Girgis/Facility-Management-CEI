import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, map } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse, AssignWorkerRequest, Issue, LookupValue, UpdateIssueRequest } from '../models/api.models';

@Injectable({ providedIn: 'root' })
export class IssuesService {
  private readonly baseUrl = `${environment.apiBaseUrl}/api/issues`;

  constructor(private readonly http: HttpClient) {}

  getSeverities(): Observable<LookupValue[]> {
    return this.http.get<ApiResponse<LookupValue[]>>(`${this.baseUrl}/severities`).pipe(map(response => response.data));
  }

  getStatuses(): Observable<LookupValue[]> {
    return this.http.get<ApiResponse<LookupValue[]>>(`${this.baseUrl}/statuses`).pipe(map(response => response.data));
  }

  updateIssue(issueId: number, request: UpdateIssueRequest): Observable<Issue> {
    return this.http.put<ApiResponse<Issue>>(`${this.baseUrl}/${issueId}`, request).pipe(map(response => response.data));
  }

  deleteIssue(issueId: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${issueId}`);
  }

  assignWorker(issueId: number, request: AssignWorkerRequest): Observable<Issue> {
    return this.http.put<ApiResponse<Issue>>(`${this.baseUrl}/${issueId}/assign-worker`, request).pipe(map(response => response.data));
  }
}
