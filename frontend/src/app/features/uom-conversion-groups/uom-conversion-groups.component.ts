import { DOCUMENT } from '@angular/common';
import { Component, inject, OnInit, signal } from '@angular/core';
import { Router } from '@angular/router';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import Swal from 'sweetalert2';
import { ApiService } from '../../core/api.service';

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
  imports: [TranslatePipe],
  templateUrl: './uom-conversion-groups.component.html',
  styleUrl: './uom-conversion-groups.component.less',
})
export class UomConversionGroupsComponent implements OnInit {
  private api       = inject(ApiService);
  private router    = inject(Router);
  private translate = inject(TranslateService);
  private doc       = inject(DOCUMENT);

  get isRtl() { return this.doc.documentElement.dir === 'rtl'; }

  groups      = signal<UomConversionGroup[]>([]);
  selectedIds = signal<Set<number>>(new Set());

  get sortedGroups(): UomConversionGroup[] {
    return [...this.groups()].sort((a, b) => {
      if (a.isFavorite !== b.isFavorite) return a.isFavorite ? -1 : 1;
      const nameA = this.isRtl ? (a.name_AR ?? '') : (a.name_EN ?? '');
      const nameB = this.isRtl ? (b.name_AR ?? '') : (b.name_EN ?? '');
      return nameA.localeCompare(nameB);
    });
  }

  ngOnInit() { this.load(); }

  load() {
    this.api.get<UomConversionGroup[]>('uomconversiongroups').subscribe(d => {
      this.groups.set(d);
      this.selectedIds.set(new Set());
    });
  }

  // ── Selection ──────────────────────────────────────────────────────────────

  isSelected(id: number) { return this.selectedIds().has(id); }

  get isAllSelected() {
    const g = this.groups();
    return g.length > 0 && g.every(item => this.selectedIds().has(item.id));
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
      this.selectedIds.set(new Set(this.groups().map(g => g.id)));
    }
  }

  // ── CRUD ───────────────────────────────────────────────────────────────────

  addNew() { this.router.navigate(['/uom-conversion-groups/operation']); }

  edit(id: number) { this.router.navigate(['/uom-conversion-groups/operation', id]); }

  delete(id: number) {
    Swal.fire({
      title: this.translate.instant('common.swal_delete_title'),
      text:  this.translate.instant('uom_conversion_groups.delete_confirm'),
      icon:  'warning',
      showCancelButton:  true,
      confirmButtonText: this.translate.instant('common.delete'),
      cancelButtonText:  this.translate.instant('common.cancel'),
      confirmButtonColor: '#f1416c',
      reverseButtons: this.isRtl,
    }).then(result => {
      if (result.isConfirmed)
        this.api.delete(`uomconversiongroups/${id}`).subscribe(() => this.load());
    });
  }

  toggleFavorite(group: UomConversionGroup) {
    this.api.patch<UomConversionGroup>(`uomconversiongroups/${group.id}/toggle-favorite`).subscribe(updated => {
      this.groups.update(list => list.map(g => g.id === updated.id ? updated : g));
    });
  }

  toggleActive(group: UomConversionGroup) {
    this.api.patch<UomConversionGroup>(`uomconversiongroups/${group.id}/toggle-active`).subscribe(updated => {
      this.groups.update(list => list.map(g => g.id === updated.id ? updated : g));
    });
  }

  activateSelected() {
    const ids = [...this.selectedIds()];
    if (!ids.length) return;
    this.api.patchBulk<void>('uomconversiongroups/bulk-active', { ids, isActive: true })
      .subscribe(() => this.load());
  }

  deactivateSelected() {
    const ids = [...this.selectedIds()];
    if (!ids.length) return;
    this.api.patchBulk<void>('uomconversiongroups/bulk-active', { ids, isActive: false })
      .subscribe(() => this.load());
  }

  deleteSelected() {
    const ids = [...this.selectedIds()];
    if (!ids.length) return;
    Swal.fire({
      title: this.translate.instant('common.swal_delete_title'),
      text:  this.translate.instant('uom_conversion_groups.delete_selected_confirm', { count: ids.length }),
      icon:  'warning',
      showCancelButton:  true,
      confirmButtonText: this.translate.instant('common.delete'),
      cancelButtonText:  this.translate.instant('common.cancel'),
      confirmButtonColor: '#f1416c',
      reverseButtons: this.isRtl,
    }).then(result => {
      if (result.isConfirmed)
        this.api.deleteBulk('uomconversiongroups/bulk', ids).subscribe(() => this.load());
    });
  }
}
