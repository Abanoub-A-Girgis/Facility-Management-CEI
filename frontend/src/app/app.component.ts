import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { TranslatePipe } from './core/pipes/translate.pipe';
import { AuthService } from './core/services/auth.service';
import { LocalizationService } from './core/services/localization.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink, RouterLinkActive, RouterOutlet, TranslatePipe],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent {
  title = 'Facility Management CEI';

  constructor(
    readonly localization: LocalizationService,
    readonly authService: AuthService,
    private readonly router: Router
  ) {
    this.authService.loadCurrentUser().subscribe();
  }

  setCulture(culture: string): void {
    this.localization.setCulture(culture);
  }

  logout(): void {
    this.authService.logout().subscribe(() => this.router.navigate(['/authorization']));
  }
}
