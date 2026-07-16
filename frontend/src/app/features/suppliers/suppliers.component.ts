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

interface SupplierType { id: number; name_EN: string; name_AR: string | null; }
interface Supplier {
  id: number;
  internalCode: string | null;
  name_EN: string;
  name_AR: string | null;
  supplierTypeId: number | null;
  country: string | null;
  phone: string | null;
  isOnHold: boolean;
  isActive: boolean;
  supplierType?: SupplierType;
}

@Component({
  selector: 'app-suppliers',
  standalone: true,
  imports: [TranslatePipe, RegularListHeaderWithActionsComponent, RegularListSearchActionsComponent, CustomTableWithPaginationComponent],
  templateUrl: './suppliers.component.html',
  styleUrl: './suppliers.component.less',
})
export class SuppliersComponent implements OnInit {
  private api = inject(ApiService);
  private router = inject(Router);
  private translate = inject(TranslateService);
  private toastr = inject(ToastrService);
  private doc = inject(DOCUMENT);
  private meta = inject(MetadataService);

  get isRtl() { return this.doc.documentElement.dir === 'rtl'; }

  suppliers     = signal<Supplier[]>([]);
  selectedIds   = signal<Set<any>>(new Set());
  metaReloadKey = signal(0);
  activeFilter  = signal<Record<string, string | number | null>>({});
  columnMeta    = signal<ColumnMeta[]>([]);

  readonly cellRenderers: Record<string, (item: any) => string> = {
    supplierType: (s: Supplier) => {
      if (!s.supplierType) return '—';
      return this.isRtl ? (s.supplierType.name_AR || s.supplierType.name_EN) : s.supplierType.name_EN;
    },
  };

  get searchFields(): SearchField[] {
    return this.meta.toSearchFields(this.columnMeta(), this.isRtl);
  }

  get filtered(): Supplier[] {
    const f = this.activeFilter();
    return this.suppliers().filter(s => {
      if (f['internalCode'] != null && !(s.internalCode ?? '').toLowerCase().includes((f['internalCode'] as string).toLowerCase())) return false;
      if (f['name_AR'] != null && !(s.name_AR ?? '').toLowerCase().includes((f['name_AR'] as string).toLowerCase())) return false;
      if (f['name_EN'] != null && !s.name_EN.toLowerCase().includes((f['name_EN'] as string).toLowerCase())) return false;
      if (f['country'] != null && !(s.country ?? '').toLowerCase().includes((f['country'] as string).toLowerCase())) return false;
      if (f['phone'] != null && !(s.phone ?? '').toLowerCase().includes((f['phone'] as string).toLowerCase())) return false;
      if (f['isOnHold'] != null && s.isOnHold !== (f['isOnHold'] === 1)) return false;
      if (f['isActive'] != null && s.isActive !== (f['isActive'] === 1)) return false;
      return true;
    });
  }

  onFilterChange(filter: Record<string, string | number | null>) {
    this.activeFilter.set(filter);
  }

  ngOnInit() {
    this.load();
  }

  load() {
    this.metaReloadKey.update(n => n + 1);
    this.api.get<Supplier[]>('suppliers').subscribe(d => this.suppliers.set(d));
  }

  addNew() { this.router.navigate(['/buying/suppliers/operation']); }
  edit(id: number) { this.router.navigate(['/buying/suppliers/operation', id]); }

  toggleActive(s: Supplier) {
    this.api.patch<Supplier>(`suppliers/${s.id}/toggle-active`).subscribe(updated =>
      this.suppliers.update(list => list.map(x => x.id === updated.id ? updated : x))
    );
  }

  toggleHold(s: Supplier) {
    this.api.patch<Supplier>(`suppliers/${s.id}/toggle-hold`).subscribe(updated =>
      this.suppliers.update(list => list.map(x => x.id === updated.id ? updated : x))
    );
  }

  delete(id: number) {
    Swal.fire({
      title: this.translate.instant('common.swal_delete_title'),
      text: this.translate.instant('suppliers.delete_confirm'),
      icon: 'warning',
      showCancelButton: true,
      confirmButtonText: this.translate.instant('common.delete'),
      cancelButtonText: this.translate.instant('common.cancel'),
      confirmButtonColor: '#f1416c',
      reverseButtons: this.isRtl,
    }).then(r => { if (r.isConfirmed) this.api.delete(`suppliers/${id}`).subscribe(() => this.load()); });
  }

  deleteSelected() {
    const ids = [...this.selectedIds()];
    if (!ids.length) return;
    Swal.fire({
      title: this.translate.instant('common.swal_delete_title'),
      text: this.translate.instant('suppliers.delete_selected_confirm', { count: ids.length }),
      icon: 'warning',
      showCancelButton: true,
      confirmButtonText: this.translate.instant('common.delete'),
      cancelButtonText: this.translate.instant('common.cancel'),
      confirmButtonColor: '#f1416c',
      reverseButtons: this.isRtl,
    }).then(r => { if (r.isConfirmed) this.api.deleteBulk('suppliers/bulk', ids).subscribe(() => this.load()); });
  }
}
