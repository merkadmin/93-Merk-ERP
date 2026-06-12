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

interface UOM { id: number; name_EN: string; name_AR: string; }
interface Group { id: number; name_EN: string; name_AR: string; }

interface UomConversionFactor {
  id: number;
  uomFromId: number;
  uomToId: number;
  value: number;
  uomConversionGroupId: number | null;
  internalCode: string | null;
  isActive: boolean;
  isFavorite: boolean;
  uomFrom?: UOM;
  uomTo?: UOM;
  uomConversionGroup?: Group;
}

@Component({
  selector: 'app-uom-conversion-factors',
  standalone: true,
  imports: [TranslatePipe, RegularListSearchActionsComponent, RegularListHeaderWithActionsComponent, CustomTableWithPaginationComponent],
  templateUrl: './uom-conversion-factors.component.html',
  styleUrl: './uom-conversion-factors.component.less',
})
export class UomConversionFactorsComponent implements OnInit {
  private api = inject(ApiService);
  private router = inject(Router);
  private translate = inject(TranslateService);
  private toastr = inject(ToastrService);
  private doc = inject(DOCUMENT);
  private meta = inject(MetadataService);

  get isRtl() { return this.doc.documentElement.dir === 'rtl'; }

  factors = signal<UomConversionFactor[]>([]);
  selectedIds   = signal<Set<any>>(new Set());
  metaReloadKey = signal(0);
  activeFilter  = signal<Record<string, string | number | null>>({});
  columnMeta    = signal<ColumnMeta[]>([]);

  uomLabel(uom?: UOM): string {
    if (!uom) return '';
    return this.isRtl ? (uom.name_AR || uom.name_EN) : (uom.name_EN || uom.name_AR);
  }

  groupLabel(g?: Group): string {
    if (!g) return '—';
    return this.isRtl ? (g.name_AR || g.name_EN) : (g.name_EN || g.name_AR);
  }

  get searchFields(): SearchField[] {
    const fromMap = new Map<number, UOM>();
    const toMap = new Map<number, UOM>();
    const groupMap = new Map<number, Group>();
    for (const f of this.factors()) {
      if (f.uomFrom) fromMap.set(f.uomFromId, f.uomFrom);
      if (f.uomTo) toMap.set(f.uomToId, f.uomTo);
      if (f.uomConversionGroup && f.uomConversionGroupId != null)
        groupMap.set(f.uomConversionGroupId, f.uomConversionGroup);
    }
    const uomOpts = (map: Map<number, UOM>) =>
      [...map.values()].map(u => ({ value: u.id, label: this.uomLabel(u) }))
        .sort((a, b) => a.label.localeCompare(b.label));
    const grpOpts = [...groupMap.values()]
      .map(g => ({ value: g.id, label: this.groupLabel(g) }))
      .sort((a, b) => a.label.localeCompare(b.label));

    return this.meta.toSearchFields(this.columnMeta(), this.isRtl, {
      uomConversionGroup: grpOpts,
      uomFrom: uomOpts(fromMap),
      uomTo: uomOpts(toMap),
    });
  }

  get sortedFactors(): UomConversionFactor[] {
    return [...this.factors()].sort((a, b) => {
      if (a.isFavorite !== b.isFavorite) return a.isFavorite ? -1 : 1;
      return this.uomLabel(a.uomFrom).localeCompare(this.uomLabel(b.uomFrom));
    });
  }

  get filteredFactors(): UomConversionFactor[] {
    const f = this.activeFilter();
    return this.sortedFactors.filter(x => {
      if (f['internalCode'] != null && !(x.internalCode ?? '').toLowerCase().includes((f['internalCode'] as string).toLowerCase())) return false;
      if (f['uomFrom'] != null && x.uomFromId !== f['uomFrom']) return false;
      if (f['uomTo'] != null && x.uomToId !== f['uomTo']) return false;
      if (f['uomConversionGroup'] != null && x.uomConversionGroupId !== f['uomConversionGroup']) return false;
      if (f['isActive'] != null && x.isActive !== (f['isActive'] === 1)) return false;
      return true;
    });
  }

  readonly cellRenderers: Record<string, (item: any) => string> = {
    uomFrom: (item) => this.uomLabel(item.uomFrom),
    uomTo: (item) => this.uomLabel(item.uomTo),
    uomConversionGroup: (item) => this.groupLabel(item.uomConversionGroup),
  };

  onFilterChange(filter: Record<string, string | number | null>) {
    this.activeFilter.set(filter);
  }

  ngOnInit() {
    this.load();
  }

  load() {
    this.metaReloadKey.update(n => n + 1);
    this.api.get<UomConversionFactor[]>('uomconversionfactors').subscribe(d => this.factors.set(d));
  }

  // ── CRUD ───────────────────────────────────────────────────────────────────

  addNew() { this.router.navigate(['/stock/uom-conversion-factors/operation']); }

  exportTemplate() {
    this.api.getBlob('uomconversionfactors/export-template').subscribe(blob => {
      const url = URL.createObjectURL(blob);
      const a = document.createElement('a');
      a.href = url;
      a.download = 'uom-conversion-factors-template.xlsx';
      a.click();
      URL.revokeObjectURL(url);
    });
  }

  importExcel(file: File) {
    const fd = new FormData();
    fd.append('file', file);
    this.api.post<{ created: number; errors: string[] }>('uomconversionfactors/import', fd).subscribe({
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

  edit(id: number) { this.router.navigate(['/stock/uom-conversion-factors/operation', id]); }

  delete(id: number) {
    Swal.fire({
      title: this.translate.instant('common.swal_delete_title'),
      text: this.translate.instant('uom_conversion_factors.delete_confirm'),
      icon: 'warning',
      showCancelButton: true,
      confirmButtonText: this.translate.instant('common.delete'),
      cancelButtonText: this.translate.instant('common.cancel'),
      confirmButtonColor: '#f1416c',
      reverseButtons: this.isRtl,
    }).then(result => {
      if (result.isConfirmed)
        this.api.delete(`uomconversionfactors/${id}`).subscribe(() => this.load());
    });
  }

  toggleFavorite(f: UomConversionFactor) {
    this.api.patch<UomConversionFactor>(`uomconversionfactors/${f.id}/toggle-favorite`).subscribe(updated => {
      this.factors.update(list => list.map(x => x.id === updated.id ? { ...updated, uomFrom: x.uomFrom, uomTo: x.uomTo, uomConversionGroup: x.uomConversionGroup } : x));
    });
  }

  toggleActive(f: UomConversionFactor) {
    this.api.patch<UomConversionFactor>(`uomconversionfactors/${f.id}/toggle-active`).subscribe(updated => {
      this.factors.update(list => list.map(x => x.id === updated.id ? { ...updated, uomFrom: x.uomFrom, uomTo: x.uomTo, uomConversionGroup: x.uomConversionGroup } : x));
    });
  }

  deleteSelected() {
    const ids = [...this.selectedIds()];
    if (!ids.length) return;
    Swal.fire({
      title: this.translate.instant('common.swal_delete_title'),
      text: this.translate.instant('uom_conversion_factors.delete_selected_confirm', { count: ids.length }),
      icon: 'warning',
      showCancelButton: true,
      confirmButtonText: this.translate.instant('common.delete'),
      cancelButtonText: this.translate.instant('common.cancel'),
      confirmButtonColor: '#f1416c',
      reverseButtons: this.isRtl,
    }).then(result => {
      if (result.isConfirmed)
        this.api.deleteBulk('uomconversionfactors/bulk', ids).subscribe(() => this.load());
    });
  }
}
