import { DOCUMENT } from '@angular/common';
import { Component, inject, OnInit, signal } from '@angular/core';
import { Router } from '@angular/router';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { ToastrService } from 'ngx-toastr';
import Swal from 'sweetalert2';
import { ApiService } from '../../core/api.service';
import { RegularListSearchActionsComponent, SearchField } from '../../shared/components/cards/regular-list-search-actions/regular-list-search-actions.component';
import { RegularListHeaderWithActionsComponent } from '../../shared/components/cards/regular-list-header-with-actions/regular-list-header-with-actions.component';

interface ItemGroup { itemGroupId: number; name_EN: string; name_AR?: string; }
interface ItemType  { itemTypeId: number;  name: string; }
interface ItemUOM   { id: number; name_EN: string; name_AR?: string; }

interface Item {
  id: number;
  internalCode: string;
  name_EN: string;
  name_AR?: string;
  itemGroupId: number;
  itemTypeId: number;
  defaultUOMId: number;
  defaultPurchaseUOMId?: number;
  acceptSelling: boolean;
  defaultSellingUOMId?: number;
  description?: string;
  openingStock?: number;
  expirationDate?: string;
  minOrderQuantity?: number;
  safetyStock?: number;
  isActive: boolean;
  isFavorite: boolean;
  itemGroup?: ItemGroup;
  itemType?: ItemType;
  defaultUOM?: ItemUOM;
  defaultPurchaseUOM?: ItemUOM;
  defaultSellingUOM?: ItemUOM;
}

@Component({
  selector: 'app-items',
  standalone: true,
  imports: [TranslatePipe, RegularListSearchActionsComponent, RegularListHeaderWithActionsComponent],
  templateUrl: './items.component.html',
  styleUrl: './items.component.less',
})
export class ItemsComponent implements OnInit {
  private api       = inject(ApiService);
  private router    = inject(Router);
  private translate = inject(TranslateService);
  private toastr    = inject(ToastrService);
  private doc       = inject(DOCUMENT);

  get isRtl() { return this.doc.documentElement.dir === 'rtl'; }

  items        = signal<Item[]>([]);
  selectedIds  = signal<Set<number>>(new Set());
  activeFilter = signal<Record<string, string | number | null>>({});

  uomLabel(uom?: ItemUOM): string {
    if (!uom) return '—';
    return this.isRtl ? (uom.name_AR || uom.name_EN) : (uom.name_EN || uom.name_AR || '');
  }

  groupLabel(g?: ItemGroup): string {
    if (!g) return '—';
    return this.isRtl ? (g.name_AR || g.name_EN) : (g.name_EN || g.name_AR || '');
  }

  itemName(item: Item): string {
    return this.isRtl ? (item.name_AR || item.name_EN) : (item.name_EN || item.name_AR || '');
  }

  get searchFields(): SearchField[] {
    const groupMap = new Map<number, string>();
    const typeMap  = new Map<number, string>();
    for (const item of this.items()) {
      if (item.itemGroup) groupMap.set(item.itemGroupId, this.groupLabel(item.itemGroup));
      if (item.itemType)  typeMap.set(item.itemTypeId,  item.itemType.name);
    }
    const groupOpts = [...groupMap.entries()].map(([value, label]) => ({ value, label })).sort((a, b) => a.label.localeCompare(b.label));
    const typeOpts  = [...typeMap.entries()].map(([value, label])  => ({ value, label })).sort((a, b) => a.label.localeCompare(b.label));

    return [
      { key: 'internalCode', label: this.translate.instant('items.code'),                 type: 'text'   },
      { key: 'name_AR',      label: this.translate.instant('common.name') + ' (AR)',      type: 'text'   },
      { key: 'name_EN',      label: this.translate.instant('common.name') + ' (EN)',      type: 'text'   },
      { key: 'groupId',      label: this.translate.instant('items.group'),                type: 'select', options: groupOpts },
      { key: 'typeId',       label: this.translate.instant('items.type'),                 type: 'select', options: typeOpts  },
    ];
  }

  get sortedItems(): Item[] {
    return [...this.items()].sort((a, b) => {
      if (a.isFavorite !== b.isFavorite) return a.isFavorite ? -1 : 1;
      return a.internalCode.localeCompare(b.internalCode);
    });
  }

  get filteredItems(): Item[] {
    const f = this.activeFilter();
    return this.sortedItems.filter(item => {
      if (f['internalCode'] != null && !item.internalCode.toLowerCase().includes((f['internalCode'] as string).toLowerCase())) return false;
      if (f['name_AR']      != null && !(item.name_AR ?? '').toLowerCase().includes((f['name_AR'] as string).toLowerCase())) return false;
      if (f['name_EN']      != null && !item.name_EN.toLowerCase().includes((f['name_EN'] as string).toLowerCase())) return false;
      if (f['groupId']      != null && item.itemGroupId !== f['groupId'])  return false;
      if (f['typeId']       != null && item.itemTypeId  !== f['typeId'])   return false;
      return true;
    });
  }

  onFilterChange(filter: Record<string, string | number | null>) {
    this.activeFilter.set(filter);
    this.selectedIds.set(new Set());
  }

  ngOnInit() { this.load(); }

  load() {
    this.api.get<Item[]>('items').subscribe(d => {
      this.items.set(d);
      this.selectedIds.set(new Set());
    });
  }

  isSelected(id: number) { return this.selectedIds().has(id); }

  get isAllSelected() {
    const items = this.filteredItems;
    return items.length > 0 && items.every(item => this.selectedIds().has(item.id));
  }

  get isIndeterminate() {
    return this.selectedIds().size > 0 && !this.isAllSelected;
  }

  toggleOne(id: number) {
    const s = new Set(this.selectedIds());
    s.has(id) ? s.delete(id) : s.add(id);
    this.selectedIds.set(s);
  }

  toggleAll() {
    if (this.isAllSelected) {
      this.selectedIds.set(new Set());
    } else {
      this.selectedIds.set(new Set(this.filteredItems.map(i => i.id)));
    }
  }

  addNew() { this.router.navigate(['/stock/items/operation']); }

  exportTemplate() {
    this.api.getBlob('items/export-template').subscribe(blob => {
      const url = URL.createObjectURL(blob);
      const a   = document.createElement('a');
      a.href     = url;
      a.download = 'items-template.xlsx';
      a.click();
      URL.revokeObjectURL(url);
    });
  }

  importExcel(file: File) {
    const fd = new FormData();
    fd.append('file', file);
    this.api.post<{ created: number; errors: string[] }>('items/import', fd).subscribe({
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

  edit(id: number) { this.router.navigate(['/stock/items/operation', id]); }

  delete(id: number) {
    Swal.fire({
      title: this.translate.instant('common.swal_delete_title'),
      text:  this.translate.instant('items.delete_confirm'),
      icon:  'warning',
      showCancelButton:  true,
      confirmButtonText: this.translate.instant('common.delete'),
      cancelButtonText:  this.translate.instant('common.cancel'),
      confirmButtonColor: '#f1416c',
      reverseButtons: this.isRtl,
    }).then(result => {
      if (result.isConfirmed)
        this.api.delete(`items/${id}`).subscribe(() => this.load());
    });
  }

  toggleFavorite(item: Item) {
    this.api.patch<Item>(`items/${item.id}/toggle-favorite`).subscribe(updated => {
      this.items.update(list => list.map(x => x.id === updated.id
        ? { ...updated, itemGroup: x.itemGroup, itemType: x.itemType, defaultUOM: x.defaultUOM } : x));
    });
  }

  toggleActive(item: Item) {
    this.api.patch<Item>(`items/${item.id}/toggle-active`).subscribe(updated => {
      this.items.update(list => list.map(x => x.id === updated.id
        ? { ...updated, itemGroup: x.itemGroup, itemType: x.itemType, defaultUOM: x.defaultUOM } : x));
    });
  }

  deleteSelected() {
    const ids = [...this.selectedIds()];
    if (!ids.length) return;
    Swal.fire({
      title: this.translate.instant('common.swal_delete_title'),
      text:  this.translate.instant('items.delete_selected_confirm', { count: ids.length }),
      icon:  'warning',
      showCancelButton:  true,
      confirmButtonText: this.translate.instant('common.delete'),
      cancelButtonText:  this.translate.instant('common.cancel'),
      confirmButtonColor: '#f1416c',
      reverseButtons: this.isRtl,
    }).then(result => {
      if (result.isConfirmed)
        this.api.deleteBulk('items/bulk', ids).subscribe(() => this.load());
    });
  }
}
