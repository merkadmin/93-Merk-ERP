import { Component, input, output } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { TranslatePipe } from '@ngx-translate/core';

export interface SearchField {
  key: string;
  label: string;
  type: 'text' | 'select' | 'number';
  placeholder?: string;
  options?: { value: string | number; label: string }[];
}

@Component({
  selector: 'app-regular-list-search-actions',
  standalone: true,
  imports: [FormsModule, TranslatePipe],
  templateUrl: './regular-list-search-actions.component.html',
  styleUrl: './regular-list-search-actions.component.less',
})
export class RegularListSearchActionsComponent {
  fields       = input<SearchField[]>([]);
  filterChange = output<Record<string, string | number | null>>();

  values: Record<string, string | number | null> = {};

  onValueChange(key: string, value: string | number | null) {
    this.values[key] = value;
    this.filterChange.emit({ ...this.values });
  }

  clear() {
    this.values = {};
    this.filterChange.emit({});
  }
}
