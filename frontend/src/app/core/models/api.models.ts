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
