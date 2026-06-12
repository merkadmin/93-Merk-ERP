import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { TranslatePipe } from '@ngx-translate/core';
import { ThemeService } from '../../../services/theme.service';
import { SidebarService } from './sidebar.service';

@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [CommonModule, RouterLink, RouterLinkActive, TranslatePipe],
  templateUrl: './sidebar.html',
  styleUrl: './sidebar.less',
})
export class SidebarComponent {
  sidebarOpen = false;
  expandedSections: Set<string>;

  constructor(
    public themeService: ThemeService,
    public sidebarService: SidebarService,
  ) {
    this.expandedSections = new Set(sidebarService.sections.map(s => s.id));
  }

  openSidebar() { this.sidebarOpen = true; }
  closeSidebar() { this.sidebarOpen = false; }

  toggleSection(id: string) {
    if (this.expandedSections.has(id)) this.expandedSections.delete(id);
    else this.expandedSections.add(id);
  }

  isSectionExpanded(id: string): boolean {
    return this.expandedSections.has(id);
  }
}
