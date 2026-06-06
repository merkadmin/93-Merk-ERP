import { DOCUMENT } from '@angular/common';
import { Component, ElementRef, HostListener, inject, input, output } from '@angular/core';
import { FormsModule } from '@angular/forms';

export interface SelectOption {
  value: number | string;
  label: string;
  sublabel?: string;
}

function normalize(text: string): string {
  return text
    .toLowerCase()
    .replace(/[ً-ٰٟ]/g, '')   // strip Arabic diacritics
    .replace(/[أإآٱ]/g, 'ا') // unify alef variants
    .replace(/ة/g, 'ه')      // taa marbuta → haa
    .replace(/ى/g, 'ي');     // alef maqsura → yaa
}

@Component({
  selector: 'app-custom-select-input',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './custom-select-input.component.html',
  styleUrl: './custom-select-input.component.less',
})
export class CustomSelectInputComponent {
  private el = inject(ElementRef);
  private doc = inject(DOCUMENT);

  options = input<SelectOption[]>([]);
  value = input<number | string | null>(null);
  placeholder = input<string>('— Select —');
  searchPlaceholder = input<string>('Search...');
  noMatchText = input<string>('No results found.');
  disabled = input<boolean>(false);

  valueChange = output<number | string | null>();

  searchQuery = '';
  isOpen = false;

  get isRtl() { return this.doc.documentElement.dir === 'rtl'; }

  @HostListener('document:click', ['$event.target'])
  onDocumentClick(target: EventTarget | null): void {
    if (!this.el.nativeElement.contains(target)) {
      this.isOpen = false;
      this.searchQuery = '';
    }
  }

  get selectedLabel(): string {
    return this.options().find(o => o.value === this.value())?.label ?? '';
  }

  get filtered(): SelectOption[] {
    const q = normalize(this.searchQuery.trim());
    if (!q) return this.options();
    return this.options().filter(o =>
      normalize(o.label).includes(q) ||
      normalize(o.sublabel ?? '').includes(q)
    );
  }

  toggle(): void {
    if (this.disabled()) return;
    this.isOpen = !this.isOpen;
  }

  pick(option: SelectOption): void {
    this.isOpen = false;
    this.searchQuery = '';
    this.valueChange.emit(option.value);
  }

  clear(): void {
    this.isOpen = false;
    this.searchQuery = '';
    this.valueChange.emit(null);
  }
}
