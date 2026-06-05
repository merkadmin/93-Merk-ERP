import { DOCUMENT } from '@angular/common';
import { Component, inject, OnInit, signal } from '@angular/core';
import { Router } from '@angular/router';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
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

  ngOnInit() { this.load(); }

  load() {
    this.api.get<UomConversionGroup[]>('uomconversiongroups').subscribe(d => {
      this.groups.set(d);
      this.selectedIds.set(new Set());
    });
  }

  // ── Selection ──────────────────────────────────────────────────────────────

  isSelected(id: number)  { return this.selectedIds().has(id); }

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
    if (!confirm(this.translate.instant('uom_conversion_groups.delete_confirm'))) return;
    this.api.delete(`uomconversiongroups/${id}`).subscribe(() => this.load());
  }

  toggleActive(group: UomConversionGroup) {
    this.api.patch<UomConversionGroup>(`uomconversiongroups/${group.id}/toggle-active`).subscribe(updated => {
      this.groups.update(list => list.map(g => g.id === updated.id ? updated : g));
    });
  }

  deleteSelected() {
    const ids = [...this.selectedIds()];
    if (!ids.length) return;
    if (!confirm(this.translate.instant('uom_conversion_groups.delete_selected_confirm', { count: ids.length }))) return;
    this.api.deleteBulk('uomconversiongroups/bulk', ids).subscribe(() => this.load());
  }
}
