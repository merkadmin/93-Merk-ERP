import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LoadingService } from '../../services/loading.service';

@Component({
  selector: 'app-loading-spinner',
  standalone: true,
  imports: [CommonModule],
  template: `
    @if (loading.loading()) {
      <div class="loading-overlay">
        <div class="loading-card">
          <span class="spinner-border text-primary" role="status" style="width:2.5rem;height:2.5rem;"></span>
        </div>
      </div>
    }
  `,
  styles: [`
    .loading-overlay {
      position: fixed; inset: 0; z-index: 9999;
      display: flex; align-items: center; justify-content: center;
      background: rgba(0, 0, 0, 0.35);
      backdrop-filter: blur(2px);
    }
    .loading-card {
      background: var(--bs-body-bg);
      border-radius: 1rem;
      padding: 1.75rem 2.25rem;
      box-shadow: 0 4px 24px rgba(0,0,0,0.18);
      display: flex;
      align-items: center;
      justify-content: center;
    }
  `]
})
export class LoadingSpinnerComponent {
  protected loading = inject(LoadingService);
}
