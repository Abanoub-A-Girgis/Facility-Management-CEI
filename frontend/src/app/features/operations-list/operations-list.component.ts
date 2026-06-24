import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Observable } from 'rxjs';
import { OperationsService } from '../../core/services/operations.service';
import { TranslatePipe } from '../../core/pipes/translate.pipe';

type Row = Record<string, unknown>;

@Component({
  selector: 'app-operations-list',
  standalone: true,
  imports: [CommonModule, TranslatePipe],
  templateUrl: './operations-list.component.html',
  styleUrl: './operations-list.component.css'
})
export class OperationsListComponent implements OnInit {
  title = '';
  description = '';
  columns: { key: string; label: string }[] = [];
  rows: Row[] = [];
  loading = true;
  error = '';

  constructor(private readonly route: ActivatedRoute, private readonly operations: OperationsService) {}

  ngOnInit(): void {
    const type = this.route.snapshot.data['type'] as string;
    const config = this.config(type);
    this.title = config.title;
    this.description = config.description;
    this.columns = config.columns;
    (config.load() as Observable<unknown[]>).subscribe({
      next: rows => {
        this.rows = rows as Row[];
        this.loading = false;
      },
      error: () => {
        this.error = 'Operations.ListLoadError';
        this.loading = false;
      }
    });
  }

  display(row: Row, key: string): string {
    const value = row[key];
    if (typeof value === 'boolean') return value ? 'Yes' : 'No';
    if (key.toLowerCase().includes('date') || key.toLowerCase().includes('time')) {
      return value ? new Date(String(value)).toLocaleString() : '-';
    }
    return value == null || value === '' ? '-' : String(value);
  }

  private config(type: string) {
    if (type === 'incidents') {
      return {
        title: 'Nav.Incidents',
        description: 'Operations.IncidentsDescription',
        columns: [
          { key: 'id', label: 'Element.Id' }, { key: 'description', label: 'Issues.Description' },
          { key: 'elementName', label: 'Operations.Element' }, { key: 'spaceName', label: 'Element.Space' },
          { key: 'severity', label: 'Issues.Severity' }, { key: 'status', label: 'Issues.Status' },
          { key: 'assignedWorkerName', label: 'Issues.AssignedTo' }
        ],
        load: () => this.operations.getIncidents()
      };
    }
    if (type === 'work-orders') {
      return {
        title: 'Nav.WorkOrders',
        description: 'Operations.WorkOrdersDescription',
        columns: [
          { key: 'id', label: 'Element.Id' }, { key: 'description', label: 'Issues.Description' },
          { key: 'type', label: 'Element.Type' }, { key: 'priority', label: 'Operations.Priority' },
          { key: 'status', label: 'Issues.Status' }, { key: 'assignedTo', label: 'Issues.AssignedTo' },
          { key: 'fixingTime', label: 'Operations.FixingTime' }
        ],
        load: () => this.operations.getWorkOrders()
      };
    }
    if (type === 'sensor-warnings') {
      return {
        title: 'Nav.SensorWarnings',
        description: 'Operations.WarningsDescription',
        columns: [
          { key: 'id', label: 'Element.Id' }, { key: 'issueDate', label: 'Operations.Issued' },
          { key: 'description', label: 'Issues.Description' }, { key: 'priority', label: 'Operations.Priority' },
          { key: 'sensorName', label: 'Operations.Sensor' }, { key: 'spaceName', label: 'Element.Space' },
          { key: 'investigated', label: 'Operations.Investigated' }
        ],
        load: () => this.operations.getSensorWarnings()
      };
    }
    return {
      title: 'Nav.BuildingInformation',
      description: 'Operations.BuildingsDescription',
      columns: [
        { key: 'id', label: 'Element.Id' }, { key: 'name', label: 'Operations.Building' },
        { key: 'floorCount', label: 'Operations.Floors' }, { key: 'spaceCount', label: 'Operations.Spaces' },
        { key: 'assetCount', label: 'Operations.Assets' }, { key: 'sensorCount', label: 'Operations.Sensors' },
        { key: 'openIssueCount', label: 'Operations.OpenIssues' }
      ],
      load: () => this.operations.getBuildings()
    };
  }
}
