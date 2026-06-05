import { Component, inject, OnInit, signal } from '@angular/core';
import { NgFor, NgIf, DecimalPipe } from '@angular/common';
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
  imports: [NgFor, NgIf, DecimalPipe],
  template: `
    <div class="d-flex justify-content-between align-items-center mb-3">
      <h5 class="mb-0"><i class="bi bi-graph-up me-2"></i>Current Stock</h5>
      <button class="btn btn-sm btn-outline-secondary" (click)="load()"><i class="bi bi-arrow-clockwise"></i> Refresh</button>
    </div>

    <table class="table table-sm table-hover table-bordered">
      <thead class="table-dark">
        <tr><th>Item Code</th><th>Item Name</th><th>Warehouse</th><th class="text-end">Actual Qty</th><th class="text-end">Reserved</th><th class="text-end">Ordered</th><th class="text-end">Valuation Rate</th><th class="text-end">Stock Value</th></tr>
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
        <tr *ngIf="bins().length === 0"><td colspan="8" class="text-center text-muted py-4">No stock entries found.</td></tr>
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
