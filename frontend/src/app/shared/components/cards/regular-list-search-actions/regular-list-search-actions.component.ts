import { Component, inject, input, output } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { CustomSelectInputComponent } from '../../custom-controls/custom-select-input/custom-select-input.component';

export interface SearchField {
  key: string;
  label: string;
  type: 'text' | 'select' | 'number' | 'boolean';
  placeholder?: string;
  options?: { value: string | number; label: string }[];
}

@Component({
  selector: 'app-regular-list-search-actions',
  standalone: true,
  imports: [FormsModule, TranslatePipe, CustomSelectInputComponent],
  templateUrl: './regular-list-search-actions.component.html',
  styleUrl: './regular-list-search-actions.component.less',
})
export class RegularListSearchActionsComponent {
  private translate = inject(TranslateService);

  fields = input<SearchField[]>([]);
  filterChange = output<Record<string, string | number | null>>();

  values: Record<string, string | number | null> = {};

  get booleanOptions() {
    return [
      { value: 1, label: this.translate.instant('common.yes') },
      { value: 0, label: this.translate.instant('common.no') },
    ];
  }

  onValueChange(key: string, value: string | number | null) {
    this.values[key] = value;
    this.filterChange.emit({ ...this.values });
  }

  clear() {
    this.values = {};
    this.filterChange.emit({});
  }
}
