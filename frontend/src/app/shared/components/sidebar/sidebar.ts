import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { TranslatePipe } from '@ngx-translate/core';
import { ThemeService } from '../../../services/theme.service';

@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [CommonModule, RouterLink, RouterLinkActive, TranslatePipe],
  templateUrl: './sidebar.html',
})
export class SidebarComponent {
  sidebarOpen = false;

  constructor(public themeService: ThemeService) {}

  openSidebar()  { this.sidebarOpen = true; }
  closeSidebar() { this.sidebarOpen = false; }
}
