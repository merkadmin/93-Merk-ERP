import { DOCUMENT } from '@angular/common';
import { Component, inject, OnInit, signal } from '@angular/core';
import { Router } from '@angular/router';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { ToastrService } from 'ngx-toastr';
import Swal from 'sweetalert2';
import { ApiService } from '../../core/api.service';
import { RegularListSearchActionsComponent, SearchField } from '../../shared/components/cards/regular-list-search-actions/regular-list-search-actions.component';
import { RegularListHeaderWithActionsComponent } from '../../shared/components/cards/regular-list-header-with-actions/regular-list-header-with-actions.component';

interface UOM {
  id: number;
  internalCode: string;
  name_EN: string;
  name_AR: string;
  mustBeWholeNumber: boolean;
  isActive: boolean;
  isFavorite: boolean;
}

@Component({
  selector: 'app-uoms',
  standalone: true,
  imports: [TranslatePipe, RegularListSearchActionsComponent, RegularListHeaderWithActionsComponent],
  templateUrl: './uoms.component.html',
  styleUrl: './uoms.component.less',
})
export class UomsComponent implements OnInit {
  private api       = inject(ApiService);
  private router    = inject(Router);
  private translate = inject(TranslateService);
  private toastr    = inject(ToastrService);
  private doc       = inject(DOCUMENT);

  get isRtl() { return this.doc.documentElement.dir === 'rtl'; }

  uoms         = signal<UOM[]>([]);
  selectedIds  = signal<Set<number>>(new Set());
  activeFilter = signal<Record<string, string | number | null>>({});

  nameLabel(u: UOM): string {
    return this.isRtl ? (u.name_AR || u.name_EN) : (u.name_EN || u.name_AR);
  }

  get searchFields(): SearchField[] {
    return [
      { key: 'internalCode', label: this.translate.instant('common.internal_code'), type: 'text' },
      { key: 'name',         label: this.translate.instant('common.name'),         type: 'text' },
    ];
  }

  get sortedUoms(): UOM[] {
    return [...this.uoms()].sort((a, b) => {
      if (a.isFavorite !== b.isFavorite) return a.isFavorite ? -1 : 1;
      return this.nameLabel(a).localeCompare(this.nameLabel(b));
    });
  }

  get filteredUoms(): UOM[] {
    const f = this.activeFilter();
    return this.sortedUoms.filter(u => {
      if (f['internalCode'] != null && !(u.internalCode ?? '').toLowerCase().includes((f['internalCode'] as string).toLowerCase())) return false;
      if (f['name'] != null) {
        const q = (f['name'] as string).toLowerCase();
        if (!(u.name_EN ?? '').toLowerCase().includes(q) && !(u.name_AR ?? '').toLowerCase().includes(q)) return false;
      }
      return true;
    });
  }

  onFilterChange(filter: Record<string, string | number | null>) {
    this.activeFilter.set(filter);
    this.selectedIds.set(new Set());
  }

  ngOnInit() { this.load(); }

  load() {
    this.api.get<UOM[]>('uoms').subscribe(d => {
      this.uoms.set(d);
      this.selectedIds.set(new Set());
    });
  }

  isSelected(id: number) { return this.selectedIds().has(id); }

  get isAllSelected() {
    const u = this.filteredUoms;
    return u.length > 0 && u.every(item => this.selectedIds().has(item.id));
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
      this.selectedIds.set(new Set(this.filteredUoms.map(u => u.id)));
    }
  }

  addNew() { this.router.navigate(['/stock/uoms/operation']); }

  exportTemplate() {
    this.api.getBlob('uoms/export-template').subscribe(blob => {
      const url = URL.createObjectURL(blob);
      const a   = document.createElement('a');
      a.href     = url;
      a.download = 'uoms-template.xlsx';
      a.click();
      URL.revokeObjectURL(url);
    });
  }

  importExcel(file: File) {
    const fd = new FormData();
    fd.append('file', file);
    this.api.post<{ created: number; errors: string[] }>('uoms/import', fd).subscribe({
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

  edit(id: number) { this.router.navigate(['/stock/uoms/operation', id]); }

  delete(id: number) {
    Swal.fire({
      title: this.translate.instant('common.swal_delete_title'),
      text:  this.translate.instant('uoms.delete_confirm'),
      icon:  'warning',
      showCancelButton:  true,
      confirmButtonText: this.translate.instant('common.delete'),
      cancelButtonText:  this.translate.instant('common.cancel'),
      confirmButtonColor: '#f1416c',
      reverseButtons: this.isRtl,
    }).then(result => {
      if (result.isConfirmed)
        this.api.delete(`uoms/${id}`).subscribe(() => this.load());
    });
  }

  toggleFavorite(uom: UOM) {
    this.api.patch<UOM>(`uoms/${uom.id}/toggle-favorite`).subscribe(updated => {
      this.uoms.update(list => list.map(u => u.id === updated.id ? updated : u));
    });
  }

  toggleActive(uom: UOM) {
    this.api.patch<UOM>(`uoms/${uom.id}/toggle-active`).subscribe(updated => {
      this.uoms.update(list => list.map(u => u.id === updated.id ? updated : u));
    });
  }

  deleteSelected() {
    const ids = [...this.selectedIds()];
    if (!ids.length) return;
    Swal.fire({
      title: this.translate.instant('common.swal_delete_title'),
      text:  this.translate.instant('uoms.delete_selected_confirm', { count: ids.length }),
      icon:  'warning',
      showCancelButton:  true,
      confirmButtonText: this.translate.instant('common.delete'),
      cancelButtonText:  this.translate.instant('common.cancel'),
      confirmButtonColor: '#f1416c',
      reverseButtons: this.isRtl,
    }).then(result => {
      if (result.isConfirmed)
        this.api.deleteBulk('uoms/bulk', ids).subscribe(() => this.load());
    });
  }
}
