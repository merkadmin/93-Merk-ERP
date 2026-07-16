import { DOCUMENT } from '@angular/common';
import { Component, inject, OnInit, signal } from '@angular/core';
import { Router } from '@angular/router';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { ToastrService } from 'ngx-toastr';
import Swal from 'sweetalert2';
import { ApiService } from '../../core/api.service';
import { ColumnMeta, MetadataService } from '../../core/metadata.service';
import { RegularListHeaderWithActionsComponent } from '../../shared/components/cards/regular-list-header-with-actions/regular-list-header-with-actions.component';
import { RegularListSearchActionsComponent, SearchField } from '../../shared/components/cards/regular-list-search-actions/regular-list-search-actions.component';
import { CustomTableWithPaginationComponent } from '../../shared/components/custom-controls/custom-table-with-pagination/custom-table-with-pagination.component';

interface NamedRef { id: number; name_EN: string; name_AR: string | null; }
interface Status { id: number; name_EN: string; name_AR: string; }
interface PurchaseReceipt {
  id: number;
  internalCode: string | null;
  postingDate: string;
  grandTotal: number;
  stockTransactionStatusId: number;
  supplier?: NamedRef;
  company?: NamedRef;
  setWarehouse?: NamedRef;
  stockTransactionStatus?: Status;
}

const STATUS_DRAFT = 1;
const STATUS_SUBMITTED = 3;

@Component({
  selector: 'app-purchase-receipts',
  standalone: true,
  imports: [TranslatePipe, RegularListHeaderWithActionsComponent, RegularListSearchActionsComponent, CustomTableWithPaginationComponent],
  templateUrl: './purchase-receipts.component.html',
  styleUrl: './purchase-receipts.component.less',
})
export class PurchaseReceiptsComponent implements OnInit {
  private api = inject(ApiService);
  private router = inject(Router);
  private translate = inject(TranslateService);
  private toastr = inject(ToastrService);
  private doc = inject(DOCUMENT);
  private meta = inject(MetadataService);

  get isRtl() { return this.doc.documentElement.dir === 'rtl'; }

  readonly STATUS_DRAFT = STATUS_DRAFT;
  readonly STATUS_SUBMITTED = STATUS_SUBMITTED;

  receipts      = signal<PurchaseReceipt[]>([]);
  selectedIds   = signal<Set<any>>(new Set());
  metaReloadKey = signal(0);
  activeFilter  = signal<Record<string, string | number | null>>({});
  columnMeta    = signal<ColumnMeta[]>([]);
  statuses      = signal<Status[]>([]);

  private label(n?: NamedRef): string {
    if (!n) return '—';
    return this.isRtl ? (n.name_AR || n.name_EN) : n.name_EN;
  }

  readonly cellRenderers: Record<string, (item: any) => string> = {
    supplier:                (r: PurchaseReceipt) => this.label(r.supplier),
    company:                 (r: PurchaseReceipt) => this.label(r.company),
    setWarehouse:             (r: PurchaseReceipt) => this.label(r.setWarehouse),
    stockTransactionStatus:  (r: PurchaseReceipt) => r.stockTransactionStatus
      ? (this.isRtl ? r.stockTransactionStatus.name_AR : r.stockTransactionStatus.name_EN) : '—',
    grandTotal: (r: PurchaseReceipt) => (r.grandTotal ?? 0).toFixed(2),
  };

  get searchFields(): SearchField[] {
    return this.meta.toSearchFields(this.columnMeta(), this.isRtl);
  }

  get filtered(): PurchaseReceipt[] {
    const f = this.activeFilter();
    return this.receipts().filter(r => {
      if (f['internalCode'] != null && !(r.internalCode ?? '').toLowerCase().includes((f['internalCode'] as string).toLowerCase())) return false;
      if (f['postingDate'] != null && !(r.postingDate ?? '').includes(f['postingDate'] as string)) return false;
      return true;
    });
  }

  onFilterChange(filter: Record<string, string | number | null>) {
    this.activeFilter.set(filter);
  }

  ngOnInit() {
    this.load();
    this.api.get<Status[]>('stocktransactionstatuses').subscribe(d => this.statuses.set(d));
  }

  load() {
    this.metaReloadKey.update(n => n + 1);
    this.api.get<PurchaseReceipt[]>('purchasereceipts').subscribe(d => this.receipts.set(d));
  }

  addNew() { this.router.navigate(['/stock/purchase-receipt/operation']); }
  edit(item: PurchaseReceipt) {
    const extras = item.stockTransactionStatusId === STATUS_SUBMITTED ? { queryParams: { readonly: 'true' } } : {};
    this.router.navigate(['/stock/purchase-receipt/operation', item.id], extras);
  }

  private updateRecordStatus(updated: PurchaseReceipt) {
    this.receipts.update(list => list.map(r => r.id !== updated.id ? r : {
      ...updated,
      supplier: r.supplier,
      company: r.company,
      setWarehouse: r.setWarehouse,
      stockTransactionStatus: this.statuses().find(s => s.id === updated.stockTransactionStatusId),
    }));
  }

  submitReceipt(item: PurchaseReceipt) {
    Swal.fire({
      title: this.translate.instant('common.swal_delete_title'),
      text: this.translate.instant('purchase_receipts.submit_confirm'),
      icon: 'question',
      showCancelButton: true,
      confirmButtonText: this.translate.instant('common.submit'),
      cancelButtonText: this.translate.instant('common.cancel'),
      reverseButtons: this.isRtl,
    }).then(r => {
      if (r.isConfirmed)
        this.api.patch<PurchaseReceipt>(`purchasereceipts/${item.id}/submit`).subscribe(updated => {
          this.updateRecordStatus(updated);
          this.toastr.success(this.translate.instant('purchase_receipts.submit_success'));
        });
    });
  }

  reissueReceipt(item: PurchaseReceipt) {
    Swal.fire({
      title: this.translate.instant('common.swal_delete_title'),
      text: this.translate.instant('purchase_receipts.reissue_confirm'),
      icon: 'question',
      showCancelButton: true,
      confirmButtonText: this.translate.instant('common.reissue'),
      cancelButtonText: this.translate.instant('common.cancel'),
      reverseButtons: this.isRtl,
    }).then(r => {
      if (r.isConfirmed)
        this.api.patch<PurchaseReceipt>(`purchasereceipts/${item.id}/reissue`).subscribe(updated => {
          this.updateRecordStatus(updated);
          this.toastr.success(this.translate.instant('purchase_receipts.reissue_success'));
        });
    });
  }

  cancelReceipt(item: PurchaseReceipt) {
    Swal.fire({
      title: this.translate.instant('common.swal_delete_title'),
      text: this.translate.instant('purchase_receipts.cancel_confirm'),
      icon: 'warning',
      showCancelButton: true,
      confirmButtonText: this.translate.instant('common.confirm'),
      cancelButtonText: this.translate.instant('common.cancel'),
      confirmButtonColor: '#f1416c',
      reverseButtons: this.isRtl,
    }).then(r => {
      if (r.isConfirmed)
        this.api.patch<PurchaseReceipt>(`purchasereceipts/${item.id}/cancel`).subscribe(updated => {
          this.updateRecordStatus(updated);
          this.toastr.success(this.translate.instant('purchase_receipts.cancel_success'));
        });
    });
  }

  delete(id: number) {
    Swal.fire({
      title: this.translate.instant('common.swal_delete_title'),
      text: this.translate.instant('purchase_receipts.delete_confirm'),
      icon: 'warning',
      showCancelButton: true,
      confirmButtonText: this.translate.instant('common.delete'),
      cancelButtonText: this.translate.instant('common.cancel'),
      confirmButtonColor: '#f1416c',
      reverseButtons: this.isRtl,
    }).then(r => { if (r.isConfirmed) this.api.delete(`purchasereceipts/${id}`).subscribe(() => this.load()); });
  }

  deleteSelected() {
    const ids = [...this.selectedIds()];
    if (!ids.length) return;
    Swal.fire({
      title: this.translate.instant('common.swal_delete_title'),
      text: this.translate.instant('purchase_receipts.delete_selected_confirm', { count: ids.length }),
      icon: 'warning',
      showCancelButton: true,
      confirmButtonText: this.translate.instant('common.delete'),
      cancelButtonText: this.translate.instant('common.cancel'),
      confirmButtonColor: '#f1416c',
      reverseButtons: this.isRtl,
    }).then(r => { if (r.isConfirmed) this.api.deleteBulk('purchasereceipts/bulk', ids).subscribe(() => this.load()); });
  }
}
