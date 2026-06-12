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

interface Warehouse         { id: number; internalCode: string | null; name_EN: string; name_AR: string | null; parentWarehouseId: number | null; wareHouseTypeId: number | null; wareHouseCategoryId: number | null; isParent: boolean; isActive: boolean; }
interface WarehouseNode     extends Warehouse { level: number; }
interface WareHouseType     { id: number; name_EN: string; name_AR: string | null; }
interface WareHouseCategory { id: number; name_EN: string; name_AR: string | null; }

@Component({
  selector: 'app-warehouses',
  standalone: true,
  imports: [TranslatePipe, RegularListHeaderWithActionsComponent, RegularListSearchActionsComponent, CustomTableWithPaginationComponent],
  templateUrl: './warehouses.component.html',
  styleUrl: './warehouses.component.less',
})
export class WarehousesComponent implements OnInit {
  private api      = inject(ApiService);
  private router   = inject(Router);
  private translate = inject(TranslateService);
  private toastr   = inject(ToastrService);
  private doc      = inject(DOCUMENT);
  private meta     = inject(MetadataService);

  get isRtl() { return this.doc.documentElement.dir === 'rtl'; }

  warehouses          = signal<Warehouse[]>([]);
  wareHouseTypes      = signal<WareHouseType[]>([]);
  wareHouseCategories = signal<WareHouseCategory[]>([]);
  selectedIds         = signal<Set<any>>(new Set());
  activeFilter        = signal<Record<string, string | number | null>>({});
  columnMeta          = signal<ColumnMeta[]>([]);

  get searchFields(): SearchField[] {
    const typeOpts = this.wareHouseTypes()
      .map(t => ({ value: t.id, label: this.isRtl ? (t.name_AR || t.name_EN) : (t.name_EN || t.name_AR || '') }))
      .sort((a, b) => a.label.localeCompare(b.label));
    const catOpts = this.wareHouseCategories()
      .map(c => ({ value: c.id, label: this.isRtl ? (c.name_AR || c.name_EN) : (c.name_EN || c.name_AR || '') }))
      .sort((a, b) => a.label.localeCompare(b.label));

    return this.meta.toSearchFields(this.columnMeta(), this.isRtl, {
      wareHouseType:     typeOpts,
      wareHouseCategory: catOpts,
    });
  }

  label(w: Warehouse): string {
    return this.isRtl ? (w.name_AR || w.name_EN) : (w.name_EN || w.name_AR || '');
  }

  private get isFiltering(): boolean {
    const f = this.activeFilter();
    return Object.values(f).some(v => v != null && v !== '');
  }

  private get flatFiltered(): WarehouseNode[] {
    const f = this.activeFilter();
    return [...this.warehouses()]
      .filter(w => {
        if (f['internalCode']      != null && !(w.internalCode ?? '').toLowerCase().includes((f['internalCode'] as string).toLowerCase())) return false;
        if (f['name_AR']           != null && !(w.name_AR ?? '').toLowerCase().includes((f['name_AR'] as string).toLowerCase())) return false;
        if (f['name_EN']           != null && !w.name_EN.toLowerCase().includes((f['name_EN'] as string).toLowerCase())) return false;
        if (f['wareHouseType']     != null && w.wareHouseTypeId     !== f['wareHouseType'])     return false;
        if (f['wareHouseCategory'] != null && w.wareHouseCategoryId !== f['wareHouseCategory']) return false;
        if (f['isParent']          != null && w.isParent  !== (f['isParent']  === 1)) return false;
        if (f['isActive']          != null && w.isActive  !== (f['isActive']  === 1)) return false;
        return true;
      })
      .sort((a, b) => a.name_EN.localeCompare(b.name_EN))
      .map(w => ({ ...w, level: 0 }));
  }

  private get treeFlat(): WarehouseNode[] {
    const all = this.warehouses();
    const root = all.find(w => !w.parentWarehouseId);
    if (!root) return all.sort((a, b) => a.name_EN.localeCompare(b.name_EN)).map(w => ({ ...w, level: 0 }));

    const walk = (id: number, level: number): WarehouseNode[] => {
      const node = all.find(w => w.id === id);
      if (!node) return [];
      const children = all
        .filter(w => w.parentWarehouseId === id)
        .sort((a, b) => a.name_EN.localeCompare(b.name_EN));
      return [{ ...node, level }, ...children.flatMap(c => walk(c.id, level + 1))];
    };

    return walk(root.id, 0);
  }

  get displayRows(): WarehouseNode[] {
    return this.isFiltering ? this.flatFiltered : this.treeFlat;
  }

  readonly cellRenderers: Record<string, (item: any) => string> = {
    wareHouseType:     (item) => this.typeLabel(item.wareHouseTypeId),
    wareHouseCategory: (item) => this.categoryLabel(item.wareHouseCategoryId),
  };

  onFilterChange(filter: Record<string, string | number | null>) {
    this.activeFilter.set(filter);
  }

  typeLabel(id: number | null): string {
    if (!id) return '—';
    const t = this.wareHouseTypes().find(x => x.id === id);
    return t ? (this.isRtl ? (t.name_AR || t.name_EN) : (t.name_EN || t.name_AR || '')) : '—';
  }

  categoryLabel(id: number | null): string {
    if (!id) return '—';
    const c = this.wareHouseCategories().find(x => x.id === id);
    return c ? (this.isRtl ? (c.name_AR || c.name_EN) : (c.name_EN || c.name_AR || '')) : '—';
  }

  ngOnInit() {
    this.load();
  }

  load() {
    this.api.get<Warehouse[]>('warehouses').subscribe(d => this.warehouses.set(d));
    this.api.get<WareHouseType[]>('warehousetypes').subscribe(d => this.wareHouseTypes.set(d));
    this.api.get<WareHouseCategory[]>('warehousecategories').subscribe(d => this.wareHouseCategories.set(d));
  }

  addNew()         { this.router.navigate(['/stock/warehouses/operation']); }
  edit(id: number) { this.router.navigate(['/stock/warehouses/operation', id]); }

  exportTemplate() {
    this.api.getBlob('warehouses/export-template').subscribe(blob => {
      const url = URL.createObjectURL(blob);
      const a   = document.createElement('a');
      a.href     = url;
      a.download = 'warehouses-template.xlsx';
      a.click();
      URL.revokeObjectURL(url);
    });
  }

  importExcel(file: File) {
    const fd = new FormData();
    fd.append('file', file);
    this.api.post<{ created: number; errors: string[] }>('warehouses/import', fd).subscribe({
      next: result => {
        if (result.errors.length === 0) {
          this.toastr.success(this.translate.instant('common.import_success_count', { count: result.created }));
        } else {
          const msg = `${this.translate.instant('common.import_success_count', { count: result.created })}<br>`
            + result.errors.map(e => `• ${e}`).join('<br>');
          this.toastr.warning(msg, this.translate.instant('common.import_partial'), { enableHtml: true, timeOut: 8000 });
        }
        this.load();
      },
      error: () => this.toastr.error(this.translate.instant('common.import_error')),
    });
  }

  toggleActive(w: Warehouse) {
    this.api.patch<Warehouse>(`warehouses/${w.id}/toggle-active`).subscribe(updated =>
      this.warehouses.update(list => list.map(x => x.id === updated.id ? updated : x))
    );
  }

  delete(id: number) {
    Swal.fire({
      title: this.translate.instant('common.swal_delete_title'),
      text:  this.translate.instant('warehouses.delete_confirm'),
      icon:  'warning',
      showCancelButton:  true,
      confirmButtonText: this.translate.instant('common.delete'),
      cancelButtonText:  this.translate.instant('common.cancel'),
      confirmButtonColor: '#f1416c',
      reverseButtons: this.isRtl,
    }).then(r => { if (r.isConfirmed) this.api.delete(`warehouses/${id}`).subscribe(() => this.load()); });
  }

  deleteSelected() {
    const ids = [...this.selectedIds()];
    if (!ids.length) return;
    Swal.fire({
      title: this.translate.instant('common.swal_delete_title'),
      text:  this.translate.instant('warehouses.delete_selected_confirm', { count: ids.length }),
      icon:  'warning',
      showCancelButton:  true,
      confirmButtonText: this.translate.instant('common.delete'),
      cancelButtonText:  this.translate.instant('common.cancel'),
      confirmButtonColor: '#f1416c',
      reverseButtons: this.isRtl,
    }).then(r => { if (r.isConfirmed) this.api.deleteBulk('warehouses/bulk', ids).subscribe(() => this.load()); });
  }
}
