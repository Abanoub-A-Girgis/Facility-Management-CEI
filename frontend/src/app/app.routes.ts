import { Routes } from '@angular/router';
import { ModelUploadComponent } from './features/model-upload/model-upload.component';
import { ModelViewerComponent } from './features/model-viewer/model-viewer.component';

export const routes: Routes = [
  { path: '', redirectTo: 'models/upload', pathMatch: 'full' },
  { path: 'models/upload', component: ModelUploadComponent },
  { path: 'models/:modelId/viewer', component: ModelViewerComponent },
  { path: 'viewer', component: ModelViewerComponent },
  { path: '**', redirectTo: 'models/upload' }
];
