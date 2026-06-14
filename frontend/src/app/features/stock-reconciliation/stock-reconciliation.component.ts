import { DOCUMENT } from '@angular/common';
import { Component, inject, OnInit, signal } from '@angular/core';
import { Router } from '@angular/router';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { ToastrService } from 'ngx-toastr';
import Swal from 'sweetalert2';
import { ApiService } from '../../core/api.service';
import { ColumnMeta, MetadataService } from '../../core/metadata.service';
import { RegularListSearchActionsComponent, SearchField } from '../../shared/components/cards/regular-list-search-actions/regular-list-search-actions.component';
import { RegularListHeaderWithActionsComponent } from '../../shared/components/cards/regular-list-header-with-actions/regular-list-header-with-actions.component';
import { CustomTableWithPaginationComponent } from '../../shared/components/custom-controls/custom-table-with-pagination/custom-table-with-pagination.component';

interface TxnType   { id: number; name_EN: string; name_AR: string; }
interface TxnStatus { id: number; name_EN: string; name_AR: string; }
interface Warehouse  { id: number; name_EN: string; name_AR: string | null; }

interface SRT {
  id: number;
  internalCode: string | null;
  stockTransactionTypeId: number;
  stockTransactionStatusId: number;
  postingDate: string;
  postingTime: string;
  setWarehouseId: number | null;
  stockTransactionType?: TxnType;
  stockTransactionStatus?: TxnStatus;
  setWarehouse?: Warehouse;
}

@Component({
  selector: 'app-stock-reconciliation',
  standalone: true,
  imports: [TranslatePipe, RegularListSearchActionsComponent, RegularListHeaderWithActionsComponent, CustomTableWithPaginationComponent],
  templateUrl: './stock-reconciliation.component.html',
  styleUrl: './stock-reconciliation.component.less',
})
export class StockReconciliationComponent implements OnInit {
  private api       = inject(ApiService);
  private router    = inject(Router);
  private translate = inject(TranslateService);
  private toastr    = inject(ToastrService);
  private doc       = inject(DOCUMENT);
  private meta      = inject(MetadataService);

  get isRtl() { return this.doc.documentElement.dir === 'rtl'; }

  records       = signal<SRT[]>([]);
  selectedIds   = signal<Set<any>>(new Set());
  metaReloadKey = signal(0);
  activeFilter  = signal<Record<string, string | number | null>>({});
  columnMeta    = signal<ColumnMeta[]>([]);

  txnTypes    = signal<TxnType[]>([]);
  txnStatuses = signal<TxnStatus[]>([]);
  warehouses  = signal<Warehouse[]>([]);

  typeLabel(t?: TxnType)     { return t ? (this.isRtl ? t.name_AR : t.name_EN) : '—'; }
  statusLabel(s?: TxnStatus) { return s ? (this.isRtl ? s.name_AR : s.name_EN) : '—'; }
  wLabel(w?: Warehouse)      { return w ? (this.isRtl ? (w.name_AR || w.name_EN) : w.name_EN) : '—'; }

  get searchFields(): SearchField[] {
    const typeOpts   = this.txnTypes().map(t   => ({ value: t.id, label: this.typeLabel(t) }));
    const statusOpts = this.txnStatuses().map(s => ({ value: s.id, label: this.statusLabel(s) }));
    const whOpts     = this.warehouses().map(w  => ({ value: w.id, label: this.wLabel(w) }));
    return this.meta.toSearchFields(this.columnMeta(), this.isRtl, {
      stockTransactionType:   typeOpts,
      stockTransactionStatus: statusOpts,
      setWarehouse:           whOpts,
    });
  }

  get filteredRecords(): SRT[] {
    const f = this.activeFilter();
    return this.records().filter(r => {
      if (f['internalCode']          != null && !(r.internalCode ?? '').toLowerCase().includes((f['internalCode'] as string).toLowerCase())) return false;
      if (f['stockTransactionType']   != null && r.stockTransactionTypeId   !== f['stockTransactionType'])   return false;
      if (f['stockTransactionStatus'] != null && r.stockTransactionStatusId !== f['stockTransactionStatus']) return false;
      if (f['setWarehouse']           != null && r.setWarehouseId           !== f['setWarehouse'])           return false;
      return true;
    });
  }

  readonly cellRenderers: Record<string, (row: any) => string> = {
    stockTransactionType:   (r) => this.typeLabel(r.stockTransactionType),
    stockTransactionStatus: (r) => this.statusLabel(r.stockTransactionStatus),
    setWarehouse:           (r) => this.wLabel(r.setWarehouse),
  };

  onFilterChange(filter: Record<string, string | number | null>) { this.activeFilter.set(filter); }

  ngOnInit() { this.load(); }

  load() {
    this.metaReloadKey.update(n => n + 1);
    this.api.get<SRT[]>('stockreconciliationtransactions').subscribe(d => this.records.set(d));
    this.api.get<TxnType[]>('stocktransactiontypes').subscribe(d => this.txnTypes.set(d));
    this.api.get<TxnStatus[]>('stocktransactionstatuses').subscribe(d => this.txnStatuses.set(d));
    this.api.get<Warehouse[]>('warehouses').subscribe(d => this.warehouses.set(d));
  }

  addNew() { this.router.navigate(['/stock/stock-reconciliation/operation']); }
  edit(id: number) { this.router.navigate(['/stock/stock-reconciliation/operation', id]); }

  delete(id: number) {
    Swal.fire({
      title: this.translate.instant('common.swal_delete_title'),
      text:  this.translate.instant('stock_reconciliation.delete_confirm'),
      icon: 'warning',
      showCancelButton: true,
      confirmButtonText: this.translate.instant('common.delete'),
      cancelButtonText:  this.translate.instant('common.cancel'),
      confirmButtonColor: '#f1416c',
      reverseButtons: this.isRtl,
    }).then(result => {
      if (result.isConfirmed)
        this.api.delete(`stockreconciliationtransactions/${id}`).subscribe(() => this.load());
    });
  }

  deleteSelected() {
    const ids = [...this.selectedIds()];
    if (!ids.length) return;
    Swal.fire({
      title: this.translate.instant('common.swal_delete_title'),
      text:  this.translate.instant('stock_reconciliation.delete_selected_confirm', { count: ids.length }),
      icon: 'warning',
      showCancelButton: true,
      confirmButtonText: this.translate.instant('common.delete'),
      cancelButtonText:  this.translate.instant('common.cancel'),
      confirmButtonColor: '#f1416c',
      reverseButtons: this.isRtl,
    }).then(result => {
      if (result.isConfirmed)
        this.api.deleteBulk('stockreconciliationtransactions/bulk', ids).subscribe(() => this.load());
    });
  }
}
