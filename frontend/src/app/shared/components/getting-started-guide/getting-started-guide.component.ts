import { DOCUMENT } from '@angular/common';
import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { TranslatePipe } from '@ngx-translate/core';
import { ApiService } from '../../../core/api.service';

interface GuideStep {
  key: string;
  route: string;
  icon: string;
}

interface GuideStatus {
  warehouse:  boolean;
  item:       boolean;
  stockRecon: boolean;
}

const DISMISSED_KEY = 'merk_guide_dismissed';

const STEPS: GuideStep[] = [
  { key: 'warehouse',  route: '/stock/warehouses/operation',           icon: 'ki-home-2'  },
  { key: 'item',       route: '/stock/items/operation',                icon: 'ki-package' },
  { key: 'stock_recon', route: '/stock/stock-reconciliation/operation', icon: 'ki-document' },
];

@Component({
  selector: 'app-getting-started-guide',
  standalone: true,
  imports: [TranslatePipe],
  templateUrl: './getting-started-guide.component.html',
  styleUrl: './getting-started-guide.component.less',
})
export class GettingStartedGuideComponent implements OnInit {
  private api    = inject(ApiService);
  private router = inject(Router);
  private doc    = inject(DOCUMENT);

  get isRtl() { return this.doc.documentElement.dir === 'rtl'; }

  steps       = STEPS;
  isOpen      = signal(false);
  isDismissed = signal(this._loadDismissed());
  dbStatus    = signal<GuideStatus>({ warehouse: false, item: false, stockRecon: false });

  completedCount = computed(() => this.steps.filter(s => this._isDone(s.key)).length);
  progress       = computed(() => Math.round((this.completedCount() / this.steps.length) * 100));
  allDone        = computed(() => this.completedCount() === this.steps.length);

  ngOnInit() { this._loadStatus(); }

  isDone(key: string) { return this._isDone(key); }

  toggle() {
    this.isOpen.update(v => !v);
    if (this.isOpen()) this._loadStatus();
  }

  dismiss() {
    this.isOpen.set(false);
    this.isDismissed.set(true);
    localStorage.setItem(DISMISSED_KEY, '1');
  }

  goTo(route: string) {
    this.router.navigate([route]);
    this.isOpen.set(false);
  }

  private _isDone(key: string): boolean {
    const s = this.dbStatus();
    switch (key) {
      case 'warehouse':  return s.warehouse;
      case 'item':       return s.item;
      case 'stock_recon': return s.stockRecon;
      default:           return false;
    }
  }

  private _loadStatus() {
    this.api.get<GuideStatus>('gettingstarted/status').subscribe(s => this.dbStatus.set(s));
  }

  private _loadDismissed(): boolean {
    return localStorage.getItem(DISMISSED_KEY) === '1';
  }
}
