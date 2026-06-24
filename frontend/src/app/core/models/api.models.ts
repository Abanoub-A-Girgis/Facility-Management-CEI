export interface ApiResponse<T> {
  success: boolean;
  message?: string;
  data: T;
}

export interface LocalizationPayload {
  culture: string;
  direction: 'ltr' | 'rtl';
  translations: Record<string, string>;
}

export interface LoginRequest {
  userName: string;
  password: string;
}

export interface AuthUser {
  id: string;
  appUserId?: number;
  userName: string;
  firstName?: string;
  lastName?: string;
  displayName: string;
  roles: string[];
}

export interface BuildingModel {
  id: number;
  name: string;
  path: string;
  floorCount: number;
  elementCount: number;
  openIssueCount: number;
}

export interface ModelUploadResponse {
  modelId?: number;
  fileName: string;
  relativePath: string;
  generatedIfcFiles: string[];
  generatedWexBimPaths: string[];
}

export interface Floor {
  id: number;
  modelId: number;
  name: string;
  wexBimPath: string;
}

export interface BuildingElement {
    id: number;
    viewerId: number;
  elementKey?: string;
  elementType: 'Asset' | 'Space' | string;
  name: string;
  materials?: string;
  floorId: number;
  floorName: string;
  spaceId?: number;
  spaceName?: string;
  openIssueCount: number;
}

export interface ViewerHighlight {
  severe: number[];
  high: number[];
  medium: number[];
  low: number[];
  rooms: number[];
  severityColors?: Record<string, number[]>;
  flashIntervalMs?: number;
}

export interface Issue {
  id: number;
  status: number;
  severity: number;
  description: string;
  comment?: string;
  reportingTime: string;
  elementId?: number;
  elementName?: string;
  spaceId: number;
  spaceName?: string;
  reporterId: number;
  reporterName?: string;
  assignedWorkerId?: number;
  assignedWorkerName?: string;
}

export interface CreateIssueRequest {
  description: string;
  severity: number;
  reporterId: number;
  spaceId?: number;
  comment?: string;
}

export interface UpdateIssueRequest {
  description: string;
  severity: number;
  status: number;
  comment?: string;
}

export interface AssignWorkerRequest {
  workerId: number;
  assignedById?: number;
}

export interface Worker {
  id: number;
  firstName: string;
  lastName: string;
  fullName: string;
  type: string;
  buildingId?: number;
}

export interface LookupValue {
  id: number;
  name: string;
}

export interface DashboardSummary {
  modelCount: number;
  floorCount: number;
  elementCount: number;
  openIssueCount: number;
  workerCount: number;
  openWorkOrderCount: number;
  uninvestigatedWarningCount: number;
}

export interface WorkOrder {
  id: number;
  type: string;
  status: string;
  priority: string;
  description: string;
  incidentId: number;
  assignedTo?: string;
  createdBy?: string;
  fixingTime?: string;
  cost?: number;
}

export interface SensorWarning {
  id: number;
  issueDate: string;
  description: string;
  investigated: boolean;
  priority: string;
  sensorName?: string;
  spaceName?: string;
  assignedUser?: string;
}

export interface BuildingInformation {
  id: number;
  name: string;
  path: string;
  floorCount: number;
  spaceCount: number;
  assetCount: number;
  sensorCount: number;
  openIssueCount: number;
}

export interface UserListItem {
  appUserId?: number;
  identityUserId: string;
  userName: string;
  fullName: string;
  role: string;
  buildingId?: number;
}

export interface CreateUserRequest {
  userName: string;
  password: string;
  firstName: string;
  lastName: string;
  role: string;
  buildingId?: number;
  supervisorId?: number;
}

export interface OperationsReport {
  summary: DashboardSummary;
  incidents: Issue[];
  workOrders: WorkOrder[];
  sensorWarnings: SensorWarning[];
  buildings: BuildingInformation[];
}
