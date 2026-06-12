import { inject, Injectable } from '@angular/core';
import { Observable, of, tap } from 'rxjs';
import { SearchField } from '../shared/components/cards/regular-list-search-actions/regular-list-search-actions.component';
import { ApiService } from './api.service';

export interface ColumnMeta {
  key: string;
  labelEN: string;
  labelAR: string;
  columnOrder: number;
  entityProperty: string;
  foreignKeyProperty?: string;
  filterType: string;
  dataType: string;
  renderAs: string;
  isSortable: boolean;
  isFilterable: boolean;
  isVisible: boolean;
  minWidth?: number;
}

export interface EntityMeta {
  entityKey: string;
  dbTable: string;
  modelClass: string;
  columns: ColumnMeta[];
}

export type SelectOption = { value: string | number; label: string };

@Injectable({ providedIn: 'root' })
export class MetadataService {
  private api = inject(ApiService);
  private cache = new Map<string, EntityMeta>();

  get(entity: string): Observable<EntityMeta> {
    if (this.cache.has(entity)) return of(this.cache.get(entity)!);
    return this.api.get<EntityMeta>(`metadata/${entity}`).pipe(
      tap(m => this.cache.set(entity, m))
    );
  }

  invalidate(entity: string): void {
    this.cache.delete(entity);
  }

  /**
   * Converts filterable columns to SearchField[].
   * - text / number  → rendered as text/number input (auto-included)
   * - boolean        → rendered as Yes/No select (auto-included)
   * - select         → rendered as dropdown — only included when selectOptions[key] is provided
   */
  toSearchFields(
    columns: ColumnMeta[],
    isRtl: boolean,
    selectOptions: Record<string, SelectOption[]> = {}
  ): SearchField[] {
    return columns
      .filter(c => c.isFilterable)
      .sort((a, b) => a.columnOrder - b.columnOrder)
      .filter(c => c.filterType !== 'select' || c.key in selectOptions)
      .map(c => ({
        key: c.key,
        label: isRtl ? c.labelAR : c.labelEN,
        type: (c.filterType === 'boolean' ? 'boolean'
          : c.filterType === 'number' ? 'number'
            : c.filterType === 'select' ? 'select'
              : 'text') as SearchField['type'],
        options: c.filterType === 'select' ? selectOptions[c.key] : undefined,
      }));
  }
}
