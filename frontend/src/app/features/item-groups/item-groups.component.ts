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

interface ItemGroup {
  itemGroupId: number;
  internalCode?: string;
  name_EN: string;
  name_AR?: string;
  parentItemGroupId: number | null;
  isMain: boolean;
  isActive: boolean;
  isFavorite: boolean;
}

@Component({
  selector: 'app-item-groups',
  standalone: true,
  imports: [TranslatePipe, RegularListSearchActionsComponent, RegularListHeaderWithActionsComponent, CustomTableWithPaginationComponent],
  templateUrl: './item-groups.component.html',
  styleUrl: './item-groups.component.less',
})
export class ItemGroupsComponent implements OnInit {
  private api = inject(ApiService);
  private router = inject(Router);
  private translate = inject(TranslateService);
  private toastr = inject(ToastrService);
  private doc = inject(DOCUMENT);
  private meta = inject(MetadataService);

  get isRtl() { return this.doc.documentElement.dir === 'rtl'; }

  groups = signal<ItemGroup[]>([]);
  selectedIds   = signal<Set<any>>(new Set());
  metaReloadKey = signal(0);
  activeFilter  = signal<Record<string, string | number | null>>({});
  columnMeta    = signal<ColumnMeta[]>([]);

  nameLabel(g: ItemGroup): string {
    return this.isRtl ? (g.name_AR || g.name_EN) : (g.name_EN || g.name_AR || '');
  }

  parentName(id: number | null): string {
    if (id == null) return '—';
    const g = this.groups().find(x => x.itemGroupId === id);
    return g ? this.nameLabel(g) : '—';
  }

  get searchFields(): SearchField[] {
    const parentOpts = this.groups()
      .map(g => ({ value: g.itemGroupId, label: this.nameLabel(g) }))
      .sort((a, b) => a.label.localeCompare(b.label));

    return this.meta.toSearchFields(this.columnMeta(), this.isRtl, {
      parentItemGroup: parentOpts,
    });
  }

  get sortedGroups(): ItemGroup[] {
    return [...this.groups()].sort((a, b) => {
      if (a.isFavorite !== b.isFavorite) return a.isFavorite ? -1 : 1;
      return a.name_EN.localeCompare(b.name_EN);
    });
  }

  get filteredGroups(): ItemGroup[] {
    const f = this.activeFilter();
    return this.sortedGroups.filter(g => {
      if (f['internalCode'] != null && !(g.internalCode ?? '').toLowerCase().includes((f['internalCode'] as string).toLowerCase())) return false;
      if (f['name_AR'] != null && !(g.name_AR ?? '').toLowerCase().includes((f['name_AR'] as string).toLowerCase())) return false;
      if (f['name_EN'] != null && !g.name_EN.toLowerCase().includes((f['name_EN'] as string).toLowerCase())) return false;
      if (f['parentItemGroup'] != null && g.parentItemGroupId !== f['parentItemGroup']) return false;
      if (f['isMain'] != null && g.isMain !== (f['isMain'] === 1)) return false;
      if (f['isActive'] != null && g.isActive !== (f['isActive'] === 1)) return false;
      return true;
    });
  }

  readonly cellRenderers: Record<string, (item: any) => string> = {
    parentItemGroup: (item) => this.parentName(item.parentItemGroupId),
  };

  onFilterChange(filter: Record<string, string | number | null>) {
    this.activeFilter.set(filter);
  }

  ngOnInit() {
    this.load();
  }

  load() {
    this.metaReloadKey.update(n => n + 1);
    this.api.get<ItemGroup[]>('itemgroups').subscribe(d => this.groups.set(d));
  }

  addNew() { this.router.navigate(['/stock/item-groups/operation']); }

  exportTemplate() {
    this.api.getBlob('itemgroups/export-template').subscribe(blob => {
      const url = URL.createObjectURL(blob);
      const a = document.createElement('a');
      a.href = url;
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
    const childCount = this.groups().filter(g => g.parentItemGroupId === id).length;
    const text = childCount > 0
      ? this.translate.instant('item_groups.delete_has_children_confirm', { count: childCount })
      : this.translate.instant('item_groups.delete_confirm');

    Swal.fire({
      title: this.translate.instant('common.swal_delete_title'),
      text,
      icon: 'warning',
      showCancelButton: true,
      confirmButtonText: this.translate.instant('common.delete'),
      cancelButtonText: this.translate.instant('common.cancel'),
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
      text: this.translate.instant('item_groups.delete_selected_confirm', { count: ids.length }),
      icon: 'warning',
      showCancelButton: true,
      confirmButtonText: this.translate.instant('common.delete'),
      cancelButtonText: this.translate.instant('common.cancel'),
      confirmButtonColor: '#f1416c',
      reverseButtons: this.isRtl,
    }).then(result => {
      if (result.isConfirmed)
        this.api.deleteBulk('itemgroups/bulk', ids).subscribe(() => this.load());
    });
  }
}
