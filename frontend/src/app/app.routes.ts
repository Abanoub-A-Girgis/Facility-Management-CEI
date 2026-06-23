import { Routes } from '@angular/router';
import { AuthorizationComponent } from './features/authorization/authorization.component';
import { authGuard } from './core/services/auth.guard';
import { ModelUploadComponent } from './features/model-upload/model-upload.component';
import { ModelViewerComponent } from './features/model-viewer/model-viewer.component';

export const routes: Routes = [
  { path: '', redirectTo: 'authorization', pathMatch: 'full' },
  { path: 'authorization', component: AuthorizationComponent },
  { path: 'models/upload', component: ModelUploadComponent, canActivate: [authGuard] },
  { path: 'models/:modelId/viewer', component: ModelViewerComponent, canActivate: [authGuard] },
  { path: 'viewer', component: ModelViewerComponent, canActivate: [authGuard] },
  { path: '**', redirectTo: 'authorization' }
];
