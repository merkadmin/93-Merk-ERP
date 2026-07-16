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

interface Currency { id: number; code: string; name_EN: string; name_AR: string | null; }
interface Company {
  id: number;
  internalCode: string | null;
  name_EN: string;
  name_AR: string | null;
  abbr: string;
  defaultCurrencyId: number | null;
  country: string | null;
  phone: string | null;
  isActive: boolean;
  defaultCurrency?: Currency;
}

@Component({
  selector: 'app-companies',
  standalone: true,
  imports: [TranslatePipe, RegularListHeaderWithActionsComponent, RegularListSearchActionsComponent, CustomTableWithPaginationComponent],
  templateUrl: './companies.component.html',
  styleUrl: './companies.component.less',
})
export class CompaniesComponent implements OnInit {
  private api = inject(ApiService);
  private router = inject(Router);
  private translate = inject(TranslateService);
  private toastr = inject(ToastrService);
  private doc = inject(DOCUMENT);
  private meta = inject(MetadataService);

  get isRtl() { return this.doc.documentElement.dir === 'rtl'; }

  companies     = signal<Company[]>([]);
  selectedIds   = signal<Set<any>>(new Set());
  metaReloadKey = signal(0);
  activeFilter  = signal<Record<string, string | number | null>>({});
  columnMeta    = signal<ColumnMeta[]>([]);

  readonly cellRenderers: Record<string, (item: any) => string> = {
    defaultCurrency: (c: Company) => {
      if (!c.defaultCurrency) return '—';
      return `${c.defaultCurrency.code} — ${this.isRtl ? (c.defaultCurrency.name_AR || c.defaultCurrency.name_EN) : c.defaultCurrency.name_EN}`;
    },
  };

  get searchFields(): SearchField[] {
    return this.meta.toSearchFields(this.columnMeta(), this.isRtl);
  }

  get filtered(): Company[] {
    const f = this.activeFilter();
    return this.companies().filter(c => {
      if (f['internalCode'] != null && !(c.internalCode ?? '').toLowerCase().includes((f['internalCode'] as string).toLowerCase())) return false;
      if (f['name_AR'] != null && !(c.name_AR ?? '').toLowerCase().includes((f['name_AR'] as string).toLowerCase())) return false;
      if (f['name_EN'] != null && !c.name_EN.toLowerCase().includes((f['name_EN'] as string).toLowerCase())) return false;
      if (f['abbr'] != null && !(c.abbr ?? '').toLowerCase().includes((f['abbr'] as string).toLowerCase())) return false;
      if (f['country'] != null && !(c.country ?? '').toLowerCase().includes((f['country'] as string).toLowerCase())) return false;
      if (f['phone'] != null && !(c.phone ?? '').toLowerCase().includes((f['phone'] as string).toLowerCase())) return false;
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
    this.metaReloadKey.update(n => n + 1);
    this.api.get<Company[]>('companies').subscribe(d => this.companies.set(d));
  }

  addNew() { this.router.navigate(['/buying/companies/operation']); }
  edit(id: number) { this.router.navigate(['/buying/companies/operation', id]); }

  toggleActive(c: Company) {
    this.api.patch<Company>(`companies/${c.id}/toggle-active`).subscribe(updated =>
      this.companies.update(list => list.map(x => x.id === updated.id ? updated : x))
    );
  }

  delete(id: number) {
    Swal.fire({
      title: this.translate.instant('common.swal_delete_title'),
      text: this.translate.instant('companies.delete_confirm'),
      icon: 'warning',
      showCancelButton: true,
      confirmButtonText: this.translate.instant('common.delete'),
      cancelButtonText: this.translate.instant('common.cancel'),
      confirmButtonColor: '#f1416c',
      reverseButtons: this.isRtl,
    }).then(r => { if (r.isConfirmed) this.api.delete(`companies/${id}`).subscribe(() => this.load()); });
  }

  deleteSelected() {
    const ids = [...this.selectedIds()];
    if (!ids.length) return;
    Swal.fire({
      title: this.translate.instant('common.swal_delete_title'),
      text: this.translate.instant('companies.delete_selected_confirm', { count: ids.length }),
      icon: 'warning',
      showCancelButton: true,
      confirmButtonText: this.translate.instant('common.delete'),
      cancelButtonText: this.translate.instant('common.cancel'),
      confirmButtonColor: '#f1416c',
      reverseButtons: this.isRtl,
    }).then(r => { if (r.isConfirmed) this.api.deleteBulk('companies/bulk', ids).subscribe(() => this.load()); });
  }
}
