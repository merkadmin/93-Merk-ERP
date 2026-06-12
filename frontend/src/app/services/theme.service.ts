import { Injectable, signal, computed } from '@angular/core';

export type ThemeMode = 'light' | 'dark';

const STORAGE_KEY = 'data-bs-theme';

@Injectable({ providedIn: 'root' })
export class ThemeService {
  private _mode = signal<ThemeMode>(this._loadStored());

  readonly mode = this._mode.asReadonly();
  readonly isDark = computed(() => this._mode() === 'dark');

  constructor() {
    this._apply(this._mode());
  }

  setMode(mode: ThemeMode): void {
    this._mode.set(mode);
    localStorage.setItem(STORAGE_KEY, mode);
    this._apply(mode);
  }

  toggle(): void {
    this.setMode(this._mode() === 'dark' ? 'light' : 'dark');
  }

  private _apply(mode: ThemeMode): void {
    document.documentElement.setAttribute('data-bs-theme', mode);
  }

  private _loadStored(): ThemeMode {
    const stored = localStorage.getItem(STORAGE_KEY);
    return stored === 'dark' ? 'dark' : 'light';
  }
}
