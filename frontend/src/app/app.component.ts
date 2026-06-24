import { CommonModule } from '@angular/common';
import { Component, ElementRef, HostListener, ViewChild } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import {
  LucideBarChart3,
  LucideBox,
  LucideBuilding2,
  LucideChevronDown,
  LucideClipboardCheck,
  LucideHome,
  LucideListChecks,
  LucideLogOut,
  LucideMenu,
  LucidePanelLeftClose,
  LucidePanelLeftOpen,
  LucideTriangleAlert,
  LucideUpload,
  LucideUser,
  LucideUserPlus,
  LucideX
} from '@lucide/angular';
import { TranslatePipe } from './core/pipes/translate.pipe';
import { AuthService } from './core/services/auth.service';
import { LocalizationService } from './core/services/localization.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    RouterLink,
    RouterLinkActive,
    RouterOutlet,
    TranslatePipe,
    LucideBarChart3,
    LucideBox,
    LucideBuilding2,
    LucideChevronDown,
    LucideClipboardCheck,
    LucideHome,
    LucideListChecks,
    LucideLogOut,
    LucideMenu,
    LucidePanelLeftClose,
    LucidePanelLeftOpen,
    LucideTriangleAlert,
    LucideUpload,
    LucideUser,
    LucideUserPlus,
    LucideX
  ],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent {
  title = 'Facility Management CEI';
  sidebarCollapsed = false;
  mobileSidebarOpen = false;
  accountMenuOpen = false;

  @ViewChild('accountMenu') accountMenu?: ElementRef<HTMLElement>;

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
    this.accountMenuOpen = false;
    this.authService.logout().subscribe(() => this.router.navigate(['/login']));
  }

  toggleAccountMenu(): void {
    this.accountMenuOpen = !this.accountMenuOpen;
  }

  @HostListener('document:click', ['$event'])
  closeAccountMenuOnOutsideClick(event: MouseEvent): void {
    if (this.accountMenuOpen && !this.accountMenu?.nativeElement.contains(event.target as Node)) {
      this.accountMenuOpen = false;
    }
  }

  @HostListener('document:keydown.escape')
  closeAccountMenu(): void {
    this.accountMenuOpen = false;
  }

  toggleSidebar(): void {
    if (window.matchMedia('(max-width: 900px)').matches) {
      this.mobileSidebarOpen = !this.mobileSidebarOpen;
      return;
    }
    this.sidebarCollapsed = !this.sidebarCollapsed;
  }

  closeMobileSidebar(): void {
    this.mobileSidebarOpen = false;
  }

  hasAnyRole(userRoles: string[], ...roles: string[]): boolean {
    return roles.some(role => userRoles.includes(role));
  }

}
