import { DOCUMENT } from '@angular/common';
import { Component, inject, OnInit, signal } from '@angular/core';
import { Router } from '@angular/router';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import Swal from 'sweetalert2';
import { ApiService } from '../../core/api.service';

interface UOM { id: number; name_EN: string; name_AR: string; }

interface UomConversionFactor {
  id: number;
  uomFromId: number;
  uomToId: number;
  value: number;
  isActive: boolean;
  isFavorite: boolean;
  uomFrom?: UOM;
  uomTo?: UOM;
}

@Component({
  selector: 'app-uom-conversion-factors',
  standalone: true,
  imports: [TranslatePipe],
  templateUrl: './uom-conversion-factors.component.html',
  styleUrl: './uom-conversion-factors.component.less',
})
export class UomConversionFactorsComponent implements OnInit {
  private api       = inject(ApiService);
  private router    = inject(Router);
  private translate = inject(TranslateService);
  private doc       = inject(DOCUMENT);

  get isRtl() { return this.doc.documentElement.dir === 'rtl'; }

  factors     = signal<UomConversionFactor[]>([]);
  selectedIds = signal<Set<number>>(new Set());

  uomLabel(uom?: UOM): string {
    if (!uom) return '';
    return this.isRtl ? (uom.name_AR || uom.name_EN) : (uom.name_EN || uom.name_AR);
  }

  get sortedFactors(): UomConversionFactor[] {
    return [...this.factors()].sort((a, b) => {
      if (a.isFavorite !== b.isFavorite) return a.isFavorite ? -1 : 1;
      return this.uomLabel(a.uomFrom).localeCompare(this.uomLabel(b.uomFrom));
    });
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
    const f = this.factors();
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
      this.selectedIds.set(new Set(this.factors().map(f => f.id)));
    }
  }

  // ── CRUD ───────────────────────────────────────────────────────────────────

  addNew() { this.router.navigate(['/uom-conversion-factors/operation']); }

  edit(id: number) { this.router.navigate(['/uom-conversion-factors/operation', id]); }

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
      this.factors.update(list => list.map(x => x.id === updated.id ? { ...updated, uomFrom: x.uomFrom, uomTo: x.uomTo } : x));
    });
  }

  toggleActive(f: UomConversionFactor) {
    this.api.patch<UomConversionFactor>(`uomconversionfactors/${f.id}/toggle-active`).subscribe(updated => {
      this.factors.update(list => list.map(x => x.id === updated.id ? { ...updated, uomFrom: x.uomFrom, uomTo: x.uomTo } : x));
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
