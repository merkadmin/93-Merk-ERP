import { Component, inject, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { NgFor, NgIf } from '@angular/common';
import { TranslatePipe } from '@ngx-translate/core';
import { ApiService } from '../../core/api.service';

interface Item      { itemId: number; itemCode: string; itemName: string; }
interface Warehouse { warehouseId: number; name: string; }

interface MovementForm {
  itemId: number;
  warehouseId: number;
  qty: number;
  valuationRate: number;
  voucherType: string;
  voucherNo: string;
  batchNo: string;
  postingDate: string;
}

@Component({
  selector: 'app-stock-movement',
  standalone: true,
  imports: [FormsModule, NgFor, NgIf, TranslatePipe],
  templateUrl: './stock-movement.component.html',
  styleUrl: './stock-movement.component.less',
})
export class StockMovementComponent implements OnInit {
  api        = inject(ApiService);
  items      = signal<Item[]>([]);
  warehouses = signal<Warehouse[]>([]);
  posting    = signal(false);
  success    = signal(false);
  error      = signal('');
  form: MovementForm = this.blank();

  blank(): MovementForm {
    return { itemId: 0, warehouseId: 0, qty: 0, valuationRate: 0, voucherType: 'StockEntry', voucherNo: '', batchNo: '', postingDate: new Date().toISOString().split('T')[0] };
  }

  ngOnInit() {
    this.api.get<Item[]>('items').subscribe(d => this.items.set(d));
    this.api.get<Warehouse[]>('warehouses').subscribe(d => this.warehouses.set(d));
  }

  post() {
    this.posting.set(true); this.success.set(false); this.error.set('');
    const payload = {
      itemId: this.form.itemId,
      warehouseId: this.form.warehouseId,
      qty: this.form.qty,
      valuationRate: this.form.valuationRate,
      voucherType: this.form.voucherType,
      voucherNo: this.form.voucherNo,
      batchNo: this.form.batchNo || null,
      postingDate: this.form.postingDate,
    };
    this.api.post('stock/movement', payload).subscribe({
      next: () => { this.posting.set(false); this.success.set(true); this.form = this.blank(); },
      error: (e) => { this.posting.set(false); this.error.set(e?.error ?? 'Failed to post movement.'); },
    });
  }
}


