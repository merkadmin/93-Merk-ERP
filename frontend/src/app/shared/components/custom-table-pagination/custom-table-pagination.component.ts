import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, OnChanges, Output, SimpleChanges, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { TranslatePipe } from '@ngx-translate/core';
import { LanguageService } from '../../../services/language.service';

@Component({
  selector: 'app-custom-table-pagination',
  standalone: true,
  imports: [CommonModule, FormsModule, TranslatePipe],
  templateUrl: './custom-table-pagination.component.html',
  styleUrl: './custom-table-pagination.component.less',
})
export class CustomTablePaginationComponent implements OnChanges {
  @Input() items: any[] = [];
  @Input() pageSizeOptions: number[] = [10, 25, 50, 75, 100];
  @Input() defaultPageSize = 25;

  // Controlled sync inputs — set by parent to keep multiple instances in step
  @Input() syncPage:     number | null = null;
  @Input() syncPageSize: number | null = null;

  @Output() pagedDataChange = new EventEmitter<any[]>();
  @Output() pageChange      = new EventEmitter<number>();
  @Output() pageSizeChange  = new EventEmitter<number>();

  private lang = inject(LanguageService);

  pageSize    = 25;
  currentPage = 1;

  get isAr(): boolean { return this.lang.lang() === 'ar'; }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['defaultPageSize']?.firstChange) {
      this.pageSize = this.defaultPageSize;
    }
    if (changes['syncPageSize'] && this.syncPageSize !== null && this.syncPageSize !== this.pageSize) {
      this.pageSize    = this.syncPageSize;
      this.currentPage = 1;
    }
    if (changes['syncPage'] && this.syncPage !== null && this.syncPage !== this.currentPage) {
      this.currentPage = this.syncPage;
    }
    if (changes['items']) {
      this.currentPage = 1;
    }
    this.emit();
  }

  // ── Computed ──────────────────────────────────────────────────────────────
  get total(): number      { return this.items.length; }
  get totalPages(): number { return Math.max(1, Math.ceil(this.total / this.pageSize)); }
  get rangeFrom(): number  { return this.total === 0 ? 0 : (this.currentPage - 1) * this.pageSize + 1; }
  get rangeTo(): number    { return Math.min(this.currentPage * this.pageSize, this.total); }

  get visiblePages(): (number | '...')[] {
    const total = this.totalPages;
    const cur   = this.currentPage;
    if (total <= 7) return Array.from({ length: total }, (_, i) => i + 1);
    // Always exactly 7 elements — prevents layout shifts
    if (cur <= 4)         return [1, 2, 3, 4, 5, '...', total];
    if (cur >= total - 3) return [1, '...', total - 4, total - 3, total - 2, total - 1, total];
    return                       [1, '...', cur - 1, cur, cur + 1, '...', total];
  }

  // ── Actions ───────────────────────────────────────────────────────────────
  onPageSizeChange(size: number): void {
    this.pageSize    = +size;
    this.currentPage = 1;
    this.emit();
    this.pageSizeChange.emit(this.pageSize);
    this.pageChange.emit(this.currentPage);
  }

  goToPage(p: number): void {
    if (p < 1 || p > this.totalPages) return;
    this.currentPage = p;
    this.emit();
    this.pageChange.emit(this.currentPage);
  }

  goFirst(): void { this.goToPage(1); }
  goLast():  void { this.goToPage(this.totalPages); }

  private emit(): void {
    const start = (this.currentPage - 1) * this.pageSize;
    this.pagedDataChange.emit(this.items.slice(start, start + this.pageSize));
  }
}
