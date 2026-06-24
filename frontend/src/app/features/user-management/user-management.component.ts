import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { BuildingModel, CreateUserRequest, UserListItem } from '../../core/models/api.models';
import { ModelsService } from '../../core/services/models.service';
import { OperationsService } from '../../core/services/operations.service';
import { TranslatePipe } from '../../core/pipes/translate.pipe';

@Component({
  selector: 'app-user-management',
  standalone: true,
  imports: [CommonModule, FormsModule, TranslatePipe],
  templateUrl: './user-management.component.html',
  styleUrl: './user-management.component.css'
})
export class UserManagementComponent implements OnInit {
  users: UserListItem[] = [];
  buildings: BuildingModel[] = [];
  saving = false;
  error = '';
  message = '';
  readonly roles = ['AccountManager', 'Owner', 'Manager', 'Supervisor', 'Inspector', 'Agent'];
  form: CreateUserRequest = { userName: '', password: '', firstName: '', lastName: '', role: 'Agent' };

  constructor(private readonly operations: OperationsService, private readonly models: ModelsService) {}

  ngOnInit(): void {
    this.loadUsers();
    this.models.getModels().subscribe(models => (this.buildings = models));
  }

  createUser(): void {
    this.saving = true;
    this.error = '';
    this.message = '';
    this.operations.createUser(this.form).subscribe({
      next: user => {
        this.users = [...this.users, user];
        this.form = { userName: '', password: '', firstName: '', lastName: '', role: 'Agent' };
        this.message = 'Users.Created';
        this.saving = false;
      },
      error: error => {
        this.error = error.error?.message ?? 'Users.CreateError';
        this.saving = false;
      }
    });
  }

  private loadUsers(): void {
    this.operations.getUsers().subscribe({
      next: users => (this.users = users),
      error: () => (this.error = 'Users.LoadError')
    });
  }
}
