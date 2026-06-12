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

interface WareHouseCategory { id: number; internalCode: string | null; name_EN: string; name_AR: string | null; description: string | null; isActive: boolean; }

@Component({
  selector: 'app-warehouse-categories',
  standalone: true,
  imports: [TranslatePipe, RegularListHeaderWithActionsComponent, RegularListSearchActionsComponent, CustomTableWithPaginationComponent],
  templateUrl: './warehouse-categories.component.html',
  styleUrl: './warehouse-categories.component.less',
})
export class WarehouseCategoriesComponent implements OnInit {
  private api = inject(ApiService);
  private router = inject(Router);
  private translate = inject(TranslateService);
  private toastr = inject(ToastrService);
  private doc = inject(DOCUMENT);
  private meta = inject(MetadataService);

  get isRtl() { return this.doc.documentElement.dir === 'rtl'; }

  categories = signal<WareHouseCategory[]>([]);
  selectedIds = signal<Set<any>>(new Set());
  activeFilter = signal<Record<string, string | number | null>>({});
  columnMeta = signal<ColumnMeta[]>([]);

  label(c: WareHouseCategory): string {
    return this.isRtl ? (c.name_AR || c.name_EN) : (c.name_EN || c.name_AR || '');
  }

  get searchFields(): SearchField[] {
    return this.meta.toSearchFields(this.columnMeta(), this.isRtl);
  }

  get filtered(): WareHouseCategory[] {
    const f = this.activeFilter();
    return this.categories().filter(c => {
      if (f['internalCode'] != null && !(c.internalCode ?? '').toLowerCase().includes((f['internalCode'] as string).toLowerCase())) return false;
      if (f['name_AR'] != null && !(c.name_AR ?? '').toLowerCase().includes((f['name_AR'] as string).toLowerCase())) return false;
      if (f['name_EN'] != null && !c.name_EN.toLowerCase().includes((f['name_EN'] as string).toLowerCase())) return false;
      if (f['isActive'] != null && c.isActive !== (f['isActive'] === 1)) return false;
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
    this.api.get<WareHouseCategory[]>('warehousecategories').subscribe(d => this.categories.set(d));
  }

  addNew() { this.router.navigate(['/stock/warehouse-categories/operation']); }
  edit(id: number) { this.router.navigate(['/stock/warehouse-categories/operation', id]); }

  exportTemplate() {
    this.api.getBlob('warehousecategories/export-template').subscribe(blob => {
      const url = URL.createObjectURL(blob);
      const a = document.createElement('a');
      a.href = url;
      a.download = 'warehouse-categories-template.xlsx';
      a.click();
      URL.revokeObjectURL(url);
    });
  }

  importExcel(file: File) {
    const fd = new FormData();
    fd.append('file', file);
    this.api.post<{ created: number; errors: string[] }>('warehousecategories/import', fd).subscribe({
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

  toggleActive(c: WareHouseCategory) {
    this.api.patch<WareHouseCategory>(`warehousecategories/${c.id}/toggle-active`).subscribe(updated =>
      this.categories.update(list => list.map(x => x.id === updated.id ? updated : x))
    );
  }

  delete(id: number) {
    Swal.fire({
      title: this.translate.instant('common.swal_delete_title'),
      text: this.translate.instant('warehouse_categories.delete_confirm'),
      icon: 'warning',
      showCancelButton: true,
      confirmButtonText: this.translate.instant('common.delete'),
      cancelButtonText: this.translate.instant('common.cancel'),
      confirmButtonColor: '#f1416c',
      reverseButtons: this.isRtl,
    }).then(r => { if (r.isConfirmed) this.api.delete(`warehousecategories/${id}`).subscribe(() => this.load()); });
  }

  deleteSelected() {
    const ids = [...this.selectedIds()];
    if (!ids.length) return;
    Swal.fire({
      title: this.translate.instant('common.swal_delete_title'),
      text: this.translate.instant('warehouse_categories.delete_selected_confirm', { count: ids.length }),
      icon: 'warning',
      showCancelButton: true,
      confirmButtonText: this.translate.instant('common.delete'),
      cancelButtonText: this.translate.instant('common.cancel'),
      confirmButtonColor: '#f1416c',
      reverseButtons: this.isRtl,
    }).then(r => { if (r.isConfirmed) this.api.deleteBulk('warehousecategories/bulk', ids).subscribe(() => this.load()); });
  }
}
