import { Component, ElementRef, input, output, ViewChild } from '@angular/core';
import { TranslatePipe } from '@ngx-translate/core';

@Component({
  selector: 'app-regular-list-header-with-actions',
  standalone: true,
  imports: [TranslatePipe],
  templateUrl: './regular-list-header-with-actions.component.html',
  styleUrl: './regular-list-header-with-actions.component.less',
})
export class RegularListHeaderWithActionsComponent {
  @ViewChild('fileInput') fileInput!: ElementRef<HTMLInputElement>;

  title = input<string>('');
  icon = input<string>('');
  color = input<string>('primary');
  selectedCount = input<number>(0);
  showImportExport = input<boolean>(false);

  add = output<void>();
  refresh = output<void>();
  deleteSelected = output<void>();
  exportTemplate = output<void>();
  importFile = output<File>();

  triggerImport() {
    this.fileInput.nativeElement.value = '';
    this.fileInput.nativeElement.click();
  }

  onFileSelected(event: Event) {
    const file = (event.target as HTMLInputElement).files?.[0];
    if (file) this.importFile.emit(file);
  }
}
