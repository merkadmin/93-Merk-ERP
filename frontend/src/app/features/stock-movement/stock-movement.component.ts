import { Component, inject, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { NgFor, NgIf } from '@angular/common';
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
  imports: [FormsModule, NgFor, NgIf],
  template: `
    <h5 class="mb-4"><i class="bi bi-arrow-left-right me-2"></i>Post Stock Movement</h5>

    <div class="card" style="max-width:650px">
      <div class="card-body">
        <form (ngSubmit)="post()">
          <div class="row g-3">

            <div class="col-12">
              <label class="form-label">Item <span class="text-danger">*</span></label>
              <select class="form-select" [(ngModel)]="form.itemId" name="itemId" required>
                <option [ngValue]="0">— Select Item —</option>
                <option *ngFor="let i of items()" [ngValue]="i.itemId">{{ i.itemCode }} — {{ i.itemName }}</option>
              </select>
            </div>

            <div class="col-12">
              <label class="form-label">Warehouse <span class="text-danger">*</span></label>
              <select class="form-select" [(ngModel)]="form.warehouseId" name="warehouseId" required>
                <option [ngValue]="0">— Select Warehouse —</option>
                <option *ngFor="let w of warehouses()" [ngValue]="w.warehouseId">{{ w.name }}</option>
              </select>
            </div>

            <div class="col-6">
              <label class="form-label">Qty <span class="text-danger">*</span></label>
              <input class="form-control" type="number" step="0.001" [(ngModel)]="form.qty" name="qty" required>
              <div class="form-text">Positive = Receipt &nbsp;|&nbsp; Negative = Issue</div>
            </div>

            <div class="col-6">
              <label class="form-label">Valuation Rate</label>
              <input class="form-control" type="number" step="0.01" [(ngModel)]="form.valuationRate" name="valuationRate">
            </div>

            <div class="col-6">
              <label class="form-label">Voucher Type</label>
              <select class="form-select" [(ngModel)]="form.voucherType" name="voucherType">
                <option>StockEntry</option>
                <option>PurchaseReceipt</option>
                <option>DeliveryNote</option>
                <option>StockReconciliation</option>
              </select>
            </div>

            <div class="col-6">
              <label class="form-label">Voucher No</label>
              <input class="form-control" [(ngModel)]="form.voucherNo" name="voucherNo">
            </div>

            <div class="col-6">
              <label class="form-label">Batch No</label>
              <input class="form-control" [(ngModel)]="form.batchNo" name="batchNo">
            </div>

            <div class="col-6">
              <label class="form-label">Posting Date <span class="text-danger">*</span></label>
              <input class="form-control" type="date" [(ngModel)]="form.postingDate" name="postingDate" required>
            </div>

          </div>

          <div class="mt-4 d-flex gap-2 align-items-center">
            <button type="submit" class="btn btn-success" [disabled]="posting()">
              <span *ngIf="posting()" class="spinner-border spinner-border-sm me-1"></span>
              Post Movement
            </button>
            <span *ngIf="success()" class="text-success fw-semibold"><i class="bi bi-check-circle me-1"></i>Posted successfully!</span>
            <span *ngIf="error()" class="text-danger">{{ error() }}</span>
          </div>
        </form>
      </div>
    </div>
  `,
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
