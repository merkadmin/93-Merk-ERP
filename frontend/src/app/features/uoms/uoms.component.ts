import { DOCUMENT } from '@angular/common';
import { Component, inject, OnInit, signal } from '@angular/core';
import { Router } from '@angular/router';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import Swal from 'sweetalert2';
import { ApiService } from '../../core/api.service';

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
  imports: [TranslatePipe],
  templateUrl: './uoms.component.html',
  styleUrl: './uoms.component.less',
})
export class UomsComponent implements OnInit {
  private api       = inject(ApiService);
  private router    = inject(Router);
  private translate = inject(TranslateService);
  private doc       = inject(DOCUMENT);

  get isRtl() { return this.doc.documentElement.dir === 'rtl'; }

  uoms        = signal<UOM[]>([]);
  selectedIds = signal<Set<number>>(new Set());

  get sortedUoms(): UOM[] {
    return [...this.uoms()].sort((a, b) => {
      if (a.isFavorite !== b.isFavorite) return a.isFavorite ? -1 : 1;
      const nameA = this.isRtl ? (a.name_AR ?? '') : (a.name_EN ?? '');
      const nameB = this.isRtl ? (b.name_AR ?? '') : (b.name_EN ?? '');
      return nameA.localeCompare(nameB);
    });
  }

  ngOnInit() { this.load(); }

  load() {
    this.api.get<UOM[]>('uoms').subscribe(d => {
      this.uoms.set(d);
      this.selectedIds.set(new Set());
    });
  }

  // ── Selection ──────────────────────────────────────────────────────────────

  isSelected(id: number) { return this.selectedIds().has(id); }

  get isAllSelected() {
    const u = this.uoms();
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
      this.selectedIds.set(new Set(this.uoms().map(u => u.id)));
    }
  }

  // ── CRUD ───────────────────────────────────────────────────────────────────

  addNew() { this.router.navigate(['/stock/uoms/operation']); }

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
