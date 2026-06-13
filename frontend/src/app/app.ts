import { Component, ViewChild, HostListener, signal } from '@angular/core';
import { CommonModule, NgTemplateOutlet } from '@angular/common';
import { RouterOutlet, RouterLink, Router, NavigationEnd } from '@angular/router';
import { TranslatePipe } from '@ngx-translate/core';
import { filter } from 'rxjs/operators';
import { ThemeService } from './services/theme.service';
import { LanguageService } from './services/language.service';
import { ServerStatusService } from './services/server-status.service';
import { AuthService } from './core/auth.service';
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
  isLoginPage = signal(false);

  constructor(
    public themeService: ThemeService,
    public langService: LanguageService,
    public serverStatus: ServerStatusService,
    public auth: AuthService,
    private router: Router,
  ) {
    this.router.events
      .pipe(filter((e): e is NavigationEnd => e instanceof NavigationEnd))
      .subscribe(e => {
        const url = e.urlAfterRedirects;
        this.isLoginPage.set(url.startsWith('/login') || url.startsWith('/register'));
      });
  }

  retry() { window.location.reload(); }

  logout(): void {
    this.auth.logout();
    this.router.navigate(['/login']);
  }

  @HostListener('document:click')
  closeMenus() { this.userMenuOpen = false; }

  toggleUserMenu(e: Event) { e.stopPropagation(); this.userMenuOpen = !this.userMenuOpen; }

  openSidebar() { this.sidebarRef?.openSidebar(); }
}
