import { Component, inject, OnInit, signal } from '@angular/core';
import { NgFor, NgIf, DecimalPipe } from '@angular/common';
import { TranslatePipe } from '@ngx-translate/core';
import { ApiService } from '../../core/api.service';

interface StockBin {
  binId: number;
  item: { itemCode: string; itemName: string };
  warehouse: { name: string };
  actualQty: number;
  reservedQty: number;
  orderedQty: number;
  valuationRate: number;
}

@Component({
  selector: 'app-stock',
  standalone: true,
  imports: [NgFor, NgIf, DecimalPipe, TranslatePipe],
  template: `
    <div class="d-flex justify-content-between align-items-center mb-3">
      <h5 class="mb-0"><i class="ki-outline ki-chart-simple fs-2 me-2"></i>{{ 'stock.title' | translate }}</h5>
      <button class="btn btn-sm btn-outline-secondary" (click)="load()"><i class="ki-outline ki-arrows-circle fs-4"></i> {{ 'common.refresh' | translate }}</button>
    </div>

    <table class="table table-sm table-hover table-bordered">
      <thead class="table-dark">
        <tr>
          <th>{{ 'stock.item_code' | translate }}</th>
          <th>{{ 'stock.item_name' | translate }}</th>
          <th>{{ 'stock.warehouse' | translate }}</th>
          <th class="text-end">{{ 'stock.actual_qty' | translate }}</th>
          <th class="text-end">{{ 'stock.reserved' | translate }}</th>
          <th class="text-end">{{ 'stock.ordered' | translate }}</th>
          <th class="text-end">{{ 'stock.valuation_rate' | translate }}</th>
          <th class="text-end">{{ 'stock.stock_value' | translate }}</th>
        </tr>
      </thead>
      <tbody>
        <tr *ngFor="let b of bins()">
          <td class="fw-semibold">{{ b.item.itemCode }}</td>
          <td>{{ b.item.itemName }}</td>
          <td>{{ b.warehouse.name }}</td>
          <td class="text-end" [class.text-danger]="b.actualQty < 0">{{ b.actualQty | number:'1.2-2' }}</td>
          <td class="text-end text-warning">{{ b.reservedQty | number:'1.2-2' }}</td>
          <td class="text-end text-info">{{ b.orderedQty | number:'1.2-2' }}</td>
          <td class="text-end">{{ b.valuationRate | number:'1.2-2' }}</td>
          <td class="text-end fw-semibold">{{ (b.actualQty * b.valuationRate) | number:'1.2-2' }}</td>
        </tr>
        <tr *ngIf="bins().length === 0">
          <td colspan="8" class="text-center text-muted py-4">{{ 'stock.no_entries' | translate }}</td>
        </tr>
      </tbody>
    </table>
  `,
})
export class StockComponent implements OnInit {
  api  = inject(ApiService);
  bins = signal<StockBin[]>([]);

  ngOnInit() { this.load(); }
  load() { this.api.get<StockBin[]>('stock').subscribe(d => this.bins.set(d)); }
}
