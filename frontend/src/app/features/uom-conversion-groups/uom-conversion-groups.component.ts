import { DOCUMENT } from '@angular/common';
import { Component, inject, OnInit, signal } from '@angular/core';
import { Router } from '@angular/router';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { ToastrService } from 'ngx-toastr';
import Swal from 'sweetalert2';
import { ApiService } from '../../core/api.service';
import { AuthService } from '../../core/auth.service';
import { ColumnMeta, MetadataService } from '../../core/metadata.service';
import { RegularListSearchActionsComponent, SearchField } from '../../shared/components/cards/regular-list-search-actions/regular-list-search-actions.component';
import { RegularListHeaderWithActionsComponent } from '../../shared/components/cards/regular-list-header-with-actions/regular-list-header-with-actions.component';
import { CustomTableWithPaginationComponent } from '../../shared/components/custom-controls/custom-table-with-pagination/custom-table-with-pagination.component';

interface UomConversionGroup {
  id: number;
  internalCode: string;
  name_EN: string;
  name_AR: string;
  isActive: boolean;
  isFavorite: boolean;
}

@Component({
  selector: 'app-uom-conversion-groups',
  standalone: true,
  imports: [TranslatePipe, RegularListSearchActionsComponent, RegularListHeaderWithActionsComponent, CustomTableWithPaginationComponent],
  templateUrl: './uom-conversion-groups.component.html',
  styleUrl: './uom-conversion-groups.component.less',
})
export class UomConversionGroupsComponent implements OnInit {
  private api = inject(ApiService);
  private auth = inject(AuthService);
  private router = inject(Router);
  private translate = inject(TranslateService);
  private toastr = inject(ToastrService);
  private doc = inject(DOCUMENT);
  private meta = inject(MetadataService);

  get isRtl() { return this.doc.documentElement.dir === 'rtl'; }

  groups = signal<UomConversionGroup[]>([]);
  selectedIds   = signal<Set<any>>(new Set());
  metaReloadKey = signal(0);
  activeFilter  = signal<Record<string, string | number | null>>({});
  columnMeta    = signal<ColumnMeta[]>([]);

  nameLabel(g: UomConversionGroup): string {
    return this.isRtl ? (g.name_AR || g.name_EN) : (g.name_EN || g.name_AR);
  }

  get searchFields(): SearchField[] {
    return this.meta.toSearchFields(this.columnMeta(), this.isRtl);
  }

  get sortedGroups(): UomConversionGroup[] {
    return [...this.groups()].sort((a, b) => {
      if (a.isFavorite !== b.isFavorite) return a.isFavorite ? -1 : 1;
      return this.nameLabel(a).localeCompare(this.nameLabel(b));
    });
  }

  get filteredGroups(): UomConversionGroup[] {
    const f = this.activeFilter();
    return this.sortedGroups.filter(g => {
      if (f['internalCode'] != null && !(g.internalCode ?? '').toLowerCase().includes((f['internalCode'] as string).toLowerCase())) return false;
      if (f['name_AR'] != null && !(g.name_AR ?? '').toLowerCase().includes((f['name_AR'] as string).toLowerCase())) return false;
      if (f['name_EN'] != null && !(g.name_EN ?? '').toLowerCase().includes((f['name_EN'] as string).toLowerCase())) return false;
      if (f['isActive'] != null && g.isActive !== (f['isActive'] === 1)) return false;
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
    this.api.get<UomConversionGroup[]>('uomconversiongroups').subscribe(d => this.groups.set(d));
  }

  // ── CRUD ───────────────────────────────────────────────────────────────────

  addNew() { this.router.navigate(['/stock/uom-conversion-groups/operation']); }

  exportTemplate() {
    this.api.getBlob('uomconversiongroups/export-template').subscribe(blob => {
      const url = URL.createObjectURL(blob);
      const a = document.createElement('a');
      a.href = url;
      a.download = 'uom-conversion-groups-template.xlsx';
      a.click();
      URL.revokeObjectURL(url);
    });
  }

  importExcel(file: File) {
    const fd = new FormData();
    fd.append('file', file);
    const userId = this.auth.user()?.id;
    if (userId != null) fd.append('insertedBy', String(userId));
    this.api.post<{ created: number; errors: string[] }>('uomconversiongroups/import', fd).subscribe({
      next: result => {
        if (result.errors.length === 0) {
          this.toastr.success(
            this.translate.instant('common.import_success_count', { count: result.created })
          );
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

  edit(id: number) { this.router.navigate(['/stock/uom-conversion-groups/operation', id]); }

  delete(id: number) {
    Swal.fire({
      title: this.translate.instant('common.swal_delete_title'),
      text: this.translate.instant('uom_conversion_groups.delete_confirm'),
      icon: 'warning',
      showCancelButton: true,
      confirmButtonText: this.translate.instant('common.delete'),
      cancelButtonText: this.translate.instant('common.cancel'),
      confirmButtonColor: '#f1416c',
      reverseButtons: this.isRtl,
    }).then(result => {
      if (result.isConfirmed)
        this.api.delete(`uomconversiongroups/${id}`).subscribe({
          next: () => this.load(),
          error: (err) => {
            if (err.status === 409)
              this.toastr.error(err.error?.message ?? 'Cannot delete: this group is in use.', undefined, { timeOut: 5000 });
            else
              this.toastr.error(this.translate.instant('common.error'));
          },
        });
    });
  }

  toggleFavorite(group: UomConversionGroup) {
    this.api.patch<UomConversionGroup>(`uomconversiongroups/${group.id}/toggle-favorite`).subscribe(updated => {
      this.groups.update(list => list.map(g => g.id === updated.id ? updated : g));
    });
  }

  toggleActive(group: UomConversionGroup) {
    this.api.get<{ count: number }>(`uomconversiongroups/${group.id}/factors-count`).subscribe(({ count }) => {
      const isDeactivating = group.isActive;
      const textKey = count > 0
        ? (isDeactivating ? 'uom_conversion_groups.deactivate_cascade_confirm' : 'uom_conversion_groups.activate_cascade_confirm')
        : (isDeactivating ? 'uom_conversion_groups.deactivate_no_factors_confirm' : 'uom_conversion_groups.activate_no_factors_confirm');

      Swal.fire({
        title: this.translate.instant('common.swal_delete_title'),
        text: this.translate.instant(textKey, { count }),
        icon: 'warning',
        showCancelButton: true,
        confirmButtonText: this.translate.instant(isDeactivating ? 'common.deactivate' : 'common.activate'),
        cancelButtonText: this.translate.instant('common.cancel'),
        confirmButtonColor: isDeactivating ? '#f1416c' : '#50cd89',
        reverseButtons: this.isRtl,
      }).then(result => {
        if (!result.isConfirmed) return;
        this.api.patch<{ group: UomConversionGroup; affectedFactorsCount: number }>(
          `uomconversiongroups/${group.id}/toggle-active`
        ).subscribe(({ group: updated, affectedFactorsCount }) => {
          this.groups.update(list => list.map(g => g.id === updated.id ? updated : g));
          if (affectedFactorsCount > 0)
            this.toastr.info(this.translate.instant('uom_conversion_groups.cascade_factors_affected', { count: affectedFactorsCount }));
        });
      });
    });
  }

  deleteSelected() {
    const ids = [...this.selectedIds()];
    if (!ids.length) return;
    Swal.fire({
      title: this.translate.instant('common.swal_delete_title'),
      text: this.translate.instant('uom_conversion_groups.delete_selected_confirm', { count: ids.length }),
      icon: 'warning',
      showCancelButton: true,
      confirmButtonText: this.translate.instant('common.delete'),
      cancelButtonText: this.translate.instant('common.cancel'),
      confirmButtonColor: '#f1416c',
      reverseButtons: this.isRtl,
    }).then(result => {
      if (result.isConfirmed)
        this.api.deleteBulk('uomconversiongroups/bulk', ids).subscribe({
          next: () => this.load(),
          error: (err) => {
            if (err.status === 409)
              this.toastr.error(err.error?.message ?? 'Cannot delete: some selected groups are in use.', undefined, { timeOut: 5000 });
            else
              this.toastr.error(this.translate.instant('common.error'));
          },
        });
    });
  }
}
