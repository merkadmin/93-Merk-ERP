import { DOCUMENT } from '@angular/common';
import { Component, ElementRef, inject, OnInit, signal, ViewChild } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { ToastrService } from 'ngx-toastr';
import { ApiService } from '../../../core/api.service';
import { AuthService } from '../../../core/auth.service';
import { RegularOperationHeaderComponent } from '../../../shared/components/cards/regular-operation-header/regular-operation-header.component';
import { RegularOperationActionsComponent } from '../../../shared/components/cards/regular-operation-actions/regular-operation-actions.component';
import { CustomSelectInputComponent, SelectOption } from '../../../shared/components/custom-controls/custom-select-input/custom-select-input.component';

interface TxnType  { id: number; name_EN: string; name_AR: string; }
interface Warehouse { id: number; name_EN: string; name_AR: string | null; isParent?: boolean; }
interface Item      { id: number; internalCode: string; name_EN: string; name_AR?: string; }
interface UOM       { id: number; name_EN: string; name_AR: string; }

interface SRTForm {
  id: number;
  stockTransactionTypeId: number;
  internalCode: string;
  postingDate: string;
  postingTime: string;
  setWarehouseId: number | null;
}

interface DetailForm {
  itemId: number;
  warehouseId: number;
  quantity: number;
  uomId: number;
}

interface ApiDetail {
  id: number;
  stockReconciliationTransactionId: number;
  itemId: number;
  warehouseId: number;
  quantity: number;
  uomId: number;
}

interface SavedRow {
  internalCode: string;
  txnType: string;
  postingDate: string;
  detailCount: number;
}

@Component({
  selector: 'app-stock-reconciliation-operation',
  standalone: true,
  imports: [FormsModule, TranslatePipe, RegularOperationHeaderComponent, RegularOperationActionsComponent, CustomSelectInputComponent],
  templateUrl: './stock-reconciliation-operation.component.html',
  styleUrl: './stock-reconciliation-operation.component.less',
})
export class StockReconciliationOperationComponent implements OnInit {
  @ViewChild('qtyInput') qtyInputRef?: ElementRef<HTMLInputElement>;

  private api       = inject(ApiService);
  private auth      = inject(AuthService);
  private router    = inject(Router);
  private route     = inject(ActivatedRoute);
  private translate = inject(TranslateService);
  private toastr    = inject(ToastrService);
  private doc       = inject(DOCUMENT);

  get isRtl() { return this.doc.documentElement.dir === 'rtl'; }

  isEdit       = signal(false);
  saving       = signal(false);
  savingNew    = signal(false);
  addingDetail = signal(false);

  txnTypes   = signal<TxnType[]>([]);
  warehouses = signal<Warehouse[]>([]);
  items      = signal<Item[]>([]);
  uoms       = signal<UOM[]>([]);

  apiDetails     = signal<ApiDetail[]>([]);
  pendingDetails = signal<DetailForm[]>([]);
  savedRows      = signal<SavedRow[]>([]);

  form: SRTForm = this.blank();
  newDetail: DetailForm = this.blankDetail();
  barcodeInput = '';

  get displayDetails(): (DetailForm | ApiDetail)[] {
    return this.isEdit() ? this.apiDetails() : this.pendingDetails();
  }

  get allWarehouseOptions(): SelectOption[] {
    return this.warehouses()
      .map(w => ({ value: w.id, label: this.isRtl ? (w.name_AR || w.name_EN) : w.name_EN }));
  }

  get warehouseOptions(): SelectOption[] {
    return this.warehouses()
      .filter(w => !w.isParent)
      .map(w => ({ value: w.id, label: this.isRtl ? (w.name_AR || w.name_EN) : w.name_EN }));
  }

  get itemOptions(): SelectOption[] {
    return this.items().map(i => ({
      value: i.id,
      label: this.isRtl ? (i.name_AR || i.name_EN) : i.name_EN,
      sublabel: i.internalCode,
    }));
  }

  get uomOptions(): SelectOption[] {
    return this.uoms().map(u => ({ value: u.id, label: this.isRtl ? u.name_AR : u.name_EN }));
  }

  itemName(id: number): string {
    const i = this.items().find(x => x.id === id);
    return i ? (this.isRtl ? (i.name_AR || i.name_EN) : i.name_EN) : '—';
  }
  warehouseName(id: number): string {
    const w = this.warehouses().find(x => x.id === id);
    return w ? (this.isRtl ? (w.name_AR || w.name_EN) : w.name_EN) : '—';
  }
  uomName(id: number): string {
    const u = this.uoms().find(x => x.id === id);
    return u ? (this.isRtl ? u.name_AR : u.name_EN) : '—';
  }

  ngOnInit() {
    this.api.get<TxnType[]>('stocktransactiontypes').subscribe(d => this.txnTypes.set(d));
    this.api.get<Warehouse[]>('warehouses').subscribe(d => this.warehouses.set(d));
    this.api.get<Item[]>('items').subscribe(d => this.items.set(d));
    this.api.get<UOM[]>('uoms').subscribe(d => this.uoms.set(d));

    const id = Number(this.route.snapshot.paramMap.get('id'));
    if (id) {
      this.isEdit.set(true);
      this.api.get<any>(`stockreconciliationtransactions/${id}`).subscribe(data => {
        this.form = {
          id:                    data.id,
          stockTransactionTypeId: data.stockTransactionTypeId,
          internalCode:          data.internalCode ?? '',
          postingDate:           data.postingDate,
          postingTime:           data.postingTime?.substring(0, 5) ?? '',
          setWarehouseId:        data.setWarehouseId ?? null,
        };
        this.apiDetails.set(data.details ?? []);
      });
    } else {
      this.loadNextCode();
    }
  }

  scanBarcode() {
    const code = this.barcodeInput.trim();
    if (!code) return;
    this.api.get<any>(`items/barcode/${encodeURIComponent(code)}`).subscribe({
      next: result => {
        this.newDetail.itemId = result.itemId;
        this.newDetail.uomId  = result.uomId;
        if (this.form.setWarehouseId && !this.newDetail.warehouseId)
          this.newDetail.warehouseId = this.form.setWarehouseId;
        this.barcodeInput = '';
        setTimeout(() => this.qtyInputRef?.nativeElement.focus(), 50);
      },
      error: () => {
        this.toastr.warning(
          this.translate.instant('stock_reconciliation.barcode_not_found', { code }),
          undefined, { timeOut: 3000 }
        );
        this.barcodeInput = '';
      },
    });
  }

  addDetail() {
    if (!this.newDetail.itemId || !this.newDetail.warehouseId || !this.newDetail.quantity || !this.newDetail.uomId) {
      this.toastr.warning(this.translate.instant('stock_reconciliation.detail_fill_all'));
      return;
    }

    if (!this.isEdit()) {
      this.pendingDetails.update(list => [...list, { ...this.newDetail }]);
      this.newDetail = this.blankDetail();
      return;
    }

    this.addingDetail.set(true);
    this.api.post<ApiDetail>(`stockreconciliationtransactions/${this.form.id}/details`, this.newDetail)
      .subscribe({
        next: detail => {
          this.apiDetails.update(list => [...list, detail]);
          this.newDetail = this.blankDetail();
          this.addingDetail.set(false);
        },
        error: () => this.addingDetail.set(false),
      });
  }

  removePendingDetail(index: number) {
    this.pendingDetails.update(list => list.filter((_, i) => i !== index));
  }

  removeApiDetail(detailId: number) {
    this.api.delete(`stockreconciliationtransactions/${this.form.id}/details/${detailId}`)
      .subscribe(() => this.apiDetails.update(list => list.filter(d => d.id !== detailId)));
  }

  private loadNextCode() {
    this.api.get<{ code: string }>('stockreconciliationtransactions/nextcode')
      .subscribe(r => { this.form.internalCode = r.code; });
  }

  private blank(): SRTForm {
    const today = new Date().toISOString().split('T')[0];
    const now   = new Date().toTimeString().substring(0, 5);
    return {
      id: 0,
      stockTransactionTypeId: 2,
      internalCode:   '',
      postingDate:    today,
      postingTime:    now,
      setWarehouseId: null,
    };
  }

  private blankDetail(): DetailForm {
    return { itemId: 0, warehouseId: 0, quantity: 0, uomId: 0 };
  }

  private validate(): boolean {
    const missing: string[] = [];
    if (!this.form.internalCode?.trim())
      missing.push(this.translate.instant('common.internal_code'));
    if (!this.form.stockTransactionTypeId)
      missing.push(this.translate.instant('stock_reconciliation.txn_type'));
    if (!this.form.postingDate)
      missing.push(this.translate.instant('stock_reconciliation.posting_date'));

    if (missing.length) {
      this.toastr.error(missing.join('<br>'), this.translate.instant('common.validation_error'), { enableHtml: true });
      return false;
    }
    return true;
  }

  private submit(andNew: boolean) {
    if (!this.validate()) return;
    andNew ? this.savingNew.set(true) : this.saving.set(true);

    const userId = this.auth.user()?.id ?? null;

    const req = this.isEdit()
      ? this.api.put<any>(`stockreconciliationtransactions/${this.form.id}`, {
          stockTransactionTypeId:   this.form.stockTransactionTypeId,
          stockTransactionStatusId: 1,
          internalCode:             this.form.internalCode,
          postingDate:              this.form.postingDate,
          postingTime:              this.form.postingTime + ':00',
          setWarehouseId:           this.form.setWarehouseId,
        })
      : this.api.post<any>('stockreconciliationtransactions', {
          stockTransactionTypeId:   this.form.stockTransactionTypeId,
          stockTransactionStatusId: 1,
          internalCode:             this.form.internalCode,
          postingDate:              this.form.postingDate,
          postingTime:              this.form.postingTime + ':00',
          setWarehouseId:           this.form.setWarehouseId,
          insertedBy:               userId,
          details:                  this.pendingDetails(),
        });

    req.subscribe({
      next: () => {
        this.toastr.success(this.translate.instant('common.save_success'));
        if (andNew) {
          const typeName = this.txnTypes().find(t => t.id === this.form.stockTransactionTypeId);
          this.savedRows.update(rows => [...rows, {
            internalCode: this.form.internalCode,
            txnType:      typeName ? (this.isRtl ? typeName.name_AR : typeName.name_EN) : '—',
            postingDate:  this.form.postingDate,
            detailCount:  this.pendingDetails().length,
          }]);
          this.form = this.blank();
          this.pendingDetails.set([]);
          this.isEdit.set(false);
          this.savingNew.set(false);
          this.loadNextCode();
        } else {
          this.back();
        }
      },
      error: () => {
        this.saving.set(false);
        this.savingNew.set(false);
      },
    });
  }

  save() { this.submit(false); }
  saveAndNew() { this.submit(true); }

  resetForm() {
    this.form = this.blank();
    this.pendingDetails.set([]);
    this.loadNextCode();
  }

  back() { this.router.navigate(['/stock/stock-reconciliation']); }
}
