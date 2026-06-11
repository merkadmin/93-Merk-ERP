import { DOCUMENT } from '@angular/common';
import { Component, inject, OnInit, signal } from '@angular/core';
import { Router } from '@angular/router';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import Swal from 'sweetalert2';
import { ApiService } from '../../core/api.service';
import { RegularListHeaderWithActionsComponent } from '../../shared/components/cards/regular-list-header-with-actions/regular-list-header-with-actions.component';
import { RegularListSearchActionsComponent, SearchField } from '../../shared/components/cards/regular-list-search-actions/regular-list-search-actions.component';

interface Warehouse     { id: number; internalCode: string | null; name_EN: string; name_AR: string | null; parentWarehouseId: number | null; wareHouseTypeId: number | null; wareHouseCategoryId: number | null; isParent: boolean; isActive: boolean; }
interface WarehouseNode extends Warehouse { level: number; }

@Component({
  selector: 'app-warehouses',
  standalone: true,
  imports: [TranslatePipe, RegularListHeaderWithActionsComponent, RegularListSearchActionsComponent],
  templateUrl: './warehouses.component.html',
  styleUrl: './warehouses.component.less',
})
export class WarehousesComponent implements OnInit {
  private api       = inject(ApiService);
  private router    = inject(Router);
  private translate = inject(TranslateService);
  private doc       = inject(DOCUMENT);

  get isRtl() { return this.doc.documentElement.dir === 'rtl'; }

  warehouses   = signal<Warehouse[]>([]);
  selectedIds  = signal<Set<number>>(new Set());
  activeFilter = signal<Record<string, string | number | null>>({});

  get searchFields(): SearchField[] {
    return [{ key: 'name', label: this.translate.instant('common.name'), type: 'text' }];
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
    const q = (f['name'] as string | null) ?? '';
    return [...this.warehouses()]
      .filter(w => w.name_EN.toLowerCase().includes(q.toLowerCase()) || (w.name_AR ?? '').toLowerCase().includes(q.toLowerCase()))
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

  onFilterChange(filter: Record<string, string | number | null>) {
    this.activeFilter.set(filter);
    this.selectedIds.set(new Set());
  }

  ngOnInit() { this.load(); }

  load() {
    this.api.get<Warehouse[]>('warehouses').subscribe(d => {
      this.warehouses.set(d);
      this.selectedIds.set(new Set());
    });
  }

  isSelected(id: number) { return this.selectedIds().has(id); }

  get isAllSelected() {
    const rows = this.displayRows;
    return rows.length > 0 && rows.every(w => this.selectedIds().has(w.id));
  }

  get isIndeterminate() { return this.selectedIds().size > 0 && !this.isAllSelected; }

  toggleOne(id: number) {
    const s = new Set(this.selectedIds());
    s.has(id) ? s.delete(id) : s.add(id);
    this.selectedIds.set(s);
  }

  toggleAll() {
    this.isAllSelected
      ? this.selectedIds.set(new Set())
      : this.selectedIds.set(new Set(this.displayRows.map(w => w.id)));
  }

  addNew()         { this.router.navigate(['/stock/warehouses/operation']); }
  edit(id: number) { this.router.navigate(['/stock/warehouses/operation', id]); }

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
