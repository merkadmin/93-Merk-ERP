import { DOCUMENT } from '@angular/common';
import { Component, inject, OnInit, signal } from '@angular/core';
import { Router } from '@angular/router';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import Swal from 'sweetalert2';
import { ApiService } from '../../core/api.service';
import { RegularListSearchActionsComponent, SearchField } from '../../shared/components/cards/regular-list-search-actions/regular-list-search-actions.component';
import { RegularListHeaderWithActionsComponent } from '../../shared/components/cards/regular-list-header-with-actions/regular-list-header-with-actions.component';

interface UOM   { id: number; name_EN: string; name_AR: string; }
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
  imports: [TranslatePipe, RegularListSearchActionsComponent, RegularListHeaderWithActionsComponent],
  templateUrl: './uom-conversion-factors.component.html',
  styleUrl: './uom-conversion-factors.component.less',
})
export class UomConversionFactorsComponent implements OnInit {
  private api       = inject(ApiService);
  private router    = inject(Router);
  private translate = inject(TranslateService);
  private doc       = inject(DOCUMENT);

  get isRtl() { return this.doc.documentElement.dir === 'rtl'; }

  factors      = signal<UomConversionFactor[]>([]);
  selectedIds  = signal<Set<number>>(new Set());
  activeFilter = signal<Record<string, string | number | null>>({});

  uomLabel(uom?: UOM): string {
    if (!uom) return '';
    return this.isRtl ? (uom.name_AR || uom.name_EN) : (uom.name_EN || uom.name_AR);
  }

  groupLabel(g?: Group): string {
    if (!g) return '—';
    return this.isRtl ? (g.name_AR || g.name_EN) : (g.name_EN || g.name_AR);
  }

  get searchFields(): SearchField[] {
    const factors = this.factors();
    const fromMap  = new Map<number, UOM>();
    const toMap    = new Map<number, UOM>();
    const groupMap = new Map<number, Group>();
    for (const f of factors) {
      if (f.uomFrom)            fromMap.set(f.uomFromId, f.uomFrom);
      if (f.uomTo)              toMap.set(f.uomToId, f.uomTo);
      if (f.uomConversionGroup && f.uomConversionGroupId != null)
        groupMap.set(f.uomConversionGroupId, f.uomConversionGroup);
    }
    const uomOpts = (map: Map<number, UOM>) =>
      [...map.values()].map(u => ({ value: u.id, label: this.uomLabel(u) }))
        .sort((a, b) => a.label.localeCompare(b.label));
    const grpOpts = [...groupMap.values()]
      .map(g => ({ value: g.id, label: this.groupLabel(g) }))
      .sort((a, b) => a.label.localeCompare(b.label));

    return [
      { key: 'groupId',      label: this.translate.instant('uom_conversion_factors.conversion_group'), type: 'select', options: grpOpts           },
      { key: 'internalCode', label: this.translate.instant('uom_conversion_factors.internal_code'),    type: 'text'                              },
      { key: 'uomFrom',      label: this.translate.instant('uom_conversion_factors.from_uom'),         type: 'select', options: uomOpts(fromMap) },
      { key: 'uomTo',        label: this.translate.instant('uom_conversion_factors.to_uom'),           type: 'select', options: uomOpts(toMap)   },
    ];
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
      if (f['groupId']      != null && x.uomConversionGroupId !== f['groupId'])                              return false;
      if (f['internalCode'] != null && !(x.internalCode ?? '').toLowerCase().includes((f['internalCode'] as string).toLowerCase())) return false;
      if (f['uomFrom']      != null && x.uomFromId            !== f['uomFrom'])                              return false;
      if (f['uomTo']        != null && x.uomToId              !== f['uomTo'])                                return false;
      return true;
    });
  }

  onFilterChange(filter: Record<string, string | number | null>) {
    this.activeFilter.set(filter);
    this.selectedIds.set(new Set());
  }

  ngOnInit() { this.load(); }

  load() {
    this.api.get<UomConversionFactor[]>('uomconversionfactors').subscribe(d => {
      this.factors.set(d);
      this.selectedIds.set(new Set());
    });
  }

  // ── Selection ──────────────────────────────────────────────────────────────

  isSelected(id: number) { return this.selectedIds().has(id); }

  get isAllSelected() {
    const f = this.filteredFactors;
    return f.length > 0 && f.every(item => this.selectedIds().has(item.id));
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
      this.selectedIds.set(new Set(this.filteredFactors.map(f => f.id)));
    }
  }

  // ── CRUD ───────────────────────────────────────────────────────────────────

  addNew() { this.router.navigate(['/stock/uom-conversion-factors/operation']); }

  edit(id: number) { this.router.navigate(['/stock/uom-conversion-factors/operation', id]); }

  delete(id: number) {
    Swal.fire({
      title: this.translate.instant('common.swal_delete_title'),
      text:  this.translate.instant('uom_conversion_factors.delete_confirm'),
      icon:  'warning',
      showCancelButton:  true,
      confirmButtonText: this.translate.instant('common.delete'),
      cancelButtonText:  this.translate.instant('common.cancel'),
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
      text:  this.translate.instant('uom_conversion_factors.delete_selected_confirm', { count: ids.length }),
      icon:  'warning',
      showCancelButton:  true,
      confirmButtonText: this.translate.instant('common.delete'),
      cancelButtonText:  this.translate.instant('common.cancel'),
      confirmButtonColor: '#f1416c',
      reverseButtons: this.isRtl,
    }).then(result => {
      if (result.isConfirmed)
        this.api.deleteBulk('uomconversionfactors/bulk', ids).subscribe(() => this.load());
    });
  }
}
