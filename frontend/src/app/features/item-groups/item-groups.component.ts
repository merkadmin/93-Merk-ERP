import { DOCUMENT } from '@angular/common';
import { Component, inject, OnInit, signal } from '@angular/core';
import { Router } from '@angular/router';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { ToastrService } from 'ngx-toastr';
import Swal from 'sweetalert2';
import { ApiService } from '../../core/api.service';
import { RegularListSearchActionsComponent, SearchField } from '../../shared/components/cards/regular-list-search-actions/regular-list-search-actions.component';
import { RegularListHeaderWithActionsComponent } from '../../shared/components/cards/regular-list-header-with-actions/regular-list-header-with-actions.component';

interface ItemGroup {
  itemGroupId: number;
  name: string;
  parentItemGroupId: number | null;
  isGroup: boolean;
  isActive: boolean;
  isFavorite: boolean;
}

@Component({
  selector: 'app-item-groups',
  standalone: true,
  imports: [TranslatePipe, RegularListSearchActionsComponent, RegularListHeaderWithActionsComponent],
  templateUrl: './item-groups.component.html',
  styleUrl: './item-groups.component.less',
})
export class ItemGroupsComponent implements OnInit {
  private api       = inject(ApiService);
  private router    = inject(Router);
  private translate = inject(TranslateService);
  private toastr    = inject(ToastrService);
  private doc       = inject(DOCUMENT);

  get isRtl() { return this.doc.documentElement.dir === 'rtl'; }

  groups       = signal<ItemGroup[]>([]);
  selectedIds  = signal<Set<number>>(new Set());
  activeFilter = signal<Record<string, string | number | null>>({});

  parentName(id: number | null): string {
    if (id == null) return '—';
    return this.groups().find(g => g.itemGroupId === id)?.name ?? '—';
  }

  get searchFields(): SearchField[] {
    return [
      { key: 'name', label: this.translate.instant('common.name'), type: 'text' },
    ];
  }

  get sortedGroups(): ItemGroup[] {
    return [...this.groups()].sort((a, b) => {
      if (a.isFavorite !== b.isFavorite) return a.isFavorite ? -1 : 1;
      return a.name.localeCompare(b.name);
    });
  }

  get filteredGroups(): ItemGroup[] {
    const f = this.activeFilter();
    return this.sortedGroups.filter(g => {
      if (f['name'] != null && !g.name.toLowerCase().includes((f['name'] as string).toLowerCase())) return false;
      return true;
    });
  }

  onFilterChange(filter: Record<string, string | number | null>) {
    this.activeFilter.set(filter);
    this.selectedIds.set(new Set());
  }

  ngOnInit() { this.load(); }

  load() {
    this.api.get<ItemGroup[]>('itemgroups').subscribe(d => {
      this.groups.set(d);
      this.selectedIds.set(new Set());
    });
  }

  isSelected(id: number) { return this.selectedIds().has(id); }

  get isAllSelected() {
    const g = this.filteredGroups;
    return g.length > 0 && g.every(item => this.selectedIds().has(item.itemGroupId));
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
      this.selectedIds.set(new Set(this.filteredGroups.map(g => g.itemGroupId)));
    }
  }

  addNew() { this.router.navigate(['/stock/item-groups/operation']); }

  exportTemplate() {
    this.api.getBlob('itemgroups/export-template').subscribe(blob => {
      const url = URL.createObjectURL(blob);
      const a   = document.createElement('a');
      a.href     = url;
      a.download = 'item-groups-template.xlsx';
      a.click();
      URL.revokeObjectURL(url);
    });
  }

  importExcel(file: File) {
    const fd = new FormData();
    fd.append('file', file);
    this.api.post<{ created: number; errors: string[] }>('itemgroups/import', fd).subscribe({
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

  edit(id: number) { this.router.navigate(['/stock/item-groups/operation', id]); }

  delete(id: number) {
    Swal.fire({
      title: this.translate.instant('common.swal_delete_title'),
      text:  this.translate.instant('item_groups.delete_confirm'),
      icon:  'warning',
      showCancelButton:  true,
      confirmButtonText: this.translate.instant('common.delete'),
      cancelButtonText:  this.translate.instant('common.cancel'),
      confirmButtonColor: '#f1416c',
      reverseButtons: this.isRtl,
    }).then(result => {
      if (result.isConfirmed)
        this.api.delete(`itemgroups/${id}`).subscribe(() => this.load());
    });
  }

  toggleFavorite(g: ItemGroup) {
    this.api.patch<ItemGroup>(`itemgroups/${g.itemGroupId}/toggle-favorite`).subscribe(updated => {
      this.groups.update(list => list.map(x => x.itemGroupId === updated.itemGroupId ? updated : x));
    });
  }

  toggleActive(g: ItemGroup) {
    this.api.patch<ItemGroup>(`itemgroups/${g.itemGroupId}/toggle-active`).subscribe(updated => {
      this.groups.update(list => list.map(x => x.itemGroupId === updated.itemGroupId ? updated : x));
    });
  }

  deleteSelected() {
    const ids = [...this.selectedIds()];
    if (!ids.length) return;
    Swal.fire({
      title: this.translate.instant('common.swal_delete_title'),
      text:  this.translate.instant('item_groups.delete_selected_confirm', { count: ids.length }),
      icon:  'warning',
      showCancelButton:  true,
      confirmButtonText: this.translate.instant('common.delete'),
      cancelButtonText:  this.translate.instant('common.cancel'),
      confirmButtonColor: '#f1416c',
      reverseButtons: this.isRtl,
    }).then(result => {
      if (result.isConfirmed)
        this.api.deleteBulk('itemgroups/bulk', ids).subscribe(() => this.load());
    });
  }
}
