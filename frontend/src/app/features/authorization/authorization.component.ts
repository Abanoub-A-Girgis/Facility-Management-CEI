import { CommonModule } from '@angular/common';
import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
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
export class AuthorizationComponent implements OnInit, OnDestroy {
  readonly slides = [
    '/assets/login/carousel/image1.jpg',
    '/assets/login/carousel/image2.jpg',
    '/assets/login/carousel/image3.jpg'
  ];
  readonly logoUrl = '/assets/login/logo-innove.png';

  userName = '';
  password = '';
  loading = false;
  error = '';
  activeSlide = 0;
  private rotationTimer?: number;

  constructor(
    private readonly authService: AuthService,
    private readonly localization: LocalizationService,
    private readonly router: Router
  ) {}

  ngOnInit(): void {
    this.startRotation();
  }

  ngOnDestroy(): void {
    this.stopRotation();
  }

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
        this.router.navigate(['/viewer']);
      },
      error: err => {
        this.loading = false;
        this.error = err.error?.message ?? this.localization.translate('Authorization.InvalidLogin');
      }
    });
  }

  setSlide(index: number): void {
    this.activeSlide = index;
    this.startRotation();
  }

  pauseRotation(): void {
    this.stopRotation();
  }

  resumeRotation(): void {
    this.startRotation();
  }

  private startRotation(): void {
    this.stopRotation();
    this.rotationTimer = window.setInterval(() => {
      this.activeSlide = (this.activeSlide + 1) % this.slides.length;
    }, 5000);
  }

  private stopRotation(): void {
    if (this.rotationTimer) {
      window.clearInterval(this.rotationTimer);
      this.rotationTimer = undefined;
    }
  }
}
