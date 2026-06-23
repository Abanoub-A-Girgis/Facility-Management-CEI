import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Floor } from '../models/api.models';
import { ModelsService } from './models.service';

@Injectable({ providedIn: 'root' })
export class FloorsService {
  constructor(private readonly modelsService: ModelsService) {}

  getByModel(modelId: number): Observable<Floor[]> {
    return this.modelsService.getFloors(modelId);
  }
}
