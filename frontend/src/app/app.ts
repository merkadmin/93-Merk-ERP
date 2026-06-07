import { Component, ViewChild, HostListener } from '@angular/core';
import { CommonModule, NgTemplateOutlet } from '@angular/common';
import { RouterOutlet, RouterLink } from '@angular/router';
import { TranslatePipe } from '@ngx-translate/core';
import { ThemeService } from './services/theme.service';
import { LanguageService } from './services/language.service';
import { ServerStatusService } from './services/server-status.service';
import { SidebarComponent } from './shared/components/sidebar/sidebar';
import { LoadingSpinnerComponent } from './shared/components/loading-spinner/loading-spinner';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, NgTemplateOutlet, RouterOutlet, RouterLink, TranslatePipe, SidebarComponent, LoadingSpinnerComponent],
  templateUrl: './app.html',
  styleUrl: './app.less'
})
export class App {
  @ViewChild(SidebarComponent) private sidebarRef!: SidebarComponent;

  userMenuOpen = false;

  constructor(
    public themeService:   ThemeService,
    public langService:    LanguageService,
    public serverStatus:   ServerStatusService,
  ) {}

  retry() { window.location.reload(); }

  @HostListener('document:click')
  closeMenus() { this.userMenuOpen = false; }

  toggleUserMenu(e: Event) { e.stopPropagation(); this.userMenuOpen = !this.userMenuOpen; }

  openSidebar() { this.sidebarRef?.openSidebar(); }
}
