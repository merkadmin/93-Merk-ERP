import { DOCUMENT } from '@angular/common';
import { Component, computed, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { TranslatePipe } from '@ngx-translate/core';

interface GuideStep {
  key: string;
  route: string;
  icon: string;
}

const COMPLETED_KEY = 'merk_guide_completed';
const DISMISSED_KEY = 'merk_guide_dismissed';

const STEPS: GuideStep[] = [
  { key: 'warehouse_category', route: '/stock/warehouse-categories/operation', icon: 'ki-abstract-28' },
  { key: 'warehouse',          route: '/stock/warehouses/operation',           icon: 'ki-home-2'     },
  { key: 'item_group',         route: '/stock/item-groups/operation',          icon: 'ki-category'   },
  { key: 'item',               route: '/stock/items/operation',                icon: 'ki-package'    },
  { key: 'stock_recon',        route: '/stock/stock-reconciliation/operation', icon: 'ki-document'   },
];

@Component({
  selector: 'app-getting-started-guide',
  standalone: true,
  imports: [TranslatePipe],
  templateUrl: './getting-started-guide.component.html',
  styleUrl: './getting-started-guide.component.less',
})
export class GettingStartedGuideComponent {
  private router = inject(Router);
  private doc    = inject(DOCUMENT);

  get isRtl() { return this.doc.documentElement.dir === 'rtl'; }

  steps       = STEPS;
  isOpen      = signal(false);
  isDismissed = signal(this._loadDismissed());
  completed   = signal<Set<string>>(this._loadCompleted());

  completedCount = computed(() => this.steps.filter(s => this.completed().has(s.key)).length);
  progress       = computed(() => Math.round((this.completedCount() / this.steps.length) * 100));
  allDone        = computed(() => this.completedCount() === this.steps.length);

  toggle() { this.isOpen.update(v => !v); }

  dismiss() {
    this.isOpen.set(false);
    this.isDismissed.set(true);
    localStorage.setItem(DISMISSED_KEY, '1');
  }

  isCompleted(key: string) { return this.completed().has(key); }

  toggleStep(key: string) {
    this.completed.update(set => {
      const next = new Set(set);
      if (next.has(key)) next.delete(key); else next.add(key);
      localStorage.setItem(COMPLETED_KEY, JSON.stringify([...next]));
      return next;
    });
  }

  goTo(route: string) {
    this.router.navigate([route]);
    this.isOpen.set(false);
  }

  private _loadCompleted(): Set<string> {
    try {
      const json = localStorage.getItem(COMPLETED_KEY);
      return json ? new Set(JSON.parse(json)) : new Set();
    } catch { return new Set(); }
  }

  private _loadDismissed(): boolean {
    return localStorage.getItem(DISMISSED_KEY) === '1';
  }
}
