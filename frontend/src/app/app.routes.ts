import { Routes } from '@angular/router';
import { AuthorizationComponent } from './features/authorization/authorization.component';
import { authGuard, guestGuard, roleGuard } from './core/services/auth.guard';
import { ModelUploadComponent } from './features/model-upload/model-upload.component';
import { ModelViewerComponent } from './features/model-viewer/model-viewer.component';
import { OperationsDashboardComponent } from './features/operations-dashboard/operations-dashboard.component';
import { OperationsListComponent } from './features/operations-list/operations-list.component';
import { UserManagementComponent } from './features/user-management/user-management.component';

export const routes: Routes = [
  { path: '', redirectTo: 'login', pathMatch: 'full' },
  { path: 'login', component: AuthorizationComponent, canActivate: [guestGuard] },
  { path: 'authorization', redirectTo: 'login', pathMatch: 'full' },
  { path: 'home', component: OperationsDashboardComponent, canActivate: [authGuard] },
  { path: 'models/upload', component: ModelUploadComponent, canActivate: [authGuard] },
  { path: 'models/:modelId/viewer', component: ModelViewerComponent, canActivate: [authGuard] },
  { path: 'viewer', component: ModelViewerComponent, canActivate: [authGuard] },
  {
    path: 'users',
    component: UserManagementComponent,
    canActivate: [authGuard, roleGuard],
    data: { roles: ['AccountManager'] }
  },
  {
    path: 'sensor-warnings',
    component: OperationsListComponent,
    canActivate: [authGuard, roleGuard],
    data: { type: 'sensor-warnings', roles: ['AccountManager', 'Manager', 'Supervisor', 'Inspector'] }
  },
  {
    path: 'incidents',
    component: OperationsListComponent,
    canActivate: [authGuard, roleGuard],
    data: { type: 'incidents', roles: ['AccountManager', 'Manager', 'Supervisor', 'Inspector'] }
  },
  {
    path: 'work-orders',
    component: OperationsListComponent,
    canActivate: [authGuard, roleGuard],
    data: { type: 'work-orders', roles: ['AccountManager', 'Manager', 'Supervisor', 'Inspector', 'Agent'] }
  },
  {
    path: 'reports',
    component: OperationsDashboardComponent,
    canActivate: [authGuard, roleGuard],
    data: { roles: ['AccountManager', 'Owner', 'Manager'] }
  },
  {
    path: 'building-information',
    component: OperationsListComponent,
    canActivate: [authGuard, roleGuard],
    data: { type: 'buildings', roles: ['AccountManager', 'Owner'] }
  },
  { path: '**', redirectTo: 'login' }
];
