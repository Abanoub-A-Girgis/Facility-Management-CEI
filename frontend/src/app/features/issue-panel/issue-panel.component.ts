import { CommonModule } from '@angular/common';
import { Component, Input, OnChanges, SimpleChanges } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { BuildingElement, Issue, LookupValue, Worker } from '../../core/models/api.models';
import { TranslatePipe } from '../../core/pipes/translate.pipe';
import { ElementsService } from '../../core/services/elements.service';
import { IssuesService } from '../../core/services/issues.service';
import { LocalizationService } from '../../core/services/localization.service';
import { WorkersService } from '../../core/services/workers.service';

@Component({
  selector: 'app-issue-panel',
  standalone: true,
  imports: [CommonModule, FormsModule, TranslatePipe],
  templateUrl: './issue-panel.component.html',
  styleUrl: './issue-panel.component.css'
})
export class IssuePanelComponent implements OnChanges {
  @Input() selectedElement?: BuildingElement;
  @Input() modelId?: number;

  issues: Issue[] = [];
  severities: LookupValue[] = [];
  workers: Worker[] = [];
  loading = false;
  saving = false;
  error = '';
  message = '';

  newIssue = {
    description: '',
    severity: 0,
    reporterId: 1,
    spaceId: undefined as number | undefined
  };

  selectedWorkerByIssueId: Record<number, number | undefined> = {};

  constructor(
    private readonly elementsService: ElementsService,
    private readonly issuesService: IssuesService,
    private readonly localization: LocalizationService,
    private readonly workersService: WorkersService
  ) {
    this.issuesService.getSeverities().subscribe(values => {
      this.severities = values;
      this.newIssue.severity = values[0]?.id ?? 0;
    });
    this.workersService.getWorkers().subscribe(workers => (this.workers = workers));
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['selectedElement']) {
      this.loadIssues();
      this.newIssue.spaceId = this.selectedElement?.spaceId;
    }
  }

  loadIssues(): void {
    if (!this.selectedElement) {
      this.issues = [];
      return;
    }

    this.loading = true;
    this.error = '';
    this.elementsService.getIssues(this.selectedElement.id).subscribe({
      next: issues => {
        this.issues = issues;
        this.loading = false;
      },
      error: () => {
        this.error = this.localization.translate('Issues.LoadError');
        this.loading = false;
      }
    });
  }

  createIssue(): void {
    if (!this.selectedElement || !this.newIssue.description.trim()) {
      this.error = this.localization.translate('Issues.DescriptionRequired');
      return;
    }

    this.saving = true;
    this.error = '';
    this.message = '';
    this.elementsService
      .createIssue(this.selectedElement.id, {
        description: this.newIssue.description.trim(),
        severity: Number(this.newIssue.severity),
        reporterId: Number(this.newIssue.reporterId),
        spaceId: this.newIssue.spaceId ? Number(this.newIssue.spaceId) : undefined
      })
      .subscribe({
        next: issue => {
          this.issues = [issue, ...this.issues];
          this.newIssue.description = '';
          this.message = this.localization.translate('Issues.Created');
          this.saving = false;
        },
        error: err => {
          this.error = err.error?.message ?? this.localization.translate('Issues.CreateError');
          this.saving = false;
        }
      });
  }

  assignWorker(issue: Issue): void {
    const workerId = this.selectedWorkerByIssueId[issue.id] ?? issue.assignedWorkerId;
    if (!workerId) {
      this.error = this.localization.translate('Issues.WorkerRequired');
      return;
    }

    this.issuesService.assignWorker(issue.id, { workerId }).subscribe({
      next: updated => {
        this.issues = this.issues.map(item => (item.id === updated.id ? updated : item));
        this.message = this.localization.translate('Issues.Assigned');
        this.error = '';
      },
      error: err => {
        this.error = err.error?.message ?? this.localization.translate('Issues.AssignError');
      }
    });
  }

  severityName(id: number): string {
    const severity = this.severities.find(item => item.id === id);
    if (!severity) {
      return String(id);
    }

    return this.localization.translate(`Severity.${severity.name}`) !== `Severity.${severity.name}`
      ? this.localization.translate(`Severity.${severity.name}`)
      : severity.name;
  }
}
