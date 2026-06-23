import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { environment } from '../../../environments/environment';
import { TranslatePipe } from '../../core/pipes/translate.pipe';
import { AuthService } from '../../core/services/auth.service';
import { LocalizationService } from '../../core/services/localization.service';

@Component({
  selector: 'app-authorization',
  standalone: true,
  imports: [CommonModule, FormsModule, TranslatePipe],
  templateUrl: './authorization.component.html',
  styleUrl: './authorization.component.css'
})
export class AuthorizationComponent {
  readonly slides = [
    `${environment.apiBaseUrl}/assets/images/carousel/image1.jpg`,
    `${environment.apiBaseUrl}/assets/images/carousel/image2.jpg`,
    `${environment.apiBaseUrl}/assets/images/carousel/image3.jpg`
  ];
  readonly logoUrl = `${environment.apiBaseUrl}/assets/images/logo innove.png`;

  userName = '';
  password = '';
  loading = false;
  error = '';
  activeSlide = 0;

  constructor(
    private readonly authService: AuthService,
    private readonly localization: LocalizationService,
    private readonly router: Router
  ) {}

  login(): void {
    if (!this.userName.trim() || !this.password) {
      this.error = this.localization.translate('Authorization.Required');
      return;
    }

    this.loading = true;
    this.error = '';

    this.authService.login({ userName: this.userName.trim(), password: this.password }).subscribe({
      next: () => {
        this.loading = false;
        this.router.navigate(['/models/upload']);
      },
      error: err => {
        this.loading = false;
        this.error = err.error?.message ?? this.localization.translate('Authorization.InvalidLogin');
      }
    });
  }

  setSlide(index: number): void {
    this.activeSlide = index;
  }
}
