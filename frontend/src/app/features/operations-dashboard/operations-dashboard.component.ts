import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { map } from 'rxjs';
import { DashboardSummary } from '../../core/models/api.models';
import { TranslatePipe } from '../../core/pipes/translate.pipe';
import { OperationsService } from '../../core/services/operations.service';

@Component({
  selector: 'app-operations-dashboard',
  standalone: true,
  imports: [CommonModule, TranslatePipe],
  templateUrl: './operations-dashboard.component.html',
  styleUrl: './operations-dashboard.component.css'
})
export class OperationsDashboardComponent implements OnInit {
  summary?: DashboardSummary;
  loading = true;
  error = '';
  titleKey = 'Operations.Home';
  descriptionKey = 'Operations.HomeDescription';

  constructor(private readonly operations: OperationsService, private readonly route: ActivatedRoute) {}

  ngOnInit(): void {
    const isReport = this.route.snapshot.routeConfig?.path === 'reports';
    this.titleKey = isReport ? 'Operations.Reports' : 'Operations.Home';
    this.descriptionKey = isReport ? 'Operations.ReportsDescription' : 'Operations.HomeDescription';
    const request = isReport
      ? this.operations.getReport().pipe(map(report => report.summary))
      : this.operations.getDashboard();

    request.subscribe({
      next: summary => {
        this.summary = summary;
        this.loading = false;
      },
      error: () => {
        this.error = 'Operations.LoadError';
        this.loading = false;
      }
    });
  }
}
