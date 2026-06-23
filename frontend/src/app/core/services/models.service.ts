import { HttpClient, HttpEvent } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, map } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse, BuildingElement, BuildingModel, Floor, ModelUploadResponse, ViewerHighlight } from '../models/api.models';

@Injectable({ providedIn: 'root' })
export class ModelsService {
  private readonly baseUrl = `${environment.apiBaseUrl}/api/models`;

  constructor(private readonly http: HttpClient) {}

  upload(file: File): Observable<HttpEvent<ApiResponse<ModelUploadResponse>>> {
    const formData = new FormData();
    formData.append('file', file);
    return this.http.post<ApiResponse<ModelUploadResponse>>(`${this.baseUrl}/upload`, formData, {
      observe: 'events',
      reportProgress: true
    });
  }

  getModels(): Observable<BuildingModel[]> {
    return this.http.get<ApiResponse<BuildingModel[]>>(this.baseUrl).pipe(map(response => response.data));
  }

  getModel(modelId: number): Observable<BuildingModel> {
    return this.http.get<ApiResponse<BuildingModel>>(`${this.baseUrl}/${modelId}`).pipe(map(response => response.data));
  }

  getFloors(modelId: number): Observable<Floor[]> {
    return this.http.get<ApiResponse<Floor[]>>(`${this.baseUrl}/${modelId}/floors`).pipe(map(response => response.data));
  }

  getElements(modelId: number): Observable<BuildingElement[]> {
    return this.http.get<ApiResponse<BuildingElement[]>>(`${this.baseUrl}/${modelId}/elements`).pipe(map(response => response.data));
  }

  getFloorElements(modelId: number, floorId: number): Observable<BuildingElement[]> {
    return this.http.get<ApiResponse<BuildingElement[]>>(`${this.baseUrl}/${modelId}/floors/${floorId}/elements`).pipe(map(response => response.data));
  }

  getViewerHighlights(modelId: number): Observable<ViewerHighlight> {
    return this.http.get<ApiResponse<ViewerHighlight>>(`${this.baseUrl}/${modelId}/viewer-highlights`).pipe(map(response => response.data));
  }
}
