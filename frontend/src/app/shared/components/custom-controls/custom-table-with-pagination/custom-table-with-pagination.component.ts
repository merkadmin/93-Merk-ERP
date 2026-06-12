import { DOCUMENT, NgTemplateOutlet } from '@angular/common';
import {
  Component, ContentChild, OnInit, TemplateRef,
  effect, inject, input, output, signal, untracked
} from '@angular/core';
import { TranslatePipe } from '@ngx-translate/core';
import { ColumnMeta, MetadataService } from '../../../../core/metadata.service';
import { CustomTablePaginationComponent } from '../../custom-table-pagination/custom-table-pagination.component';

@Component({
  selector: 'app-custom-table-with-pagination',
  standalone: true,
  imports: [NgTemplateOutlet, TranslatePipe, CustomTablePaginationComponent],
  templateUrl: './custom-table-with-pagination.component.html',
  styleUrl: './custom-table-with-pagination.component.less',
})
export class CustomTableWithPaginationComponent implements OnInit {
  private meta = inject(MetadataService);
  private doc  = inject(DOCUMENT);

  // ── Inputs ────────────────────────────────────────────────────────────────
  entity        = input.required<string>();
  rows          = input<any[]>([]);
  idKey         = input<string>('id');
  cellRenderers = input<Record<string, (row: any) => string>>({});

  // ── Outputs ───────────────────────────────────────────────────────────────
  metadataLoaded  = output<ColumnMeta[]>();
  selectionChange = output<Set<any>>();

  // ── Content templates ─────────────────────────────────────────────────────
  @ContentChild('rowActions') rowActionsTemplate?: TemplateRef<{ $implicit: any }>;
  @ContentChild('treeCell')   treeCellTemplate?:   TemplateRef<{ $implicit: any }>;

  // ── State ─────────────────────────────────────────────────────────────────
  columnMeta   = signal<ColumnMeta[]>([]);
  selectedIds  = signal<Set<any>>(new Set());
  displayRows  = signal<any[]>([]);

  constructor() {
    effect(() => {
      this.rows();
      untracked(() => {
        const empty = new Set<any>();
        this.selectedIds.set(empty);
        this.selectionChange.emit(empty);
      });
    });
  }

  ngOnInit(): void {
    this.meta.get(this.entity()).subscribe(m => {
      this.columnMeta.set(m.columns);
      this.metadataLoaded.emit(m.columns);
    });
  }

  // ── Computed ──────────────────────────────────────────────────────────────
  get isRtl(): boolean { return this.doc.documentElement.dir === 'rtl'; }

  get visibleColumns(): ColumnMeta[] {
    return this.columnMeta().filter(c => c.isVisible);
  }

  // ── Selection ─────────────────────────────────────────────────────────────
  get isAllSelected(): boolean {
    const p = this.displayRows();
    return p.length > 0 && p.every(r => this.selectedIds().has(r[this.idKey()]));
  }

  get isIndeterminate(): boolean {
    return this.selectedIds().size > 0 && !this.isAllSelected;
  }

  isSelected(row: any): boolean {
    return this.selectedIds().has(row[this.idKey()]);
  }

  toggleOne(row: any): void {
    const s  = new Set(this.selectedIds());
    const id = row[this.idKey()];
    s.has(id) ? s.delete(id) : s.add(id);
    this.selectedIds.set(s);
    this.selectionChange.emit(s);
  }

  toggleAll(): void {
    const s = new Set(this.selectedIds());
    if (this.isAllSelected) {
      for (const row of this.displayRows()) s.delete(row[this.idKey()]);
    } else {
      for (const row of this.displayRows()) s.add(row[this.idKey()]);
    }
    this.selectedIds.set(s);
    this.selectionChange.emit(s);
  }

  // ── Cell rendering ─────────────────────────────────────────────────────────
  getCellText(row: any, col: ColumnMeta): string {
    const renderer = this.cellRenderers()[col.key];
    if (renderer) return renderer(row);
    const v = row[col.key];
    return v != null && v !== '' ? String(v) : '—';
  }

  getCellBoolean(row: any, col: ColumnMeta): boolean {
    return !!row[col.key];
  }
}
